using SkiaSharp;
using YoloDotNet;
using YoloDotNet.ExecutionProvider.Cpu;
using YoloDotNet.Models;


namespace CsYoloConsoleTest
{
    public class ClassifyTest
    {
        public static void Run(string modelPath, string imgPath)
        {
            Console.WriteLine($"------ Classify Start ------");

            // init
            using var yolo = new Yolo(new YoloOptions
            {
                ExecutionProvider = new CpuExecutionProvider(modelPath)
            });

            // 載入圖片
            using var image = SKBitmap.Decode(imgPath);

            // 辨識
            var result = yolo.RunClassification(image);

            // 處理結果
            if (result.Count > 0)
            {
                var topLabel = result[0].Label;
                var confidence = result[0].Confidence;

                Console.WriteLine($"Label: {topLabel}");
                Console.WriteLine($"Confidence: {confidence:P1}");
            }

            //var result = yolo.RunClassification(image, 5);
            //foreach (var item in result)
            //{
            //    Console.WriteLine($"Label: {item.Label}");
            //    Console.WriteLine($"Confidence: {item.Confidence:P1}\n");
            //}

            Console.WriteLine($"------------\n");
        }
    }
}
