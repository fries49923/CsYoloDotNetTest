using SkiaSharp;
using YoloDotNet;
using YoloDotNet.ExecutionProvider.Cpu;
using YoloDotNet.Extensions;
using YoloDotNet.Models;

public class PoseTest
{
    public static void Run(string modelPath, string imgPath)
    {
        Console.WriteLine($"------ Pose Estimation Start ------");

        // init
        using var yolo = new Yolo(new YoloOptions
        {
            ExecutionProvider = new CpuExecutionProvider(modelPath)
        });

        // 載入圖片
        using var image = SKBitmap.Decode(imgPath);

        // 辨識
        var results = yolo.RunPoseEstimation(image, confidence: 0.25, iou: 0.7);

        // 繪製並儲存
        var drawingOptions = GetDrawingOptions();

        image.Draw(results, drawingOptions);
        image.Save($"pose_result_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");

        foreach (var res in results)
        {
            // 取得該偵測目標的關鍵點
            var keypoints = res.KeyPoints;

            Console.WriteLine($"偵測到人物，信心度: {res.Confidence:P1}");

            // 範例：抓取特定關節點
            // Index 0: 鼻子, 5: 左肩, 6: 右肩, 9: 左手腕, 10: 右手腕
            var nose = keypoints[0];
            var leftWrist = keypoints[9];
            var leftShoulder = keypoints[5];

            Console.WriteLine($"-> 鼻子位置: ({nose.X:F0}, {nose.Y:F0})");

            // --- 簡單的動作判定邏輯 ---
            // 如果左手腕的 Y 座標小於左肩膀的 Y 座標 (在圖片座標系中，愈往上 Y 愈小)
            if (leftWrist.Confidence > 0.5 && leftWrist.Y < leftShoulder.Y)
            {
                Console.WriteLine("!!! 偵測到：正在舉起左手 !!!");
            }

            Console.WriteLine("");
        }

        Console.WriteLine($"------------\n");
    }

    // https://github.com/NickSwardh/YoloDotNet/blob/master/Demo/PoseEstimationDemo/Program.cs
    public static PoseDrawingOptions GetDrawingOptions()
    {
        return new PoseDrawingOptions
        {
            DrawBoundingBoxes = true,
            DrawConfidenceScore = true,
            DrawLabels = true,
            EnableFontShadow = true,

            // SKTypeface defines the font used for text rendering.
            // SKTypeface.Default uses the system default font.
            // To load a custom font:
            //   - Use SKTypeface.FromFamilyName("fontFamilyName", SKFontStyle) to load by font family name (if installed).
            //   - Use SKTypeface.FromFile("path/to/font.ttf") to load a font directly from a file.
            // Example:
            //   Font = SKTypeface.FromFamilyName("Arial", SKFontStyle.Normal)
            //   Font = SKTypeface.FromFile("C:\\Fonts\\CustomFont.ttf")
            Font = SKTypeface.Default,

            FontSize = 18,
            FontColor = SKColors.White,
            DrawLabelBackground = true,
            EnableDynamicScaling = true,
            BorderThickness = 2,

            // By default, YoloDotNet automatically assigns colors to bounding boxes.
            // To override these default colors, you can define your own array of hexadecimal color codes.
            // Each element in the array corresponds to the class index in your model.
            // Example:
            //   BoundingBoxHexColors = ["#00ff00", "#547457", ...] // Color per class id

            BoundingBoxOpacity = 128,

            // Specifies the keypoints and their connection map used for drawing.
            KeyPointMarkers = CustomKeyPointColorMap.KeyPoints,

            // Draw keypoints and their connections by defining an array of keypoint markers.
            PoseConfidence = 0.65f

            // The following options configure tracked object tails, which visualize 
            // the movement path of detected objects across a sequence of frames or images.
            // Drawing the tail only works when tracking is enabled (e.g., using SortTracker).
            // This is demonstrated in the VideoStream demo.

            // DrawTrackedTail = false,
            // TailPaintColorEnd = new(),
            // ailPaintColorStart = new(),
            // TailThickness = 0,
        };
    }

    // https://github.com/NickSwardh/YoloDotNet/blob/master/Demo/PoseEstimationDemo/KeyPointSetup.cs
    /// <summary>
    /// Demonstrates configuring a custom keypoint marker profile with custom colors and illustrating keypoint connections.
    /// </summary>
    public static class CustomKeyPointColorMap
    {
        /// <summary>
        /// Keypoints must be in the SAME EXACT order(!) as the classes in your trained model.
        /// </summary>
        private enum KeyPointType
        {
            Nose,
            LeftEye,
            RightEye,
            LeftEar,
            RightEar,
            LeftShoulder,
            RightShoulder,
            LeftElbow,
            RightElbow,
            LeftWrist,
            RightWrist,
            LeftHip,
            RightHip,
            LeftKnee,
            RightKnee,
            LeftAnkle,
            RightAnkle
        }

        /// <summary>
        /// Color names for identifying hexadecimal colors
        /// </summary>
        private enum KeyPointColor
        {
            Green,
            LightBlue,
            Yellow,
            HotPink
        }

        /// <summary>
        /// Named hexadecimal colors
        /// </summary>
        private static Dictionary<KeyPointColor, string> Colors => new()
        {
            { KeyPointColor.Green, "#A2FF33" },     // Light green
            { KeyPointColor.LightBlue, "#33ACFF" }, // Light blue
            { KeyPointColor.Yellow, "#FFF633" },    // Yellow
            { KeyPointColor.HotPink, "#FF33AC" }    // Hot pink
        };

        ///// <summary>
        ///// Keypoint options.
        ///// </summary>
        //public static PoseDrawingOptions KeyPointOptions => new()
        //{
        //    PoseConfidence = 0.65,
        //    KeyPointMarkers = KeyPointMapping
        //};

        #region Method for configuring custom keypoints and their connections
        /// <summary>
        /// Configure keypoint-connections and what colors to use.
        /// </summary>
        public static KeyPointMarker[] KeyPoints =>
        [
            new () // Nose
            {
                Color = Colors[KeyPointColor.Green],
                Connections =
                [
                    new ((int)KeyPointType.LeftEye, Colors[KeyPointColor.Green]),
                    new ((int)KeyPointType.RightEye, Colors[KeyPointColor.Green])
                ]
            },
            new () // Left eye
            {
                Color = Colors[KeyPointColor.Green],
                Connections = [ new ((int)KeyPointType.RightEye, Colors[KeyPointColor.Green]) ]
            },
            new () // Right eye
            {
                Color = Colors[KeyPointColor.Green],
            },
            new () // Left ear
            {
                Color = Colors[KeyPointColor.Green],
                Connections =
                [
                    new ((int)KeyPointType.LeftEye, Colors[KeyPointColor.Green]),
                    new ((int)KeyPointType.LeftShoulder, Colors[KeyPointColor.Green]),
                ]
            },
            new () // Right ear
            {
                Color = Colors[KeyPointColor.Green],
                Connections =
                [
                    new ((int)KeyPointType.RightEye, Colors[KeyPointColor.Green]),
                    new ((int)KeyPointType.RightShoulder, Colors[KeyPointColor.Green]),
                ]
            },
            new () // Left shoulder
            {
                Color = Colors[KeyPointColor.LightBlue],
                Connections =
                [
                    new ((int)KeyPointType.RightShoulder, Colors[KeyPointColor.LightBlue]),
                    new ((int)KeyPointType.LeftElbow, Colors[KeyPointColor.LightBlue]),
                    new ((int)KeyPointType.LeftHip, Colors[KeyPointColor.HotPink])
                ]
            },
            new () // Right shoulder
            {
                Color = Colors[KeyPointColor.LightBlue],
                Connections =
                [
                    new ((int)KeyPointType.RightElbow, Colors[KeyPointColor.LightBlue]),
                    new ((int)KeyPointType.RightHip, Colors[KeyPointColor.HotPink])
                ]
            },
            new () // Left elbow
            {
                Color = Colors[KeyPointColor.LightBlue],
                Connections = [ new ((int)KeyPointType.LeftWrist, Colors[KeyPointColor.LightBlue]) ]
            },
            new () // Right elbow
            {
                Color = Colors[KeyPointColor.LightBlue],
                Connections = [ new ((int)KeyPointType.RightWrist, Colors[KeyPointColor.LightBlue]) ]
            },
            new () // Left wrist
            {
                Color = Colors[KeyPointColor.LightBlue]
            },
            new () // Right wrist
            {
                Color = Colors[KeyPointColor.LightBlue]
            },
            new () // Left hip
            {
                Color = Colors[KeyPointColor.Yellow],
                Connections =
                [
                    new ((int)KeyPointType.RightHip, Colors[KeyPointColor.HotPink]),
                    new ((int)KeyPointType.LeftKnee, Colors[KeyPointColor.Yellow])
                ]
            },
            new () // Right hip
            {
                Color = Colors[KeyPointColor.Yellow],
                Connections = [ new ((int)KeyPointType.RightKnee, Colors[KeyPointColor.Yellow]) ]
            },
            new () // Left knee
            {
                Color = Colors[KeyPointColor.Yellow],
                Connections = [ new ((int)KeyPointType.LeftAnkle, Colors[KeyPointColor.Yellow]) ]
            },
            new () // Right knee
            {
                Color = Colors[KeyPointColor.Yellow],
                Connections = [ new ((int)KeyPointType.RightAnkle, Colors[KeyPointColor.Yellow]) ]
            },
            new () // Left ankle
            {
                Color = Colors[KeyPointColor.Yellow]
            },
            new () // Right ankle
            {
                Color = Colors[KeyPointColor.Yellow]
            }
        ];
        #endregion

    }
}