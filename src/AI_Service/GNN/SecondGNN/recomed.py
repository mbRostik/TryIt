import torch
from GNN import GNNRecommender

# === Load Graph and Model ===
def load_model_and_graph(model_path: str, graph_path: str):
    graph = torch.load(graph_path)
    metadata = [(k, v.num_nodes) for k, v in graph.node_items()]
    model = GNNRecommender(hidden_channels=128, metadata=metadata, edge_types=graph.edge_types)
    model.load_state_dict(torch.load(model_path))
    model.eval()
    return model, graph

# === Recommend Categories ===
def recommend_categories(user_id: str, model: GNNRecommender, graph, k=5):
    user_index = graph['user'].original_ids.index(user_id)
    user_emb = model(graph)['user'][user_index]
    post_emb = model(graph)['post']

    scores = torch.matmul(post_emb, user_emb)
    top_post_indices = torch.topk(scores, k).indices.tolist()

    def extract_categories(edge_index, post_indices, cat_original_ids):
        post_to_cat = {}
        for i in range(edge_index.size(1)):
            post_idx, cat_idx = edge_index[0, i].item(), edge_index[1, i].item()
            post_to_cat.setdefault(post_idx, []).append(cat_idx)

        categories = []
        for post_idx in top_post_indices:
            categories += post_to_cat.get(post_idx, [])
        return [cat_original_ids[i] for i in categories]

    # Extract category recommendations
    if 'text_category' in graph.node_types:
        edge = graph['post', 'has_text_category', 'text_category'].edge_index
        original_ids = graph['text_category'].original_ids
        text_cats = extract_categories(edge, top_post_indices, original_ids)
        print("📝 Recommended Text Categories:", text_cats)

    if 'photo_category' in graph.node_types:
        edge = graph['post', 'has_photo_category', 'photo_category'].edge_index
        original_ids = graph['photo_category'].original_ids
        photo_cats = extract_categories(edge, top_post_indices, original_ids)
        print("🖼️ Recommended Photo Categories:", photo_cats)

def recommend_posts(user_id: str, model: GNNRecommender, graph, k=5):
    user_index = graph['user'].original_ids.index(user_id)
    with torch.no_grad():
        embeddings = model(graph)
        user_emb = embeddings['user'][user_index]
        post_emb = embeddings['post']

        scores = torch.matmul(post_emb, user_emb)
        top_post_indices = torch.topk(scores, k).indices.tolist()

        top_post_ids = [graph['post'].original_ids[i] for i in top_post_indices]
        print("📬 Recommended Posts:", top_post_ids)

# === Main Entrypoint ===
if __name__ == '__main__':
    user_id = "25885eef-9c2b-4472-ba1a-0906a4216f26"

    text_model, text_graph = load_model_and_graph("text_recommender.pt", "text_graph.pt")
    print("\n=== TEXT POST RECOMMENDATIONS ===")
    recommend_posts(user_id, text_model, text_graph, k=5)

    photo_model, photo_graph = load_model_and_graph("photo_recommender.pt", "photo_graph.pt")
    print("\n=== PHOTO POST RECOMMENDATIONS ===")
    recommend_posts(user_id, photo_model, photo_graph, k=5)