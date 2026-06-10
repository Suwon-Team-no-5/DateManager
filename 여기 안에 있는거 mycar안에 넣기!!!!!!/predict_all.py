#!/usr/bin/env python3
# predict_all.py (robust batch, safe image load, retry, index 포함, 배치 로그 + 진단 덤프)
import sys
import os
import json
import math
import numpy as np
from tensorflow.keras.models import load_model
from tensorflow.keras.preprocessing.image import load_img, img_to_array

def eprint(*args, **kwargs):
    print(*args, file=sys.stderr, **kwargs)

def safe_load_image(path, target_size=(120,160)):
    try:
        img = load_img(path, target_size=target_size)
        arr = img_to_array(img)
        # 흑백/채널 부족 처리
        if arr.ndim == 2:
            arr = np.stack([arr]*3, axis=-1)
        if arr.shape[-1] == 1:
            arr = np.repeat(arr, 3, axis=-1)
        # 사이즈 보장
        if arr.shape[0] != target_size[0] or arr.shape[1] != target_size[1]:
            img = load_img(path, target_size=target_size)
            arr = img_to_array(img)
            if arr.ndim == 2:
                arr = np.stack([arr]*3, axis=-1)
            if arr.shape[-1] == 1:
                arr = np.repeat(arr, 3, axis=-1)
        return arr.astype("float32") / 255.0
    except Exception as ex:
        eprint(f"[predict_all.py] image load failed: {path} -> {ex}")
        return np.zeros((target_size[0], target_size[1], 3), dtype=np.float32)

def find_catalog_files(tub_path):
    files = []
    for root, dirs, filenames in os.walk(tub_path):
        if f"{os.sep}backup{os.sep}" in root or root.endswith(f"{os.sep}backup"):
            continue
        for fn in filenames:
            if fn.endswith('.catalog'):
                files.append(os.path.join(root, fn))
    return files

def load_all_records(catalog_files):
    records = []
    for cat in catalog_files:
        try:
            with open(cat, 'r', encoding='utf-8') as f:
                for line in f:
                    if not line.strip(): continue
                    try:
                        records.append(json.loads(line))
                    except json.JSONDecodeError:
                        # 무효 라인 건너뜀
                        continue
        except Exception as ex:
            eprint(f"[predict_all.py] failed reading catalog {cat}: {ex}")
            continue
    return records

def build_image_candidates(tub_path, rel_path):
    images_folder = os.path.join(tub_path, "images")
    candidates = [
        os.path.join(tub_path, rel_path),
        os.path.join(images_folder, rel_path),
        os.path.join(images_folder, os.path.basename(rel_path))
    ]
    for c in candidates:
        if c and os.path.exists(c):
            return c
    return None

def _get_mem_available_bytes():
    try:
        with open("/proc/meminfo","r") as f:
            for line in f:
                if line.startswith("MemAvailable:"):
                    parts = line.split()
                    return int(parts[1]) * 1024
    except Exception:
        pass
    try:
        with open("/proc/meminfo","r") as f:
            for line in f:
                if line.startswith("MemTotal:"):
                    parts = line.split()
                    return int(parts[1]) * 1024 // 2
    except Exception:
        pass
    return 1_000_000_000

def choose_batch_size(arg_batch):
    if arg_batch:
        try:
            return max(1, int(arg_batch))
        except:
            return 32
    avail = _get_mem_available_bytes()
    per_image = 120 * 160 * 3 * 4
    overhead = 6
    guessed = max(1, int(avail / (per_image * overhead * 1.1)))
    return int(max(1, min(32, guessed)))

def normalize_preds(preds):
    preds = np.asarray(preds)
    if preds.ndim == 1:
        preds = preds.reshape((-1, 1))
    return preds

def predict_with_retry(model, X, max_sub_batch=8):
    try:
        # GPU가 없으므로 무리한 대용량 배치 대신 소규모 배치(verbose=0)로 안정적 예측
        preds = model.predict(X, batch_size=max(1, min(X.shape[0], 16)), verbose=0)
        return normalize_preds(preds)
    except Exception as ex:
        eprint(f"[predict_all.py] predict failed: {ex} -- 전량 안전 모드로 변환")
        # 실패 시 에러만 내고 멈추지 않도록, 정확히 이미지 개수(X.shape[0])만큼 0으로 채운 배열 반환
        return np.zeros((X.shape[0], 2), dtype=np.float32)

def dump_diagnostics(results):
    try:
        zero_count = sum(1 for r in results if abs(r.get("steering",0)) < 1e-6 and abs(r.get("throttle",0)) < 1e-6)
        nan_count = sum(1 for r in results if math.isnan(r.get("steering",0)) or math.isnan(r.get("throttle",0)))
        inf_count = sum(1 for r in results if math.isinf(r.get("steering",0)) or math.isinf(r.get("throttle",0)))
        zero_indices = [r.get("index", -1) for r in results if abs(r.get("steering",0)) < 1e-6 and abs(r.get("throttle",0)) < 1e-6][:200]
        diag = {"zero_count": zero_count, "nan_count": nan_count, "inf_count": inf_count, "zero_indices_sample": zero_indices}
        with open("/tmp/predict_diagnostics.json", "w", encoding="utf-8") as fh:
            json.dump(diag, fh, ensure_ascii=False)
        eprint("[predict_all.py] diagnostics written to /tmp/predict_diagnostics.json")
    except Exception as ex:
        eprint(f"[predict_all.py] diagnostics write failed: {ex}")

def main():
    if len(sys.argv) < 3:
        eprint("Usage: predict_all.py <tub_path> <model_path> [batch_size]")
        sys.exit(1)

    tub_path = sys.argv[1]
    model_path = sys.argv[2]
    arg_batch = sys.argv[3] if len(sys.argv) >= 4 else None
    batch_size = choose_batch_size(arg_batch)

    if not os.path.exists(model_path):
        eprint(f"[predict_all.py] Model not found: {model_path}")
        print(json.dumps([]))
        return

    try:
        model = load_model(model_path)
        eprint(f"[predict_all.py] model loaded: {model_path}")
    except Exception as ex:
        eprint(f"[predict_all.py] model load error: {ex}")
        print(json.dumps([]))
        return

    catalog_files = find_catalog_files(tub_path)
    all_records = load_all_records(catalog_files)
    all_records.sort(key=lambda x: x.get('_index', 0))
    total = len(all_records)
    if total == 0:
        print(json.dumps([])); return

    eprint(f"[predict_all.py] total records: {total}, using batch_size={batch_size}")

    results = []
    target_size = (120,160)

    try:
        for start in range(0, total, batch_size):
            end = min(start+batch_size, total)
            batch = all_records[start:end]
            imgs = []
            indices = []
            for rec in batch:
                img_rel = rec.get('cam/image_array','')
                session = rec.get('_session_id','')
                idx = rec.get('_index', -1)
                indices.append(idx)
                if session == "26-05-21_1" or not img_rel:
                    imgs.append(np.zeros((target_size[0], target_size[1],3), dtype=np.float32))
                    continue
                full = build_image_candidates(tub_path, img_rel)
                if full:
                    imgs.append(safe_load_image(full, target_size=target_size))
                else:
                    eprint(f"[predict_all.py] image not found for record index {idx}: {img_rel}")
                    imgs.append(np.zeros((target_size[0], target_size[1],3), dtype=np.float32))

            X = np.array(imgs, dtype=np.float32)
            preds = predict_with_retry(model, X)
            preds = normalize_preds(preds)
            pred_len = preds.shape[0]
            expected = len(batch)
            eprint(f"[predict_all.py] batch {start}-{end} preds {pred_len}/{expected}")

            for i in range(expected):
                if i < pred_len:
                    row = preds[i]
                    steer = float(row[0]) if row.size>0 else 0.0
                    thr = float(row[1]) if row.size>1 else 0.0
                else:
                    steer, thr = 0.0, 0.0
                results.append({"index": int(indices[i]) if indices[i] is not None else -1,
                                "steering": steer, "throttle": thr})
            eprint(f"[predict_all.py] processed {end}/{total}")
    except Exception as ex:
        eprint(f"[predict_all.py] runtime error: {ex}")
        print(json.dumps([]))
        return

    eprint(f"[predict_all.py] final results count: {len(results)}")
    dump_diagnostics(results)
    print(json.dumps(results))

if __name__ == "__main__":
    main()