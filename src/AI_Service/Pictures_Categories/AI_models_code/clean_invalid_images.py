from pathlib import Path
from PIL import Image, UnidentifiedImageError
import os

# Set your image dataset root
DATASET_DIR = Path(r"D:\C\Projects\AI_TryIt\Pictures_Categories\Data\imagenet_dataset")

deleted_count = 0
for img_path in DATASET_DIR.rglob("*.*"):
    if img_path.suffix.lower() not in [".jpg", ".jpeg", ".png", ".bmp", ".tiff"]:
        continue

    try:
        if img_path.stat().st_size == 0:
            raise OSError("Empty file")

        # Try opening with PIL
        with Image.open(img_path) as img:
            img.verify()  # does not load data, just validates format

    except (UnidentifiedImageError, OSError) as e:
        print(f"❌ Deleting corrupted: {img_path} — {e}")
        img_path.unlink()
        deleted_count += 1

print(f"\n✅ Done. Deleted {deleted_count} invalid image(s).")
