import os
import torch
import torch.nn as nn
import torch.optim as optim
from torchvision import datasets, models, transforms
from torch.utils.data import DataLoader, random_split
from sklearn.metrics import f1_score, precision_score, recall_score
from pathlib import Path
import matplotlib.pyplot as plt
import numpy as np

def main():
    # ✅ Paths
    DATA_DIR = Path(r"D:\C\Projects\AI_TryIt\Pictures_Categories\Data\imagenet_dataset")
    MODEL_SAVE_PATH = Path(r"D:\C\Projects\AI_TryIt\Pictures_Categories\AI_models_code\model_resnet18.pth")

    # ✅ Hyperparameters
    BATCH_SIZE = 32
    NUM_EPOCHS = 20
    IMG_SIZE = 224
    LEARNING_RATE = 0.001

    # ✅ Transforms
    transform = transforms.Compose([
        transforms.Resize((IMG_SIZE, IMG_SIZE)),
        transforms.RandomHorizontalFlip(),
        transforms.ToTensor(),
        transforms.Normalize([0.5]*3, [0.5]*3)
    ])

    # ✅ Load dataset
    full_dataset = datasets.ImageFolder(DATA_DIR, transform=transform)
    class_names = full_dataset.classes
    print(f"🧠 Found {len(class_names)} classes:", class_names)

    # ✅ Train/Val split
    train_size = int(0.8 * len(full_dataset))
    val_size = len(full_dataset) - train_size
    train_dataset, val_dataset = random_split(full_dataset, [train_size, val_size])

    train_loader = DataLoader(train_dataset, batch_size=BATCH_SIZE, shuffle=True, num_workers=2)
    val_loader = DataLoader(val_dataset, batch_size=BATCH_SIZE, shuffle=False, num_workers=2)

    # ✅ Use GPU if available
    device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    print(f"🚀 Using device: {device}")

    # ✅ Load pretrained ResNet and customize it
    model = models.resnet18(weights=models.ResNet18_Weights.DEFAULT)
    model.fc = nn.Linear(model.fc.in_features, len(class_names))
    model = model.to(device)

    # ✅ Loss and optimizer
    criterion = nn.CrossEntropyLoss()
    optimizer = optim.Adam(model.parameters(), lr=LEARNING_RATE)

    # ✅ Metric storage
    history = {
        "train_accuracy": [],
        "val_accuracy": [],
        "val_f1": [],
        "val_precision": [],
        "val_recall": []
    }

    # ✅ Training loop
    for epoch in range(NUM_EPOCHS):
        model.train()
        total_loss, total_correct = 0, 0
        for inputs, labels in train_loader:
            inputs, labels = inputs.to(device), labels.to(device)
            optimizer.zero_grad()
            outputs = model(inputs)
            loss = criterion(outputs, labels)
            loss.backward()
            optimizer.step()

            total_loss += loss.item()
            total_correct += (outputs.argmax(1) == labels).sum().item()

        train_accuracy = total_correct / len(train_dataset)

        # ✅ Validation phase
        model.eval()
        val_correct = 0
        all_preds = []
        all_labels = []
        with torch.no_grad():
            for inputs, labels in val_loader:
                inputs, labels = inputs.to(device), labels.to(device)
                outputs = model(inputs)
                preds = outputs.argmax(1)
                val_correct += (preds == labels).sum().item()

                all_preds.extend(preds.cpu().numpy())
                all_labels.extend(labels.cpu().numpy())

        val_accuracy = val_correct / len(val_dataset)
        val_f1 = f1_score(all_labels, all_preds, average='macro')
        val_precision = precision_score(all_labels, all_preds, average='macro', zero_division=0)
        val_recall = recall_score(all_labels, all_preds, average='macro', zero_division=0)

        # ✅ Store metrics
        history["train_accuracy"].append(train_accuracy)
        history["val_accuracy"].append(val_accuracy)
        history["val_f1"].append(val_f1)
        history["val_precision"].append(val_precision)
        history["val_recall"].append(val_recall)

        print(f"📊 Epoch {epoch+1}/{NUM_EPOCHS} - Train Acc: {train_accuracy:.4f} - Val Acc: {val_accuracy:.4f} - F1: {val_f1:.4f} - Precision: {val_precision:.4f} - Recall: {val_recall:.4f}")

    # ✅ Plot metrics
    epochs = list(range(1, NUM_EPOCHS + 1))
    plt.figure(figsize=(10, 6))
    plt.plot(epochs, history["train_accuracy"], label="Train Accuracy")
    plt.plot(epochs, history["val_accuracy"], label="Val Accuracy")
    plt.plot(epochs, history["val_f1"], label="F1 Score")
    plt.plot(epochs, history["val_precision"], label="Precision")
    plt.plot(epochs, history["val_recall"], label="Recall")
    plt.xlabel("Epoch")
    plt.ylabel("Score")
    plt.title("Model Training Metrics")
    plt.legend()
    plt.grid(True)
    plt.tight_layout()
    plt.savefig("training_metrics_graph.png")
    plt.show()

    # ✅ Save model
    torch.save(model.state_dict(), MODEL_SAVE_PATH)
    print(f"\n💾 Model saved to: {MODEL_SAVE_PATH}")

# ✅ Windows multiprocessing fix
if __name__ == "__main__":
    main()
