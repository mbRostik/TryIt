
# === uvicorn webApi:app --reload ===

from pydantic import BaseModel
from fastapi import FastAPI, File, UploadFile, Form
from typing import List
from transformers import pipeline
import torch
from torchvision import models, transforms
from PIL import Image
import os
import io
import nltk
from nltk.tokenize import sent_tokenize
nltk.download("punkt")
nltk.download("punkt_tab") 
app = FastAPI()

class TextRequest(BaseModel):
    text: str

# === TEXT CLASSIFIER CONFIG ===
TEXT_MODEL_PATH = r"D:\C\Projects\AI_TryIt\Text_Categories\saved_bert_dbpedia_model"
text_classifier = pipeline(
    "text-classification",
    model=TEXT_MODEL_PATH,
    tokenizer=TEXT_MODEL_PATH,
    top_k=None
)

# === IMAGE CLASSIFIER CONFIG ===
CLASS_NAMES_PATH = r"D:\C\Projects\AI_TryIt\Pictures_Categories\Data\imagenet_dataset"
MODEL_PATH = r"D:\C\Projects\AI_TryIt\Pictures_Categories\AI_models_code\model_resnet18.pth"

class_names = sorted(os.listdir(CLASS_NAMES_PATH))

image_transform = transforms.Compose([
    transforms.Resize((224, 224)),
    transforms.ToTensor(),
    transforms.Normalize([0.5] * 3, [0.5] * 3)
])

device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
print(f"🚀 Inference will run on: {device}")

image_model = models.resnet18(weights=None)
image_model.fc = torch.nn.Linear(image_model.fc.in_features, len(class_names))
image_model.load_state_dict(torch.load(MODEL_PATH, map_location=device))
image_model = image_model.to(device)
image_model.eval()


# === TEXT ENDPOINT ===
@app.post("/predict_category")
def predict_category(request: TextRequest):
    sentences = sent_tokenize(request.text)
    all_predictions = []

    for sentence in sentences:
        scores = text_classifier(sentence)[0]
        filtered = [
            pred["label"]
            for pred in scores if pred["score"] > 0.2
        ]
        all_predictions.extend(filtered)

    return {"predicted_categories": all_predictions}

# === IMAGE ENDPOINT ===

@app.post("/predict_photo_category")
def predict_images(files: List[UploadFile] = File(...)):
    labels = []

    for file in files:
        img_bytes = file.file.read()
        image = Image.open(io.BytesIO(img_bytes)).convert("RGB")
        input_tensor = image_transform(image).unsqueeze(0).to(device)

        with torch.no_grad():
            outputs = image_model(input_tensor)
            probs = torch.nn.functional.softmax(outputs[0], dim=0)

        filtered_labels = [
            class_names[i]
            for i, prob in enumerate(probs)
            if prob.item() > 0.25
        ]

        labels.extend(filtered_labels)

    return {"predicted_categories": labels}




@app.post("/v1-redict-images")
def predict_images(files: List[UploadFile] = File(...)):
    results = []

    for file in files:
        img_bytes = file.file.read()
        image = Image.open(io.BytesIO(img_bytes)).convert("RGB")
        input_tensor = image_transform(image).unsqueeze(0).to(device)

        with torch.no_grad():
            outputs = image_model(input_tensor)
            probs = torch.nn.functional.softmax(outputs[0], dim=0)

        filtered = [
            {"label": class_names[i], "confidence": round(prob.item(), 3)}
            for i, prob in enumerate(probs) if prob.item() > 0.25
        ]

        results.append({
            "filename": file.filename,
            "predictions": filtered
        })

    return {"results": results}