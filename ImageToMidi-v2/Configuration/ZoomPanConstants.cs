namespace ImageToMidi_v2.Configuration
{
    /// <summary>
    /// Zoom and Pan���ܵĳ��������࣬������������ص�Ĭ��ֵ������
    /// </summary>
    public static class ZoomPanConstants
    {
        // Zoom settings
        
        /// <summary>
        /// ���Ų���������ÿ�����Ų����ķ���
        /// </summary>
        public const double ZoomStep = 1.0;
        
        /// <summary>
        /// ��С���ż��𣬷�ֹͼƬ���Ź�С
        /// </summary>
        public const double MinZoom = 0.1;
        
        /// <summary>
        /// ������ż��𣬷�ֹͼƬ���Ź���
        /// </summary>
        public const double MaxZoom = 10.0;
        
        /// <summary>
        /// ���ƫ����������ͼƬ������ק�ķ�Χ
        /// </summary>
        public const double MaxOffset = 0.5;
        
        /// <summary>
        /// ���������ŵı�������
        /// </summary>
        public const double ZoomScaleMultiplier = 1.3;
        
        /// <summary>
        /// ���������ŵ����ж�����
        /// </summary>
        public const double ZoomSensitivityFactor = 120.0;
        
        // Animation settings
        
        /// <summary>
        /// ���Ŷ����ĳ���ʱ�䣨���룩
        /// </summary>
        public const int ZoomAnimationDurationMs = 150;
        
        /// <summary>
        /// ����֡���ʱ�䣨���룩
        /// </summary>
        public const int AnimationFrameIntervalMs = 16;
        
        // Mouse settings
        
        /// <summary>
        /// ˫�����ĳ�ʱʱ�䣨���룩
        /// </summary>
        public const int DoubleClickTimeoutMs = 400;
        
        // Reset zoom behavior
        
        /// <summary>
        /// ��������ʱ�ı������ӣ�scaleMult �����η���
        /// </summary>
        public const double ResetZoomMultiplier = 3.0;
    }
}