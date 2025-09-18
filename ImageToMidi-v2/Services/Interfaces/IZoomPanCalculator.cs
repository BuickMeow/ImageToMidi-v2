using Avalonia;
using ImageToMidi_v2.Models;

namespace ImageToMidi_v2.Services.Interfaces
{
    /// <summary>
    /// 缩放和平移计算服务接口，提供图片缩放和平移相关的数学计算功能
    /// 所有计算都以各自的中心为原点，统一坐标系统
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
        /// 计算基于固定像素移动的偏移量更新
        /// 使图片移动与鼠标移动保持1:1的像素关系，不受缩放级别影响
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        /// <param name="pixelOffset">鼠标移动的像素偏移量</param>
        /// <returns>更新后的偏移量</returns>
        Point UpdateOffsetByPixelMovement(ZoomPanState state, Vector pixelOffset);
        
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
        /// 获取从中心坐标系到UI坐标系的转换偏移
        /// 用于将基于中心的坐标转换为UI系统需要的左上角原点坐标
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        /// <returns>中心到UI坐标系的转换偏移</returns>
        Point GetCenterToUIOffset(ZoomPanState state);
    }
}