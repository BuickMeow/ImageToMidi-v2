using Avalonia;
using ImageToMidi_v2.Models;

namespace ImageToMidi_v2.Services.Interfaces
{
    /// <summary>
    /// 缩放和平移计算服务接口，提供图片缩放和平移相关的数学计算功能
    /// </summary>
    public interface IZoomPanCalculator
    {
        /// <summary>
        /// 基于鼠标位置更新缩放偏移量，使缩放操作以鼠标位置为中心进行
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        /// <param name="oldZoom">旧的缩放级别</param>
        /// <param name="newZoom">新的缩放级别</param>
        /// <param name="mousePosition">鼠标在控件中的位置</param>
        void UpdateZoomOffsetAtMousePosition(ZoomPanState state, double oldZoom, double newZoom, Point mousePosition);
        
        /// <summary>
        /// 基于控件中心更新缩放偏移量，使缩放操作以控件中心为基准进行
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        /// <param name="oldZoom">旧的缩放级别</param>
        /// <param name="newZoom">新的缩放级别</param>
        void UpdateZoomOffsetAtCenter(ZoomPanState state, double oldZoom, double newZoom);
        
        /// <summary>
        /// 限制偏移量在合理范围内，防止图片拖拽过度
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        void ClampOffset(ZoomPanState state);
        
        /// <summary>
        /// 计算图片在容器中的显示尺寸
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        /// <returns>图片在容器中的实际显示尺寸</returns>
        Size GetDisplaySize(ZoomPanState state);
        
        /// <summary>
        /// 计算图片在容器中的偏移位置（用于居中显示）
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        /// <returns>图片相对于容器左上角的偏移位置</returns>
        Point GetImageOffset(ZoomPanState state);
    }
}