using CsYoloConsoleTest;

// Detect
//DetectTest.Run(
//    @"Model\yolov8n.onnx",
//    @"Image\pexels-ezz7-979503.jpg");

// Segmentation
//SegmentationTest.Run(
//    @"Model\yolo26n-seg.onnx",
//    @"Image\pexels-ezz7-979503.jpg");

// Classify
ClassifyTest.Run(
    @"Model\yolo26n-cls.onnx",
    @"Image\pexels-ezz7-979503.jpg");

Console.ReadLine();