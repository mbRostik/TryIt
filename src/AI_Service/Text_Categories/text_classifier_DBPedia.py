import numpy as np
import json
import matplotlib.pyplot as plt
from datasets import load_dataset
from transformers import (
    AutoTokenizer,
    AutoModelForSequenceClassification,
    Trainer,
    TrainingArguments,
    DataCollatorWithPadding,
    EarlyStoppingCallback
)
from sklearn.metrics import accuracy_score, f1_score, precision_score, recall_score

MODEL_NAME = "bert-base-uncased"
SAVE_DIR = "./saved_bert_dbpedia_model"

print("📥 Loading DBPedia 14 dataset...")
dataset = load_dataset("dbpedia_14")
dataset["train"] = dataset["train"].shuffle(seed=42).select(range(20000))
dataset["test"] = dataset["test"].shuffle(seed=42).select(range(5000))

label_names = dataset["train"].features["label"].names
label2id = {label: i for i, label in enumerate(label_names)}
id2label = {i: label for i, label in enumerate(label_names)}
num_labels = len(label_names)

tokenizer = AutoTokenizer.from_pretrained(MODEL_NAME)

def preprocess(example):
    return tokenizer(example["content"], truncation=True)

tokenized = dataset.map(preprocess, batched=True)
train_data = tokenized["train"]
val_data = tokenized["test"]

model = AutoModelForSequenceClassification.from_pretrained(
    MODEL_NAME,
    num_labels=num_labels,
    id2label=id2label,
    label2id=label2id
)

# Store metrics for plotting
training_metrics = {
    "epoch": [],
    "accuracy": [],
    "f1_macro": [],
    "precision_macro": [],
    "recall_macro": []
}

def compute_metrics(eval_pred):
    logits, labels = eval_pred
    preds = np.argmax(logits, axis=1)
    accuracy = accuracy_score(labels, preds)
    f1_macro = f1_score(labels, preds, average="macro")
    precision_macro = precision_score(labels, preds, average="macro")
    recall_macro = recall_score(labels, preds, average="macro")
    
    # Store
    training_metrics["epoch"].append(len(training_metrics["epoch"]) + 1)
    training_metrics["accuracy"].append(accuracy)
    training_metrics["f1_macro"].append(f1_macro)
    training_metrics["precision_macro"].append(precision_macro)
    training_metrics["recall_macro"].append(recall_macro)
    
    return {
        "accuracy": accuracy,
        "f1_macro": f1_macro,
        "precision_macro": precision_macro,
        "recall_macro": recall_macro
    }

training_args = TrainingArguments(
    output_dir="./results",
    evaluation_strategy="epoch",
    save_strategy="epoch",
    learning_rate=2e-5,
    per_device_train_batch_size=8,
    per_device_eval_batch_size=8,
    num_train_epochs=5,
    weight_decay=0.01,
    load_best_model_at_end=True,
    metric_for_best_model="accuracy",
    logging_dir="./logs",
    logging_strategy="epoch",
    save_total_limit=2,
    report_to="none"
)

trainer = Trainer(
    model=model,
    args=training_args,
    train_dataset=train_data,
    eval_dataset=val_data,
    tokenizer=tokenizer,
    data_collator=DataCollatorWithPadding(tokenizer),
    compute_metrics=compute_metrics,
    callbacks=[EarlyStoppingCallback(early_stopping_patience=2)]
)

print("🚀 Training model...")
trainer.train()

# 📊 Plot metrics
plt.figure(figsize=(10, 6))
for metric in ["accuracy", "f1_macro", "precision_macro", "recall_macro"]:
    plt.plot(training_metrics["epoch"], training_metrics[metric], label=metric)
plt.xlabel("Epoch")
plt.ylabel("Score")
plt.title("Model Evaluation Metrics per Epoch")
plt.legend()
plt.grid(True)
plt.tight_layout()
plt.savefig("training_metrics_plot.png")
plt.show()

# Final evaluation
print("📊 Final evaluation:")
print(trainer.evaluate())

print(f"💾 Saving model to: {SAVE_DIR}")
trainer.save_model(SAVE_DIR)
tokenizer.save_pretrained(SAVE_DIR)
with open(f"{SAVE_DIR}/label_mappings.json", "w") as f:
    json.dump({"label2id": label2id, "id2label": id2label}, f)

print("✅ Done.")
