using SkiaSharp;
using YoloDotNet;
using YoloDotNet.ExecutionProvider.Cpu;
using YoloDotNet.Extensions;
using YoloDotNet.Models;

namespace CsYoloConsoleTest
{
    public class DetectTest
    {
        public static void Run(string modelPath, string imgPath)
        {
            Console.WriteLine($"------ Detect Start ------");

            // init
            using var yolo = new Yolo(new YoloOptions
            {
                ExecutionProvider = new CpuExecutionProvider(modelPath)
            });

            // 載入圖片
            using var image = SKBitmap.Decode(imgPath);

            // 辨識
            var results = yolo.RunObjectDetection(image, confidence: 0.25, iou: 0.7);

            // 繪製並儲存
            image.Draw(results);
            image.Save($"detect_result_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");

            foreach (var res in results)
            {
                // 類別名稱 & 類別編號
                string label = res.Label.Name;
                int labelId = res.Label.Index;

                // Confidence (0.0 ~ 1.0)
                double score = res.Confidence;

                // 座標資訊 (Rect)
                var box = res.BoundingBox;

                Console.WriteLine($"類別編號: {labelId}");
                Console.WriteLine($"物體: {label} ({score:P1})");
                Console.WriteLine($"位置: X={box.Left}, Y={box.Right}, W={box.Width}, H={box.Height}");
                Console.WriteLine("");
            }

            Console.WriteLine($"------------\n");
        }
    }
}
