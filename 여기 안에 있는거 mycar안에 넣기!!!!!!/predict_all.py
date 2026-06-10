#!/usr/bin/env python3
# predict_all.py (auto-batch, index 포함, stderr 진행 로그)
import sys
import os
import json
import numpy as np
from tensorflow.keras.models import load_model
from tensorflow.keras.preprocessing.image import load_img, img_to_array

def safe_load_image(path, target_size=(120, 160)):
    try:
        img = load_img(path, target_size=target_size)
        return img_to_array(img) / 255.0
    except Exception:
        return np.zeros((target_size[0], target_size[1], 3), dtype=np.float32)

def find_catalog_files(tub_path):
    files = []
    for root, dirs, filenames in os.walk(tub_path):
        # backup 폴더는 건너뜀
        if f"{os.sep}backup{os.sep}" in root or root.endswith(f"{os.sep}backup"):
            continue
        for fn in filenames:
            if fn.endswith('.catalog'):
                files.append(os.path.join(root, fn))
    return files

def load_all_records(catalog_files):
    records = []
    for cat_file in catalog_files:
        try:
            with open(cat_file, 'r', encoding='utf-8') as f:
                for line in f:
                    if not line.strip():
                        continue
                    try:
                        records.append(json.loads(line))
                    except json.JSONDecodeError:
                        continue
        except Exception:
            continue
    return records

def build_image_candidates(tub_path, rel_path):
    images_folder = os.path.join(tub_path, "images")
    candidates = [
        os.path.join(tub_path, rel_path),
        os.path.join(images_folder, rel_path),
        os.path.join(images_folder, os.path.basename(rel_path))
    ]
    for cand in candidates:
        if os.path.exists(cand):
            return cand
    return None

def _get_mem_available_bytes():
    try:
        with open("/proc/meminfo", "r") as f:
            for line in f:
                if line.startswith("MemAvailable:"):
                    parts = line.split()
                    return int(parts[1]) * 1024
    except Exception:
        pass
    try:
        with open("/proc/meminfo", "r") as f:
            for line in f:
                if line.startswith("MemTotal:"):
                    parts = line.split()
                    return int(parts[1]) * 1024 // 2
    except Exception:
        pass
    return 1_000_000_000

def choose_batch_size(arg_batch):
    if arg_batch is not None:
        try:
            return max(1, int(arg_batch))
        except Exception:
            return 32
    # 자동 추정
    avail = _get_mem_available_bytes()
    per_image_bytes = 120 * 160 * 3 * 4  # float32
    overhead_factor = 6
    est_per_item = per_image_bytes * overhead_factor
    guessed = max(1, int(avail / (est_per_item * 1.1)))
    return int(max(1, min(64, guessed)))

def main():
    if len(sys.argv) < 3:
        print("Usage: predict_all.py <tub_path> <model_path> [batch_size]", file=sys.stderr)
        sys.exit(1)

    tub_path = sys.argv[1]
    model_path = sys.argv[2]
    arg_batch = sys.argv[3] if len(sys.argv) >= 4 else None
    batch_size = choose_batch_size(arg_batch)

    # 모델 존재 확인
    if not os.path.exists(model_path):
        print(f"[predict_all.py] Model not found: {model_path}", file=sys.stderr)
        print(json.dumps([]))
        return

    try:
        model = load_model(model_path)
        print(f"[predict_all.py] model loaded: {model_path}", file=sys.stderr)
    except Exception as e:
        print("[predict_all.py] model load error: " + str(e), file=sys.stderr)
        print(json.dumps([]))
        return

    catalog_files = find_catalog_files(tub_path)
    all_records = load_all_records(catalog_files)
    all_records.sort(key=lambda x: x.get('_index', 0))

    total = len(all_records)
    if total == 0:
        print(json.dumps([]))
        return

    print(f"[predict_all.py] total records: {total}, using batch_size={batch_size}", file=sys.stderr)

    results = []
    target_size = (120, 160)

    try:
        for start in range(0, total, batch_size):
            end = min(start + batch_size, total)
            batch_records = all_records[start:end]
            batch_imgs = []
            batch_indices = []
            for rec in batch_records:
                image_path_rel = rec.get('cam/image_array', '')
                session_id = rec.get('_session_id', '')
                frame_index = rec.get('_index', -1)

                batch_indices.append(frame_index)

                if session_id == "26-05-21_1" or not image_path_rel:
                    batch_imgs.append(np.zeros((target_size[0], target_size[1], 3), dtype=np.float32))
                    continue

                full_img_path = build_image_candidates(tub_path, image_path_rel)
                if full_img_path:
                    batch_imgs.append(safe_load_image(full_img_path, target_size=target_size))
                else:
                    batch_imgs.append(np.zeros((target_size[0], target_size[1], 3), dtype=np.float32))

            X = np.array(batch_imgs, dtype=np.float32)
            preds = model.predict(X, batch_size=max(1, min(batch_size, 64)), verbose=0)

            for idx, p in enumerate(preds):
                try:
                    steering = float(p[0])
                    throttle = float(p[1]) if len(p) > 1 else 0.0
                except Exception:
                    steering = 0.0
                    throttle = 0.0
                results.append({
                    "index": int(batch_indices[idx]) if batch_indices[idx] is not None else -1,
                    "steering": steering,
                    "throttle": throttle
                })

            # 진행 로그
            print(f"[predict_all.py] processed {end}/{total}", file=sys.stderr)
    except Exception as ex:
        print("[predict_all.py] runtime error: " + str(ex), file=sys.stderr)
        print(json.dumps([]))
        return

    print(f"[predict_all.py] final results count: {len(results)}", file=sys.stderr)
    print(json.dumps(results))

if __name__ == "__main__":
    main()