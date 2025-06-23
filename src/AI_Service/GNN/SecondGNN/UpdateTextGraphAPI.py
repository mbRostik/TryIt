from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List
import torch
from torch_geometric.data import HeteroData
import os

app = FastAPI()

GRAPH_PATH = r"D:\C\Projects\AI_TryIt\GNN\SecondGNN\text_graph.pt"

def load_graph() -> HeteroData:
    if not os.path.exists(GRAPH_PATH):
        data = HeteroData()
        data['user'].num_nodes = 0
        data['user'].original_ids = []
        data['post'].num_nodes = 0
        data['post'].original_ids = []
        data['text_category'].num_nodes = 0
        data['text_category'].original_ids = []
        return data
    return torch.load(GRAPH_PATH)

def save_graph(data: HeteroData):
    torch.save(data, GRAPH_PATH)

graph = load_graph()

class GraphUpdateRequest(BaseModel):
    user_id: str
    post_id: str
    text_categories: List[str]

@app.post("/update-graph")
def update_graph(req: GraphUpdateRequest):
    global graph

    print(f"📥 Received update request for user: {req.user_id}, post: {req.post_id}")
    print(f"📝 Text Categories: {req.text_categories}")

    user_map = {uid: i for i, uid in enumerate(graph['user'].original_ids)}
    post_map = {pid: i for i, pid in enumerate(graph['post'].original_ids)}
    cat_map = {cid: i for i, cid in enumerate(graph['text_category'].original_ids)}

    if req.user_id not in user_map:
        user_index = graph['user'].num_nodes
        user_map[req.user_id] = user_index
        graph['user'].original_ids.append(req.user_id)
        graph['user'].num_nodes += 1
        print(f"➕ Added new user: {req.user_id}")
    else:
        user_index = user_map[req.user_id]
        print(f"✅ User already exists: {req.user_id}")

    if req.post_id not in post_map:
        post_index = graph['post'].num_nodes
        post_map[req.post_id] = post_index
        graph['post'].original_ids.append(req.post_id)
        graph['post'].num_nodes += 1
        print(f"➕ Added new post: {req.post_id}")
    else:
        post_index = post_map[req.post_id]
        print(f"✅ Post already exists: {req.post_id}")

    for cat in req.text_categories:
        if cat not in cat_map:
            cat_index = graph['text_category'].num_nodes
            cat_map[cat] = cat_index
            graph['text_category'].original_ids.append(cat)
            graph['text_category'].num_nodes += 1
            print(f"➕ Added new category: {cat}")
        else:
            print(f"✅ Category already exists: {cat}")

    for cat in req.text_categories:
        post_idx = post_map[req.post_id]
        cat_idx = cat_map[cat]

        edge = torch.tensor([[post_idx], [cat_idx]], dtype=torch.long)
        edge_rev = torch.tensor([[cat_idx], [post_idx]], dtype=torch.long)
        weight = torch.tensor([1.0], dtype=torch.float)

        def append_edge(edge_type, ei, ew):
            if edge_type in graph.edge_types:
                graph[edge_type].edge_index = torch.cat([graph[edge_type].edge_index, ei], dim=1)
                graph[edge_type].edge_weight = torch.cat([graph[edge_type].edge_weight, ew])
                print(f"🔄 Appended edge to existing type: {edge_type}")
            else:
                graph[edge_type].edge_index = ei
                graph[edge_type].edge_weight = ew
                print(f"🆕 Created new edge type: {edge_type}")

        already_exists = False
        if ('post', 'has_text_category', 'text_category') in graph.edge_types:
            existing_ei = graph['post', 'has_text_category', 'text_category'].edge_index
            exists_mask = (existing_ei[0] == post_idx) & (existing_ei[1] == cat_idx)
            if torch.any(exists_mask):
                already_exists = True
                print(f"⚠️ Edge already exists: ({post_idx}, {cat_idx})")

        if not already_exists:
            append_edge(('post', 'has_text_category', 'text_category'), edge, weight)
            append_edge(('text_category', 'belongs_to_post', 'post'), edge_rev, weight)

    save_graph(graph)
    print("💾 Graph saved successfully.")
    return {"status": "success", "message": "Graph updated and saved successfully."}



class PostReactionRequest(BaseModel):
    user_id: str
    post_id: str
    reaction: bool  # True = like, False = dislike

@app.post("/react-to-post")
def react_to_post(req: PostReactionRequest):
    global graph

    user_map = {uid: i for i, uid in enumerate(graph['user'].original_ids)}
    post_map = {pid: i for i, pid in enumerate(graph['post'].original_ids)}

    # Add user if not present
    if req.user_id not in user_map:
        user_index = graph['user'].num_nodes
        user_map[req.user_id] = user_index
        graph['user'].original_ids.append(req.user_id)
        graph['user'].num_nodes += 1
    else:
        user_index = user_map[req.user_id]

    # Add post if not present
    if req.post_id not in post_map:
        post_index = graph['post'].num_nodes
        post_map[req.post_id] = post_index
        graph['post'].original_ids.append(req.post_id)
        graph['post'].num_nodes += 1
    else:
        post_index = post_map[req.post_id]

    # Determine edge types based on reaction
    if req.reaction:
        user_to_post_type = ('user', 'likes', 'post')
        post_to_user_type = ('post', 'liked_by', 'user')
    else:
        user_to_post_type = ('user', 'dislikes', 'post')
        post_to_user_type = ('post', 'disliked_by', 'user')

    # Prepare edge and reverse edge
    edge = torch.tensor([[user_index], [post_index]], dtype=torch.long)
    edge_rev = torch.tensor([[post_index], [user_index]], dtype=torch.long)
    weight = torch.tensor([1.0], dtype=torch.float)

    def append_or_increment(edge_type, ei, ew):
        if edge_type not in graph.edge_types:
            graph[edge_type].edge_index = ei
            graph[edge_type].edge_weight = ew
            return

        existing_ei = graph[edge_type].edge_index
        existing_ew = graph[edge_type].edge_weight
        matches = (existing_ei[0] == ei[0, 0]) & (existing_ei[1] == ei[1, 0])

        if torch.any(matches):
            idx = torch.nonzero(matches).item()
            existing_ew[idx] += 1.0
        else:
            graph[edge_type].edge_index = torch.cat([existing_ei, ei], dim=1)
            graph[edge_type].edge_weight = torch.cat([existing_ew, ew])

    # Apply updates
    append_or_increment(user_to_post_type, edge, weight)
    append_or_increment(post_to_user_type, edge_rev, weight)

    save_graph(graph)
    return {
        "status": "success",
        "message": f"Reaction ({'like' if req.reaction else 'dislike'}) from {req.user_id} to {req.post_id} recorded."
    }