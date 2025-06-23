from transformers import pipeline

# ✅ Load the classification pipeline
clf = pipeline(
    "text-classification",
    model="./saved_bert_dbpedia_model",
    tokenizer="./saved_bert_dbpedia_model",
    return_all_scores=True  # 🔑 Return scores for all classes
)

# ✅ Example texts
examples = [
    "Lionel Messi is a famous football player from Argentina.",
    "The Great Wall of China is one of the most iconic landmarks in the world.",
    "Nirvana released the album Nevermind in 1991.",
    "Photosynthesis is the process by which plants make food from sunlight."
]

# ✅ Show all categories with confidence > 0.01
for text in examples:
    print(f"\n📝 '{text}' → Predictions > 0.01:")
    predictions = clf(text)[0]
    for pred in sorted(predictions, key=lambda x: x["score"], reverse=True):
        if pred["score"] > 0.01:
            print(f"{pred['label']:<20} → {round(pred['score'], 3)}")
