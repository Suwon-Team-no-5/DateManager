# predict_all.py
import sys
import os
import json
import numpy as np
import tensorflow as tf
from tensorflow.keras.models import load_model
from tensorflow.keras.preprocessing.image import load_img, img_to_array

def main():
    tub_path = sys.argv[1]
    model_path = sys.argv[2]

    # 1. AI 모델 최초 1회 로드
    model = load_model(model_path)

    # 2. .catalog 파일 목록 수집 (backup 폴더 제외)
    catalog_files = []
    for root, dirs, files in os.walk(tub_path):
        if f"{os.sep}backup{os.sep}" in root or root.endswith(f"{os.sep}backup"):
            continue
        for file in files:
            if file.endswith('.catalog'):
                catalog_files.append(os.path.join(root, file))

    # 3. 모든 catalog 데이터 수집 및 _index 정렬
    all_records = []
    for cat_file in catalog_files:
        try:
            with open(cat_file, 'r', encoding='utf-8') as f:
                for line in f:
                    if not line.strip(): continue
                    try:
                        all_records.append(json.loads(line))
                    except json.JSONDecodeError: continue
        except Exception: continue

    # 💡 C#의 allFrames.OrderBy(f => f.Index)와 일치화
    all_records.sort(key=lambda x: x.get('_index', 0))

    # 4. 이미지 순차 로드 및 메모리 적재
    images = []
    images_folder = os.path.join(tub_path, "images")

    for record in all_records:
        image_path_rel = record.get('cam/image_array', '')
        session_id = record.get('_session_id', '')

        if session_id == "26-05-21_1" or not image_path_rel:
            images.append(np.zeros((120, 160, 3), dtype=np.float32))
            continue

        full_img_path = ""
        candidates = [
            os.path.join(tub_path, image_path_rel),
            os.path.join(images_folder, image_path_rel),
            os.path.join(images_folder, os.path.basename(image_path_rel))
        ]
        for cand in candidates:
            if os.path.exists(cand):
                full_img_path = cand
                break

        if full_img_path and os.path.exists(full_img_path):
            try:
                img = load_img(full_img_path, target_size=(120, 160))
                images.append(img_to_array(img) / 255.0)
            except Exception:
                images.append(np.zeros((120, 160, 3), dtype=np.float32))
        else:
            images.append(np.zeros((120, 160, 3), dtype=np.float32))

    if len(images) == 0:
        print(json.dumps([]))
        return

    # 5. 초고속 일괄 예측 계산
    X = np.array(images, dtype=np.float32)
    outputs = model.predict(X, batch_size=64, verbose=0)

    # 6. JSON 반환
    results = []
    for out in outputs:
        results.append({
            "steering": float(out[0]),
            "throttle": float(out[1])
        })
    print(json.dumps(results))

if __name__ == "__main__":
    main()
