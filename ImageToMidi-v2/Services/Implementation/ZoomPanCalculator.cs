using System;
using Avalonia;
using ImageToMidi_v2.Configuration;
using ImageToMidi_v2.Models;
using ImageToMidi_v2.Services.Interfaces;

namespace ImageToMidi_v2.Services.Implementation
{
    /// <summary>
    /// 缩放和平移计算服务的实现，提供图片缩放和平移相关的数学计算功能
    /// 所有计算都以各自的中心为原点，统一坐标系统
    /// </summary>
    public class ZoomPanCalculator : IZoomPanCalculator
    {
        /// <summary>
        /// 将UI鼠标坐标转换为容器中心坐标系
        /// </summary>
        /// <param name="uiPosition">UI坐标系下的位置（左上角为原点）</param>
        /// <param name="containerSize">容器大小</param>
        /// <returns>以容器中心为原点的坐标</returns>
        private Point UIToContainerCenter(Point uiPosition, Size containerSize)
        {
            return new Point(
                uiPosition.X - containerSize.Width / 2,
                uiPosition.Y - containerSize.Height / 2
            );
        }

        /// <summary>
        /// 将容器中心坐标转换为图片中心坐标（归一化）
        /// </summary>
        /// <param name="containerPos">容器中心坐标系下的位置</param>
        /// <param name="state">缩放平移状态对象</param>
        /// <returns>以图片中心为原点的归一化坐标</returns>
        private Point ContainerCenterToImageCenter(Point containerPos, ZoomPanState state)
        {
            var displaySize = GetDisplaySize(state);
            
            return new Point(
                containerPos.X / displaySize.Width,
                containerPos.Y / displaySize.Height
            );
        }

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

            // 1. UI坐标 -> 容器中心坐标
            var containerCenterPos = UIToContainerCenter(mousePosition, state.ContainerSize);
            
            // 2. 容器中心坐标 -> 图片中心坐标（归一化）
            var imageCenterPos = ContainerCenterToImageCenter(containerCenterPos, state);
            
            // 3. 考虑当前缩放和偏移的图片内容坐标
            var contentPos = new Point(
                imageCenterPos.X / Math.Max(1.0, oldZoom) + state.Offset.X,
                imageCenterPos.Y / Math.Max(1.0, oldZoom) + state.Offset.Y
            );
            
            // 4. 缩放计算（保持内容坐标不变）
            double scaleMult = newZoom / oldZoom;
            state.Offset = new Point(
                (state.Offset.X - contentPos.X) * scaleMult + contentPos.X,
                (state.Offset.Y - contentPos.Y) * scaleMult + contentPos.Y
            );

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

            // 对于中心缩放，不需要改变偏移量
            // 因为缩放中心就是容器中心，而偏移量就是相对于中心的
            ClampOffset(state);
        }

        /// <summary>
        /// 计算基于固定像素移动的偏移量更新
        /// 使图片移动与鼠标移动保持1:1的像素关系，不受缩放级别影响
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        /// <param name="pixelOffset">鼠标移动的像素偏移量</param>
        /// <returns>更新后的偏移量</returns>
        public Point UpdateOffsetByPixelMovement(ZoomPanState state, Vector pixelOffset)
        {
            if (state.ImageSize.Width <= 0 || state.ImageSize.Height <= 0 || 
                state.ContainerSize.Width <= 0 || state.ContainerSize.Height <= 0)
                return state.Offset;

            // 计算显示尺寸
            var displaySize = GetDisplaySize(state);
            
            // 关键修复：考虑缩放倍率，确保与UI变换公式匹配
            // UI公式：actualPixelOffset = normalizedOffset × displaySize × zoom
            // 逆向推导：normalizedOffset = pixelOffset / (displaySize × zoom)
            // 这样确保1:1的像素移动关系，同时与变换系统兼容
            var normalizedOffset = new Point(
                pixelOffset.X / (displaySize.Width * state.Zoom),
                pixelOffset.Y / (displaySize.Height * state.Zoom)
            );

            // 更新偏移量
            return new Point(
                state.Offset.X + normalizedOffset.X,
                state.Offset.Y + normalizedOffset.Y
            );
        }

        /// <summary>
        /// 限制偏移量在合理范围内，防止图片拖拽过度
        /// 使用固定的归一化边界，让实际像素移动范围随缩放自然增长
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        public void ClampOffset(ZoomPanState state)
        {
            // 使用简单的固定归一化边界，就像旧版本一样
            // 这个模型的优雅之处在于：
            // 实际像素偏移 = 归一化偏移 × 显示尺寸 × 缩放倍率
            // 随着缩放增加，用户可移动的实际像素距离自动增长
            const double maxNormalizedOffset = 0.5;
            
            state.Offset = new Point(
                Math.Max(-maxNormalizedOffset, Math.Min(maxNormalizedOffset, state.Offset.X)),
                Math.Max(-maxNormalizedOffset, Math.Min(maxNormalizedOffset, state.Offset.Y))
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
        /// 获取从中心坐标系到UI坐标系的转换偏移
        /// 用于将基于中心的坐标转换为UI系统需要的左上角原点坐标
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        /// <returns>中心到UI坐标系的转换偏移</returns>
        public Point GetCenterToUIOffset(ZoomPanState state)
        {
            return new Point(
                state.ContainerSize.Width / 2,
                state.ContainerSize.Height / 2
            );
        }
    }
}