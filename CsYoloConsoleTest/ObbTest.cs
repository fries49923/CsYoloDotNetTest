using SkiaSharp;
using YoloDotNet;
using YoloDotNet.ExecutionProvider.Cpu;
using YoloDotNet.Extensions;
using YoloDotNet.Models;

public class ObbTest
{
    public static void Run(string modelPath, string imgPath)
    {
        Console.WriteLine($"------ OBB (Oriented Bounding Box) Start ------");

        // init
        using var yolo = new Yolo(new YoloOptions
        {
            ExecutionProvider = new CpuExecutionProvider(modelPath)
        });

        // 載入圖片
        using var image = SKBitmap.Decode(imgPath);

        // 辨識
        var results = yolo.RunObbDetection(image, confidence: 0.25, iou: 0.45);

        // 繪製並儲存
        image.Draw(results);
        image.Save($"obb_result_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");

        foreach (var res in results)
        {
            double angleInDegrees = res.OrientationAngle * (180.0 / Math.PI);

            Console.WriteLine($"Label: {res.Label.Name}");
            Console.WriteLine($"Confidence: {res.Confidence:P1}");
            Console.WriteLine($"Angle: {angleInDegrees:F2}°");
            Console.WriteLine($"Center: ({res.BoundingBox.Left + res.BoundingBox.Width / 2:F0}, {res.BoundingBox.Top + res.BoundingBox.Height / 2:F0})");
            Console.WriteLine("");
        }

        Console.WriteLine($"------------\n");
    }
}