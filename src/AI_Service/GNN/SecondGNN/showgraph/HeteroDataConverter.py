import torch
import networkx as nx
import matplotlib.pyplot as plt
from torch_geometric.data import HeteroData


def visualize_hetero_graph(data: HeteroData, title="Heterogeneous Graph"):
    G = nx.MultiDiGraph()

    # Додаємо вузли
    for node_type in data.node_types:
        num_nodes = data[node_type].num_nodes
        G.add_nodes_from([(f"{node_type}_{i}", {"type": node_type}) for i in range(num_nodes)])

    # Додаємо ребра
    for edge_type in data.edge_types:
        src_type, rel_type, dst_type = edge_type
        edge_index = data[edge_type].edge_index
        for src, dst in zip(edge_index[0].tolist(), edge_index[1].tolist()):
            G.add_edge(f"{src_type}_{src}", f"{dst_type}_{dst}", label=rel_type)

    # Позиціювання вузлів автоматично
    pos = nx.spring_layout(G, seed=42, k=0.5)

    # Малюємо вузли
    node_colors = {'user': 'lightblue', 'post': 'orange', 'text_category': 'green', 'photo_category': 'pink'}
    node_colors_list = [node_colors[G.nodes[n]["type"]] for n in G.nodes]

    nx.draw_networkx_nodes(G, pos, node_size=400, node_color=node_colors_list)
    nx.draw_networkx_labels(G, pos, font_size=7)

    # Малюємо ребра з мітками типів
    nx.draw_networkx_edges(G, pos, arrows=True, width=1.0, alpha=0.6)
    edge_labels = {(u, v): d['label'] for u, v, d in G.edges(data=True)}
    nx.draw_networkx_edge_labels(G, pos, edge_labels=edge_labels, font_size=6)

    plt.title(title)
    plt.axis('off')
    plt.tight_layout()
    plt.show()

data = torch.load("D:\\C\\Projects\\AI_TryIt\\GNN\\SecondGNN\\text_graph.pt")

visualize_hetero_graph(data, title="Text Graph")
