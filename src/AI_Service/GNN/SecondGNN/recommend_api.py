from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List
import torch
import random
from GNN import GNNRecommender

app = FastAPI()

MODEL_PATH = r"D:\C\Projects\AI_TryIt\GNN\SecondGNN\text_recommender.pt"
GRAPH_PATH = r"D:\C\Projects\AI_TryIt\GNN\SecondGNN\text_graph.pt"

def load_model_and_graph(model_path, graph_path):
    graph = torch.load(graph_path)
    metadata = [(k, v.num_nodes) for k, v in graph.node_items()]
    model = GNNRecommender(hidden_channels=128, metadata=metadata, edge_types=graph.edge_types)
    model.load_state_dict(torch.load(model_path))
    model.eval()
    return model, graph

model, graph = load_model_and_graph(MODEL_PATH, GRAPH_PATH)

class RecommendationRequest(BaseModel):
    user_id: str
    exclude_post_ids: List[str] = []
    skip: int = 0
    limit: int = 20

@app.post("/recommend-posts")
def recommend_posts(req: RecommendationRequest):
    user_id = req.user_id
    all_post_ids = graph['post'].original_ids
    excluded_ids = set(req.exclude_post_ids)

    if user_id not in graph['user'].original_ids:
        user_index = graph['user'].num_nodes
        graph['user'].original_ids.append(user_id)
        graph['user'].num_nodes += 1
    else:
        user_index = graph['user'].original_ids.index(user_id)

    reacted = set()
    if ('user', 'likes', 'post') in graph.edge_types:
        ei = graph['user', 'likes', 'post'].edge_index
        reacted.update(ei[1][ei[0] == user_index].tolist())
    if ('user', 'dislikes', 'post') in graph.edge_types:
        ei = graph['user', 'dislikes', 'post'].edge_index
        reacted.update(ei[1][ei[0] == user_index].tolist())

    with torch.no_grad():
        emb = model(graph)
        user_emb = emb['user'][user_index]
        post_emb = emb['post']
        scores = torch.matmul(post_emb, user_emb)

    excluded_indices = reacted | {i for i, pid in enumerate(all_post_ids) if pid in excluded_ids}
    available_indices = [i for i in range(len(scores)) if i not in excluded_indices]

    if not reacted or not available_indices:
        candidates = [pid for pid in all_post_ids if pid not in excluded_ids]
        fallback = candidates[req.skip:req.skip + req.limit]
        return {"mode": "random", "user_id": user_id, "posts": fallback}

    sorted_indices = sorted(
        available_indices, key=lambda i: scores[i].item(), reverse=True
    )
    paginated_indices = sorted_indices[req.skip:req.skip + req.limit]
    recommended_ids = [all_post_ids[i] for i in paginated_indices]

    return {"mode": "recommended", "user_id": user_id, "posts": recommended_ids}
