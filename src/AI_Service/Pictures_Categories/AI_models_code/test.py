import torch
from torchvision import models, transforms
from torchvision.datasets import ImageFolder
from PIL import Image
import os

# === CONFIG ===
MODEL_PATH = r"D:\C\Projects\AI_TryIt\Pictures_Categories\AI_models_code\model_resnet18.pth"
CLASS_NAMES_PATH = r"D:\C\Projects\AI_TryIt\Pictures_Categories\Data\imagenet_dataset"
IMAGES_TO_TEST = [
    r"D:\C\Projects\AI_TryIt\Pictures_Categories\Data\imagenet_dataset\canoe\n02951358_6.JPEG",
    r"D:\C\Projects\AI_TryIt\Pictures_Categories\Data\imagenet_dataset\limousine\n03670208_15.JPEG",
    r"D:\C\Projects\AI_TryIt\Pictures_Categories\Data\imagenet_dataset\organ\n03854065_355.JPEG",
]

# === Transforms (must match training)
transform = transforms.Compose([
    transforms.Resize((224, 224)),
    transforms.ToTensor(),
    transforms.Normalize([0.5]*3, [0.5]*3)
])

# === Load class names
class_names = ImageFolder(CLASS_NAMES_PATH).classes

# === Load model
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
print(f"🚀 Inference will run on: {device}")

model = models.resnet18(weights=None)
model.fc = torch.nn.Linear(model.fc.in_features, len(class_names))
model.load_state_dict(torch.load(MODEL_PATH, map_location=device))
model = model.to(device)
model.eval()

# === Predict and show class probabilities > 0.01
for image_path in IMAGES_TO_TEST:
    image = Image.open(image_path).convert("RGB")
    input_tensor = transform(image).unsqueeze(0).to(device)

    with torch.no_grad():
        outputs = model(input_tensor)
        probs = torch.nn.functional.softmax(outputs[0], dim=0)

    print(f"\n🖼️ Predictions for: {os.path.basename(image_path)}")
    sorted_probs = sorted(enumerate(probs), key=lambda x: x[1], reverse=True)
    for idx, prob in sorted_probs:
        if prob.item() > 0.01:
            print(f"{class_names[idx]:<20} → {prob.item():.2f}")
