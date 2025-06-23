import pyodbc
import pandas as pd
import torch
from torch_geometric.data import HeteroData

# === Configuration ===
DB_CONFIG = {
    'server': 'ROSTIKPC\\SQLEXPRESS',
    'database': 'Posts',
    'driver': 'ODBC Driver 17 for SQL Server'
}

TEXT_GRAPH_PATH = 'text_graph.pt'
PHOTO_GRAPH_PATH = 'photo_graph.pt'

# === Step 1: Load data from SQL Server ===
def load_data():
    conn_str = (
        f"DRIVER={{{DB_CONFIG['driver']}}};"
        f"SERVER={DB_CONFIG['server']};"
        f"DATABASE={DB_CONFIG['database']};"
        f"Trusted_Connection=yes;"
        f"Encrypt=no;"
    )
    conn = pyodbc.connect(conn_str)

    users = pd.read_sql("SELECT DISTINCT u.Id FROM Users u", conn)
    posts = pd.read_sql("SELECT Id FROM Posts", conn)
    reactions = pd.read_sql("SELECT UserId, PostId, Reaction FROM PostReactionts", conn)
    text_cats = pd.read_sql("SELECT PostId, PostTextCategoryId FROM PostsWithTextCategories", conn)
    photo_cats = pd.read_sql("SELECT PostId, PostPhotoCategoryId FROM PostsWithPhotoCategories", conn)

    conn.close()
    return users, posts, reactions, text_cats, photo_cats

# === Step 2a: Build Text Graph ===
def build_text_graph(users, posts, reactions, text_cats):
    data = HeteroData()
    user_ids = users['Id'].astype(str).tolist()
    post_ids = posts['Id'].astype(str).tolist()
    user_map = {uid: i for i, uid in enumerate(user_ids)}
    post_map = {pid: i for i, pid in enumerate(post_ids)}

    data['user'].num_nodes = len(user_ids)
    data['user'].original_ids = user_ids
    data['post'].num_nodes = len(post_ids)
    data['post'].original_ids = post_ids

    filtered_reactions = reactions.groupby(['UserId', 'PostId', 'Reaction']).size().reset_index(name='weight')
    filtered_reactions = filtered_reactions[filtered_reactions['weight'] >= 1]

    likes = filtered_reactions[filtered_reactions['Reaction'] == True]
    dislikes = filtered_reactions[filtered_reactions['Reaction'] == False]

    for (name, df) in [('likes', likes), ('dislikes', dislikes)]:
        edge_index = torch.tensor([
            [user_map[str(u)] for u in df['UserId']],
            [post_map[str(p)] for p in df['PostId']]
        ], dtype=torch.long)
        weights = torch.tensor(df['weight'].tolist(), dtype=torch.float)

        data['user', name, 'post'].edge_index = edge_index
        data['user', name, 'post'].edge_weight = weights
        data['post', f'{name}_by', 'user'].edge_index = edge_index[[1, 0]]
        data['post', f'{name}_by', 'user'].edge_weight = weights

    text_cat_ids = text_cats['PostTextCategoryId'].astype(str).unique().tolist()
    text_cat_map = {cid: i for i, cid in enumerate(text_cat_ids)}
    data['text_category'].num_nodes = len(text_cat_ids)
    data['text_category'].original_ids = text_cat_ids

    edge_index_text = torch.tensor([
        [post_map[str(pid)] for pid in text_cats['PostId']],
        [text_cat_map[str(cid)] for cid in text_cats['PostTextCategoryId']]
    ], dtype=torch.long)
    edge_weight_text = torch.ones(edge_index_text.shape[1], dtype=torch.float)

    data['post', 'has_text_category', 'text_category'].edge_index = edge_index_text
    data['post', 'has_text_category', 'text_category'].edge_weight = edge_weight_text
    data['text_category', 'belongs_to_post', 'post'].edge_index = edge_index_text[[1, 0]]
    data['text_category', 'belongs_to_post', 'post'].edge_weight = edge_weight_text

    return data

# === Step 2b: Build Photo Graph ===
def build_photo_graph(users, posts, reactions, photo_cats):
    data = HeteroData()
    user_ids = users['Id'].astype(str).tolist()
    post_ids = posts['Id'].astype(str).tolist()
    user_map = {uid: i for i, uid in enumerate(user_ids)}
    post_map = {pid: i for i, pid in enumerate(post_ids)}

    data['user'].num_nodes = len(user_ids)
    data['user'].original_ids = user_ids
    data['post'].num_nodes = len(post_ids)
    data['post'].original_ids = post_ids

    filtered_reactions = reactions.groupby(['UserId', 'PostId', 'Reaction']).size().reset_index(name='weight')
    filtered_reactions = filtered_reactions[filtered_reactions['weight'] >= 1]

    likes = filtered_reactions[filtered_reactions['Reaction'] == True]
    dislikes = filtered_reactions[filtered_reactions['Reaction'] == False]

    for (name, df) in [('likes', likes), ('dislikes', dislikes)]:
        edge_index = torch.tensor([
            [user_map[str(u)] for u in df['UserId']],
            [post_map[str(p)] for p in df['PostId']]
        ], dtype=torch.long)
        weights = torch.tensor(df['weight'].tolist(), dtype=torch.float)

        data['user', name, 'post'].edge_index = edge_index
        data['user', name, 'post'].edge_weight = weights
        data['post', f'{name}_by', 'user'].edge_index = edge_index[[1, 0]]
        data['post', f'{name}_by', 'user'].edge_weight = weights

    photo_cat_ids = photo_cats['PostPhotoCategoryId'].astype(str).unique().tolist()
    photo_cat_map = {cid: i for i, cid in enumerate(photo_cat_ids)}
    data['photo_category'].num_nodes = len(photo_cat_ids)
    data['photo_category'].original_ids = photo_cat_ids

    edge_index_photo = torch.tensor([
        [post_map[str(pid)] for pid in photo_cats['PostId']],
        [photo_cat_map[str(cid)] for cid in photo_cats['PostPhotoCategoryId']]
    ], dtype=torch.long)
    edge_weight_photo = torch.ones(edge_index_photo.shape[1], dtype=torch.float)

    data['post', 'has_photo_category', 'photo_category'].edge_index = edge_index_photo
    data['post', 'has_photo_category', 'photo_category'].edge_weight = edge_weight_photo
    data['photo_category', 'belongs_to_post', 'post'].edge_index = edge_index_photo[[1, 0]]
    data['photo_category', 'belongs_to_post', 'post'].edge_weight = edge_weight_photo

    return data

# === Save graph to file ===
def save_graph(data, path):
    torch.save(data, path)

# === Main ===
def main():
    users, posts, reactions, text_cats, photo_cats = load_data()
    print(f"👥 Users in reactions: {len(users)}")
    print(f"📝 Posts: {len(posts)}")
    print(f"👍 Reactions: {len(reactions)}")
    print(f"🏷️ Text categories: {len(text_cats)}")
    print(f"🖼️ Photo categories: {len(photo_cats)}")
    
    text_graph = build_text_graph(users, posts, reactions, text_cats)
    photo_graph = build_photo_graph(users, posts, reactions, photo_cats)
    save_graph(text_graph, TEXT_GRAPH_PATH)
    save_graph(photo_graph, PHOTO_GRAPH_PATH)
    print("✅ Saved text_graph.pt and photo_graph.pt")
    
def validate_graph(graph, name):
    print(f"📊 Validation Report for {name}:")
    for edge_type in graph.edge_types:
        ei = graph[edge_type].edge_index
        ew = graph[edge_type].edge_weight
        print(f"  {edge_type}:")
        print(f"    Edge count: {ei.size(1)}")
        print(f"    Weights present: {'Yes' if ew is not None else 'No'}")
        if ew is not None and ew.numel() > 0:
            print(f"    Unique weights: {ew.unique().tolist()}")
            print(f"    NaNs in weights: {torch.isnan(ew).any().item()}")
            print(f"    Min weight: {ew.min().item()}")
            print(f"    Max weight: {ew.max().item()}")
        else:
            print("    Edge weights: None or empty")

if __name__ == '__main__':
    main()
    validate_graph(torch.load(TEXT_GRAPH_PATH), "Text Graph")
    validate_graph(torch.load(PHOTO_GRAPH_PATH), "Photo Graph")
