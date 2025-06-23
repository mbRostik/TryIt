import torch

TEXT_GRAPH_PATH = 'text_graph.pt'
PHOTO_GRAPH_PATH = 'photo_graph.pt'

def print_high_weight_edges(graph, edge_type, original_ids=None, min_weight=3.0, top_k=10):
    edge_index = graph[edge_type].edge_index
    edge_weight = graph[edge_type].edge_weight

    if edge_weight is None or edge_weight.numel() == 0:
        print(f"No weights found for edge type {edge_type}")
        return

    mask = edge_weight >= min_weight
    high_weight_edges = edge_index[:, mask]
    high_weights = edge_weight[mask]

    print(f"\n🔎 Top {top_k} edges in {edge_type} with weight ≥ {min_weight}:")
    for i in range(min(top_k, high_weights.size(0))):
        src_idx = high_weight_edges[0, i].item()
        dst_idx = high_weight_edges[1, i].item()
        weight = high_weights[i].item()

        if original_ids:
            src = original_ids['user'][src_idx] if 'user' in edge_type[0] else src_idx
            dst = original_ids['post'][dst_idx] if 'post' in edge_type[2] else dst_idx
        else:
            src, dst = src_idx, dst_idx

        print(f"  {edge_type[0]} {src} → {edge_type[2]} {dst} | weight: {weight}")

def load_original_ids(graph):
    return {
        node_type: graph[node_type].original_ids
        for node_type in graph.node_types if hasattr(graph[node_type], 'original_ids')
    }

def main():
    print("📂 Loading and inspecting text_graph.pt...")
    text_graph = torch.load(TEXT_GRAPH_PATH)
    original_ids_text = load_original_ids(text_graph)
    print_high_weight_edges(text_graph, ('user', 'likes', 'post'), original_ids_text, min_weight=3.0)

    print("\n📂 Loading and inspecting photo_graph.pt...")
    photo_graph = torch.load(PHOTO_GRAPH_PATH)
    original_ids_photo = load_original_ids(photo_graph)
    print_high_weight_edges(photo_graph, ('user', 'likes', 'post'), original_ids_photo, min_weight=3.0)

if __name__ == '__main__':
    main()
