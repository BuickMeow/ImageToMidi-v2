using Avalonia;

namespace ImageToMidi_v2.Models
{
    /// <summary>
    /// 缩放和平移状态的数据模型，封装了图片缩放控件的所有状态信息
    /// </summary>
    public class ZoomPanState
    {
        /// <summary>
        /// 获取或设置当前的缩放级别
        /// </summary>
        /// <value>缩放级别，1.0 表示原始大小</value>
        public double Zoom { get; set; } = 1.0;
        
        /// <summary>
        /// 获取或设置目标缩放级别（用于动画过渡）
        /// </summary>
        /// <value>目标缩放级别，1.0 表示原始大小</value>
        public double TargetZoom { get; set; } = 1.0;
        
        /// <summary>
        /// 获取或设置图片的偏移位置
        /// </summary>
        /// <value>相对于容器中心的偏移量</value>
        public Point Offset { get; set; } = new(0, 0);
        
        /// <summary>
        /// 获取或设置容器的大小
        /// </summary>
        /// <value>包含图片的容器控件的尺寸</value>
        public Size ContainerSize { get; set; } = new(1, 1);
        
        /// <summary>
        /// 获取或设置图片的原始大小
        /// </summary>
        /// <value>图片的像素尺寸</value>
        public Size ImageSize { get; set; } = new(1, 1);
        
        // 鼠标处理状态
        
        /// <summary>
        /// 获取或设置指示鼠标是否未移动的值
        /// </summary>
        /// <value>如果鼠标在按下后未移动则为 true，否则为 false</value>
        public bool MouseNotMoved { get; set; } = true;
        
        /// <summary>
        /// 获取或设置指示鼠标按钮是否按下的值
        /// </summary>
        /// <value>如果鼠标按钮当前被按下则为 true，否则为 false</value>
        public bool MouseIsDown { get; set; } = false;
        
        /// <summary>
        /// 获取或设置鼠标移动开始时的位置
        /// </summary>
        /// <value>鼠标按下时的坐标位置</value>
        public Point MouseMoveStart { get; set; }
        
        /// <summary>
        /// 获取或设置鼠标移动开始时的偏移量
        /// </summary>
        /// <value>鼠标按下时图片的偏移位置</value>
        public Point OffsetStart { get; set; }
    }
}