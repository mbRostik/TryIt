import torch
import numpy as np
import matplotlib.pyplot as plt
from torch_geometric.data import HeteroData
from GNN import GNNRecommender, get_training_edges

def evaluate_topk(model, data: HeteroData, K=10, batch_size=256):
    device = next(model.parameters()).device
    data = data.to(device)
    model.eval()

    user_ids = torch.arange(data['user'].num_nodes, device=device)
    post_ids = torch.arange(data['post'].num_nodes, device=device)

    out = model(data)
    user_emb = out['user']
    post_emb = out['post']

    true_matrix = torch.zeros((user_emb.size(0), post_emb.size(0)), device=device)
    if ('user', 'likes', 'post') in data.edge_types:
        edges = data['user', 'likes', 'post'].edge_index
        true_matrix[edges[0], edges[1]] = 1
    else:
        print("⚠️ No 'likes' edge found. Skipping evaluation.")
        return {}

    recalls, precisions, hits, ndcgs, maps = [], [], [], [], []

    for start in range(0, user_emb.size(0), batch_size):
        end = min(start + batch_size, user_emb.size(0))
        users_batch = user_emb[start:end]  # [B, D]

        # Compute scores [B, P]
        scores = model.predict(
            users_batch.unsqueeze(1).expand(-1, post_emb.size(0), -1).reshape(-1, users_batch.size(1)),
            post_emb.repeat(end - start, 1)
        ).view(end - start, post_emb.size(0))

        for i in range(scores.size(0)):
            user_idx = start + i
            gt_items = true_matrix[user_idx].nonzero(as_tuple=True)[0].tolist()
            if not gt_items:
                continue

            scores_u = scores[i].detach().cpu().numpy()
            gt_binary = np.zeros_like(scores_u)
            gt_binary[gt_items] = 1

            topk = np.argsort(-scores_u)[:K]
            hits_vec = gt_binary[topk]

            precisions.append(np.sum(hits_vec) / K)
            recalls.append(np.sum(hits_vec) / len(gt_items))
            hits.append(1.0 if np.sum(hits_vec) > 0 else 0.0)

            dcg = np.sum(hits_vec / np.log2(np.arange(2, 2 + len(hits_vec))))
            ideal_hits = sorted([1] * min(len(gt_items), K) + [0] * (K - min(len(gt_items), K)))
            idcg = np.sum(np.array(ideal_hits) / np.log2(np.arange(2, 2 + len(ideal_hits))))
            ndcgs.append(dcg / idcg if idcg > 0 else 0)

            num_hits, avg_prec = 0, 0.0
            for rank, item in enumerate(topk):
                if gt_binary[item]:
                    num_hits += 1
                    avg_prec += num_hits / (rank + 1)
            maps.append(avg_prec / min(len(gt_items), K))

    metrics = {
        "Recall@K": np.mean(recalls),
        "Precision@K": np.mean(precisions),
        "Hit@K": np.mean(hits),
        "NDCG@K": np.mean(ndcgs),
        "MAP@K": np.mean(maps)
    }

    return metrics


def train_model(data: HeteroData, model_name: str, epochs=100, hidden_channels=128, lr=0.005):
    K = 10
    device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
    metadata = [(k, v.num_nodes) for k, v in data.node_items()]
    model = GNNRecommender(hidden_channels, metadata, data.edge_types).to(device)
    optimizer = torch.optim.Adam(model.parameters(), lr=lr)
    criterion = torch.nn.BCEWithLogitsLoss()

    data = data.to(device)
    edge_index, edge_labels = get_training_edges(data)
    loss_history = []

    for epoch in range(epochs):
        model.train()
        optimizer.zero_grad()

        out = model(data)
        user_emb = out['user']
        post_emb = out['post']

        user_idx = edge_index[:, 0]
        post_idx = edge_index[:, 1]

        pred = model.predict(user_emb[user_idx], post_emb[post_idx])
        labels = edge_labels.to(device)

        loss = criterion(pred, labels)
        loss.backward()
        optimizer.step()

        print(f"[{model_name}] Epoch {epoch+1}/{epochs}, Loss: {loss.item():.4f}")
        loss_history.append(loss.item())

    # Plot training loss
    plt.figure()
    plt.plot(range(1, epochs + 1), loss_history, label=f'{model_name} Loss')
    plt.xlabel("Epoch")
    plt.ylabel("Loss")
    plt.title(f"Training Loss Curve ({model_name})")
    plt.grid(True)
    plt.tight_layout()
    plt.savefig(f"{model_name}_loss.png")
    plt.show()

    torch.save(model.state_dict(), f"{model_name}.pt")
    print(f"✅ Model '{model_name}' saved to disk.")

    metrics = evaluate_topk(model, data, K=10)
    if metrics:
        print(f"\n📊 Final Evaluation Metrics for {model_name}:")
        for k, v in metrics.items():
            print(f"  {k}: {v:.4f}")
            plt.figure(figsize=(8, 6))
            plt.bar(metrics.keys(), metrics.values())
            plt.title(f"{model_name} Evaluation Metrics @K={K}")
            plt.ylabel("Score")
            plt.ylim(0, 1)
            plt.grid(True, axis='y')
            plt.tight_layout()
            plt.savefig(f"{model_name}_metrics.png")
            plt.show()

    return model


if __name__ == '__main__':
    text_graph = torch.load('text_graph.pt')
    photo_graph = torch.load('photo_graph.pt')

    text_model = train_model(text_graph, model_name="text_recommender")
    photo_model = train_model(photo_graph, model_name="photo_recommender")
