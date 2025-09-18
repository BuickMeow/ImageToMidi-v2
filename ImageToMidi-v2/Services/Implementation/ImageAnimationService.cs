using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation.Easings;
using ImageToMidi_v2.Configuration;
using ImageToMidi_v2.Models;
using ImageToMidi_v2.Services.Interfaces;

namespace ImageToMidi_v2.Services.Implementation
{
    /// <summary>
    /// 图片动画服务的实现，提供平滑的缩放动画功能
    /// </summary>
    public class ImageAnimationService : IImageAnimationService
    {
        private readonly IZoomPanCalculator _zoomPanCalculator;
        private System.Threading.Timer? _zoomTimer;

        /// <summary>
        /// 初始化 ImageAnimationService 的新实例
        /// </summary>
        /// <param name="zoomPanCalculator">缩放平移计算服务实例</param>
        /// <exception cref="ArgumentNullException">当 zoomPanCalculator 为 null 时抛出</exception>
        public ImageAnimationService(IZoomPanCalculator zoomPanCalculator)
        {
            _zoomPanCalculator = zoomPanCalculator ?? throw new ArgumentNullException(nameof(zoomPanCalculator));
        }

        /// <summary>
        /// 异步执行基于指定位置的缩放动画
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        /// <param name="mousePosition">缩放中心位置</param>
        /// <param name="onUpdate">每次动画更新时的回调函数</param>
        /// <param name="onComplete">动画完成时的回调函数</param>
        /// <returns>表示动画操作的任务</returns>
        public Task AnimateZoomToPositionAsync(ZoomPanState state, Point mousePosition, Action onUpdate, Action onComplete)
        {
            return StartZoomAnimationAsync(state, mousePosition, onUpdate, onComplete);
        }

        /// <summary>
        /// 异步执行以控件中心为基准的缩放动画
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        /// <param name="onUpdate">每次动画更新时的回调函数</param>
        /// <param name="onComplete">动画完成时的回调函数</param>
        /// <returns>表示动画操作的任务</returns>
        public Task AnimateZoomToCenterAsync(ZoomPanState state, Action onUpdate, Action onComplete)
        {
            var centerPoint = new Point(state.ContainerSize.Width / 2, state.ContainerSize.Height / 2);
            return StartZoomAnimationAsync(state, centerPoint, onUpdate, onComplete);
        }

        /// <summary>
        /// 停止当前正在执行的动画
        /// </summary>
        public void StopAnimation()
        {
            _zoomTimer?.Dispose();
            _zoomTimer = null;
        }

        /// <summary>
        /// 启动缩放动画的内部实现
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        /// <param name="mousePosition">缩放中心位置</param>
        /// <param name="onUpdate">每次动画更新时的回调函数</param>
        /// <param name="onComplete">动画完成时的回调函数</param>
        /// <returns>表示动画操作的任务</returns>
        private Task StartZoomAnimationAsync(ZoomPanState state, Point mousePosition, Action onUpdate, Action onComplete)
        {
            var tcs = new TaskCompletionSource<bool>();
            
            // 停止任何正在进行的动画
            StopAnimation();

            // 设置动画参数
            var duration = TimeSpan.FromMilliseconds(ZoomPanConstants.ZoomAnimationDurationMs);
            var easing = new QuadraticEaseOut(); // 使用二次缓出效果
            var startZoom = state.Zoom;
            var startTime = DateTime.Now;

            // 创建定时器来执行动画帧
            _zoomTimer = new System.Threading.Timer(_ =>
            {
                var elapsed = DateTime.Now - startTime;
                var progress = Math.Min(1.0, elapsed.TotalMilliseconds / duration.TotalMilliseconds);
                
                if (progress >= 1.0)
                {
                    // 动画完成，设置最终状态
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        var oldZoom = state.Zoom;
                        state.Zoom = state.TargetZoom;
                        _zoomPanCalculator.UpdateZoomOffsetAtMousePosition(state, oldZoom, state.Zoom, mousePosition);
                        onUpdate();
                        onComplete();
                    });
                    
                    StopAnimation();
                    tcs.SetResult(true);
                    return;
                }

                // 计算当前帧的缩放值
                var easedProgress = easing.Ease(progress);
                var interpolatedZoom = startZoom + (state.TargetZoom - startZoom) * easedProgress;
                
                // 在 UI 线程上更新状态
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    var oldZoom = state.Zoom;
                    state.Zoom = interpolatedZoom;
                    _zoomPanCalculator.UpdateZoomOffsetAtMousePosition(state, oldZoom, state.Zoom, mousePosition);
                    onUpdate();
                });
                
            }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(ZoomPanConstants.AnimationFrameIntervalMs));

            return tcs.Task;
        }
    }
}