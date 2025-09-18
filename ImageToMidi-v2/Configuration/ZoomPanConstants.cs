namespace ImageToMidi_v2.Configuration
{
    /// <summary>
    /// Zoom and Pan功能的常量配置类，定义了所有相关的默认值和限制
    /// </summary>
    public static class ZoomPanConstants
    {
        // Zoom settings
        
        /// <summary>
        /// 缩放步长，控制每次缩放操作的幅度
        /// </summary>
        public const double ZoomStep = 1;
        
        /// <summary>
        /// 最小缩放级别，防止图片缩放过小
        /// </summary>
        public const double MinZoom = 0.1;
        
        /// <summary>
        /// 最大缩放级别，防止图片缩放过大
        /// </summary>
        public const double MaxZoom = 10.0;
        
        /// <summary>
        /// 最大偏移量，限制图片可以拖拽的范围
        /// </summary>
        public const double MaxOffset = 0.5;
        
        /// <summary>
        /// 鼠标滚轮缩放的倍数因子
        /// </summary>
        public const double ZoomScaleMultiplier = 120.0;
        
        /// <summary>
        /// 鼠标滚轮缩放的敏感度因子
        /// </summary>
        public const double ZoomSensitivityFactor = 30.0;
        
        /*// Edge margin settings for pan clamping
        
        /// <summary>
        /// 最小边缘空间（像素），确保基本的边缘可见性
        /// </summary>
        public const double MinEdgeMargin = 20.0;
        
        /// <summary>
        /// 基础边缘空间比例，相对于容器大小的比例
        /// </summary>
        public const double BaseEdgeMarginRatio = 0.125; // 1/8 of container size
        
        /// <summary>
        /// 边缘空间缩放因子，控制边缘空间随缩放倍率的变化程度
        /// </summary>
        public const double EdgeMarginScaleFactor = 2.0;
        */
        // Animation settings
        
        /// <summary>
        /// 缩放动画的持续时间（毫秒）
        /// </summary>
        public const int ZoomAnimationDurationMs = 150;
        
        /// <summary>
        /// 动画帧间隔时间（毫秒）
        /// </summary>
        public const int AnimationFrameIntervalMs = 16;
        
        // Mouse settings
        
        /// <summary>
        /// 双击检测的超时时间（毫秒）
        /// </summary>
        public const int DoubleClickTimeoutMs = 400;
        
        // Reset zoom behavior
        
        /// <summary>
        /// 重置缩放时的倍数因子（scaleMult 的三次方）
        /// </summary>
        public const double ResetZoomMultiplier = 3.0;
    }
}