import torch
from torch import nn
from torch_geometric.data import HeteroData
from torch_geometric.nn import HeteroConv, GraphConv
import matplotlib.pyplot as plt

# === Generalized GNN Model for Recommender System ===
class GNNRecommender(nn.Module):
    def __init__(self, hidden_channels, metadata, edge_types, dropout_rate=0.2):
        super().__init__()
        self.embeddings = nn.ModuleDict({
            node_type: nn.Embedding(num_nodes, hidden_channels)
            for node_type, num_nodes in metadata
        })

        self.convs = nn.ModuleList([
            HeteroConv({
                edge_type: GraphConv(hidden_channels, hidden_channels)
                for edge_type in edge_types
            }, aggr='sum') for _ in range(3)
        ])

        self.batch_norms = nn.ModuleList([
            nn.BatchNorm1d(hidden_channels) for _ in range(3)
        ])

        self.dropout = nn.Dropout(p=dropout_rate)

        self.decoder = nn.Sequential(
            nn.Linear(hidden_channels * 2, hidden_channels),
            nn.ReLU(),
            nn.Linear(hidden_channels, 1)
        )

    def forward(self, data: HeteroData):
        device = next(self.parameters()).device
        x_dict = {
            node_type: self.embeddings[node_type](
                torch.arange(data[node_type].num_nodes, device=device)
            ) for node_type in data.node_types
        }

        edge_weight_dict = {
            edge_type: data[edge_type].edge_weight
            for edge_type in data.edge_types if 'edge_weight' in data[edge_type]
        }

        for i, conv in enumerate(self.convs):
            x_dict = conv(x_dict, data.edge_index_dict, edge_weight_dict=edge_weight_dict)
            for node_type in x_dict:
                x_dict[node_type] = self.batch_norms[i](x_dict[node_type])
                x_dict[node_type] = self.dropout(x_dict[node_type])

        return x_dict

    def predict(self, x_user, x_post):
        combined = torch.cat([x_user, x_post], dim=1)
        return self.decoder(combined).squeeze()


# === Extract Training Edges (likes + dislikes) ===
def get_training_edges(graph):
    device = (
        graph['user', 'likes', 'post'].edge_index.device if ('user', 'likes', 'post') in graph.edge_types else
        graph['user', 'dislikes', 'post'].edge_index.device if ('user', 'dislikes', 'post') in graph.edge_types else
        torch.device('cuda' if torch.cuda.is_available() else 'cpu')
    )

    user_post = []
    labels = []

    if ('user', 'likes', 'post') in graph.edge_types:
        likes = graph['user', 'likes', 'post']
        if likes.edge_index.size(1) > 0:
            user_post.append(likes.edge_index.t())
            labels.append(torch.ones(likes.edge_index.size(1), device=device))

    if ('user', 'dislikes', 'post') in graph.edge_types:
        dislikes = graph['user', 'dislikes', 'post']
        if dislikes.edge_index.size(1) > 0:
            user_post.append(dislikes.edge_index.t())
            labels.append(torch.zeros(dislikes.edge_index.size(1), device=device))

    if not user_post:
        raise ValueError("⚠️ No training edges (likes/dislikes) found.")

    all_edges = torch.cat(user_post, dim=0)
    all_labels = torch.cat(labels, dim=0)

    return all_edges, all_labels


# === Training Function ===
def train_model(data: HeteroData, model_name: str, epochs=150, hidden_channels=128, lr=0.001):
    device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
    data = data.to(device)

    # Move custom edge weights to device
    for edge_type in data.edge_types:
        if 'edge_weight' in data[edge_type]:
            data[edge_type].edge_weight = data[edge_type].edge_weight.to(device)

    metadata = [(k, v.num_nodes) for k, v in data.node_items()]
    model = GNNRecommender(hidden_channels, metadata, data.edge_types).to(device)

    optimizer = torch.optim.Adam(model.parameters(), lr=lr)
    criterion = nn.BCEWithLogitsLoss()

    edge_index, edge_labels = get_training_edges(data)
    loss_history = []

    print("👁️ Edge Types in Graph:", data.edge_types)
    print("👁️ Likes edge count:", data['user', 'likes', 'post'].edge_index.size(1) if ('user', 'likes', 'post') in data.edge_types else 'Not found')
    print("👁️ Dislikes edge count:", data['user', 'dislikes', 'post'].edge_index.size(1) if ('user', 'dislikes', 'post') in data.edge_types else 'Not found')

    for epoch in range(1, epochs + 1):
        model.train()
        optimizer.zero_grad()

        out = model(data)
        user_emb = out['user']
        post_emb = out['post']

        user_idx = edge_index[:, 0]
        post_idx = edge_index[:, 1]
        pred = model.predict(user_emb[user_idx], post_emb[post_idx])

        loss = criterion(pred, edge_labels)
        loss.backward()
        optimizer.step()

        loss_history.append(loss.item())
        print(f"[{model_name}] Epoch {epoch}/{epochs}, Loss: {loss.item():.4f}")

    # Plot training curve
    plt.plot(range(1, epochs + 1), loss_history, label=f'{model_name} Loss')
    plt.xlabel("Epoch")
    plt.ylabel("Loss")
    plt.title(f"Training Loss Curve ({model_name})")
    plt.grid(True)
    plt.legend()
    plt.tight_layout()
    plt.savefig(f"{model_name}_loss.png")
    plt.show()

    torch.save(model.state_dict(), f"{model_name}_model.pt")
    print(f"✅ Saved {model_name} model to {model_name}_model.pt")
    return model
