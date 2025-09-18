using System;
using Avalonia;
using ImageToMidi_v2.Configuration;
using ImageToMidi_v2.Models;
using ImageToMidi_v2.Services.Interfaces;

namespace ImageToMidi_v2.Services.Implementation
{
    /// <summary>
    /// 缩放和平移计算服务的实现，提供图片缩放和平移相关的数学计算功能
    /// </summary>
    public class ZoomPanCalculator : IZoomPanCalculator
    {
        /// <summary>
        /// 基于鼠标位置更新缩放偏移量，使缩放操作以鼠标位置为中心进行
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        /// <param name="oldZoom">旧的缩放级别</param>
        /// <param name="newZoom">新的缩放级别</param>
        /// <param name="mousePosition">鼠标在控件中的位置</param>
        public void UpdateZoomOffsetAtMousePosition(ZoomPanState state, double oldZoom, double newZoom, Point mousePosition)
        {
            // 验证输入参数的有效性
            if (state.ImageSize.Width <= 0 || state.ImageSize.Height <= 0 || 
                state.ContainerSize.Width <= 0 || state.ContainerSize.Height <= 0) 
                return;

            // 计算图片显示尺寸和在容器中的偏移位置
            var displaySize = GetDisplaySize(state);
            var imageOffset = GetImageOffset(state);

            // 将鼠标位置转换为相对于图片的归一化坐标
            double relativeMouseX = (mousePosition.X - imageOffset.X) / displaySize.Width;
            double relativeMouseY = (mousePosition.Y - imageOffset.Y) / displaySize.Height;

            // 考虑当前的缩放和平移，计算鼠标在图片原始坐标系中的位置
            double currentImageX = relativeMouseX / oldZoom - state.Offset.X / oldZoom;
            double currentImageY = relativeMouseY / oldZoom - state.Offset.Y / oldZoom;

            // 计算新的偏移，使鼠标位置保持在相同的图片位置上
            double newOffsetX = relativeMouseX / newZoom - currentImageX;
            double newOffsetY = relativeMouseY / newZoom - currentImageY;

            state.Offset = new Point(newOffsetX, newOffsetY);
            ClampOffset(state);
        }

        /// <summary>
        /// 基于控件中心更新缩放偏移量，使缩放操作以控件中心为基准进行
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        /// <param name="oldZoom">旧的缩放级别</param>
        /// <param name="newZoom">新的缩放级别</param>
        public void UpdateZoomOffsetAtCenter(ZoomPanState state, double oldZoom, double newZoom)
        {
            if (state.ImageSize.Width <= 0 || state.ImageSize.Height <= 0) return;

            // 以控件中心为缩放点
            var centerPoint = new Point(state.ContainerSize.Width / 2, state.ContainerSize.Height / 2);
            UpdateZoomOffsetAtMousePosition(state, oldZoom, newZoom, centerPoint);
        }

        /// <summary>
        /// 限制偏移量在合理范围内，防止图片拖拽过度
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        public void ClampOffset(ZoomPanState state)
        {
            // 基于当前缩放级别动态计算边界限制
            double maxOffsetX = ZoomPanConstants.MaxOffset;
            double maxOffsetY = ZoomPanConstants.MaxOffset;

            if (state.Zoom > 1.0)
            {
                // 当图片被放大时，允许更大的拖拽范围
                maxOffsetX *= state.Zoom;
                maxOffsetY *= state.Zoom;
            }

            // 将偏移量限制在计算出的边界内
            state.Offset = new Point(
                Math.Max(-maxOffsetX, Math.Min(maxOffsetX, state.Offset.X)),
                Math.Max(-maxOffsetY, Math.Min(maxOffsetY, state.Offset.Y))
            );
        }

        /// <summary>
        /// 计算图片在容器中的显示尺寸，保持图片宽高比
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        /// <returns>图片在容器中的实际显示尺寸</returns>
        public Size GetDisplaySize(ZoomPanState state)
        {
            double aspect = state.ImageSize.Width / state.ImageSize.Height;
            double containerAspect = state.ContainerSize.Width / state.ContainerSize.Height;
            
            // 根据图片和容器的宽高比决定缩放方式
            if (aspect > containerAspect)
            {
                // 图片较宽，以容器宽度为准
                return new Size(state.ContainerSize.Width, state.ContainerSize.Width / aspect);
            }
            else
            {
                // 图片较高，以容器高度为准
                return new Size(state.ContainerSize.Height * aspect, state.ContainerSize.Height);
            }
        }

        /// <summary>
        /// 计算图片在容器中的偏移位置，用于居中显示
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        /// <returns>图片相对于容器左上角的偏移位置</returns>
        public Point GetImageOffset(ZoomPanState state)
        {
            var displaySize = GetDisplaySize(state);
            return new Point(
                (state.ContainerSize.Width - displaySize.Width) / 2,
                (state.ContainerSize.Height - displaySize.Height) / 2
            );
        }
    }
}