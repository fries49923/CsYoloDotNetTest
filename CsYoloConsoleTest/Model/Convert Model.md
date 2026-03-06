可以使用 Colab 轉換模型到 onnx

```csharp
!pip install ultralytics

from ultralytics import YOLO

# 載入模型(會自動下載官方預訓練的 .pt 檔)
# 可以選 yolov8n(最快), yolov8s, yolov8m, yolov8l, yolov8x (最準但最慢)
model = YOLO('yolov8n.pt')

# 匯出成 ONNX 格式
# imgsz: 設定模型輸入影像大小，通常是 640
path = model.export(format='onnx', imgsz=640)

print(f"轉檔完成！檔案位置: {path}")
```