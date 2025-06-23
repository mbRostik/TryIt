import os
import torch
from torchvision import models, transforms
from PIL import Image
from pathlib import Path

MODEL_PATH = Path(r"D:\C\Projects\AI_TryIt\Pictures_Categories\AI_models_code\model_resnet18.pth")
CLASS_FOLDER = Path(r"D:\C\Projects\AI_TryIt\Pictures_Categories\Data\imagenet_dataset\airplane")
TRAIN_DATASET = Path(r"D:\C\Projects\AI_TryIt\Pictures_Categories\Data\imagenet_dataset")
IMG_SIZE = 224

# === Load class names from training folder structure ===
from torchvision.datasets import ImageFolder
_ = ImageFolder(TRAIN_DATASET)  # just to get class_to_idx
class_names = _.classes
print(f"✅ Loaded {len(class_names)} classes.")

# === Define preprocessing (same as training) ===
transform = transforms.Compose([
    transforms.Resize((IMG_SIZE, IMG_SIZE)),
    transforms.ToTensor(),
    transforms.Normalize([0.5]*3, [0.5]*3)
])

# === Load the model ===
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
print(f"🚀 Using device: {device}")

model = models.resnet18(weights=None)
model.fc = torch.nn.Linear(model.fc.in_features, len(class_names))
model.load_state_dict(torch.load(MODEL_PATH, map_location=device))
model = model.to(device)
model.eval()

# === Predict function ===
def predict_image(img_path: Path):
    try:
        image = Image.open(img_path).convert("RGB")
        input_tensor = transform(image).unsqueeze(0).to(device)
        with torch.no_grad():
            outputs = model(input_tensor)
            _, predicted = torch.max(outputs, 1)
            predicted_label = class_names[predicted.item()]
        return predicted_label
    except Exception as e:
        return f"❌ Error: {e}"

# === Run prediction on all images in folder ===
image_paths = list(CLASS_FOLDER.glob("*.jpg")) + list(CLASS_FOLDER.glob("*.jpeg")) + list(CLASS_FOLDER.glob("*.png"))
print(f"🔍 Found {len(image_paths)} images in: {CLASS_FOLDER}")

for path in image_paths:
    label = predict_image(path)
