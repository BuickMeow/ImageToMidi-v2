using System;
using System.Threading.Tasks;
using Avalonia;
using ImageToMidi_v2.Models;

namespace ImageToMidi_v2.Services.Interfaces
{
    /// <summary>
    /// 图片动画服务接口，提供缩放动画功能
    /// </summary>
    public interface IImageAnimationService
    {
        /// <summary>
        /// 异步执行基于指定位置的缩放动画
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        /// <param name="mousePosition">缩放中心位置</param>
        /// <param name="onUpdate">每次动画更新时的回调函数</param>
        /// <param name="onComplete">动画完成时的回调函数</param>
        /// <returns>表示动画操作的任务</returns>
        Task AnimateZoomToPositionAsync(ZoomPanState state, Point mousePosition, Action onUpdate, Action onComplete);
        
        /// <summary>
        /// 异步执行以控件中心为基准的缩放动画
        /// </summary>
        /// <param name="state">缩放平移状态对象</param>
        /// <param name="onUpdate">每次动画更新时的回调函数</param>
        /// <param name="onComplete">动画完成时的回调函数</param>
        /// <returns>表示动画操作的任务</returns>
        Task AnimateZoomToCenterAsync(ZoomPanState state, Action onUpdate, Action onComplete);
        
        /// <summary>
        /// 停止当前正在执行的动画
        /// </summary>
        void StopAnimation();
    }
}