from torch_geometric.data import HeteroData

data = torch.load("text_graph.pt")
visualize_hetero_graph(data, title="Text Graph")
