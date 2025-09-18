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
    /// ͼƬ���������ʵ�֣��ṩƽ�������Ŷ�������
    /// </summary>
    public class ImageAnimationService : IImageAnimationService
    {
        private readonly IZoomPanCalculator _zoomPanCalculator;
        private System.Threading.Timer? _zoomTimer;

        /// <summary>
        /// ��ʼ�� ImageAnimationService ����ʵ��
        /// </summary>
        /// <param name="zoomPanCalculator">����ƽ�Ƽ������ʵ��</param>
        /// <exception cref="ArgumentNullException">�� zoomPanCalculator Ϊ null ʱ�׳�</exception>
        public ImageAnimationService(IZoomPanCalculator zoomPanCalculator)
        {
            _zoomPanCalculator = zoomPanCalculator ?? throw new ArgumentNullException(nameof(zoomPanCalculator));
        }

        /// <summary>
        /// �첽ִ�л���ָ��λ�õ����Ŷ���
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        /// <param name="mousePosition">��������λ��</param>
        /// <param name="onUpdate">ÿ�ζ�������ʱ�Ļص�����</param>
        /// <param name="onComplete">�������ʱ�Ļص�����</param>
        /// <returns>��ʾ��������������</returns>
        public Task AnimateZoomToPositionAsync(ZoomPanState state, Point mousePosition, Action onUpdate, Action onComplete)
        {
            return StartZoomAnimationAsync(state, mousePosition, onUpdate, onComplete);
        }

        /// <summary>
        /// �첽ִ���Կؼ�����Ϊ��׼�����Ŷ���
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        /// <param name="onUpdate">ÿ�ζ�������ʱ�Ļص�����</param>
        /// <param name="onComplete">�������ʱ�Ļص�����</param>
        /// <returns>��ʾ��������������</returns>
        public Task AnimateZoomToCenterAsync(ZoomPanState state, Action onUpdate, Action onComplete)
        {
            var centerPoint = new Point(state.ContainerSize.Width / 2, state.ContainerSize.Height / 2);
            return StartZoomAnimationAsync(state, centerPoint, onUpdate, onComplete);
        }

        /// <summary>
        /// ֹͣ��ǰ����ִ�еĶ���
        /// </summary>
        public void StopAnimation()
        {
            _zoomTimer?.Dispose();
            _zoomTimer = null;
        }

        /// <summary>
        /// �������Ŷ������ڲ�ʵ��
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        /// <param name="mousePosition">��������λ��</param>
        /// <param name="onUpdate">ÿ�ζ�������ʱ�Ļص�����</param>
        /// <param name="onComplete">�������ʱ�Ļص�����</param>
        /// <returns>��ʾ��������������</returns>
        private Task StartZoomAnimationAsync(ZoomPanState state, Point mousePosition, Action onUpdate, Action onComplete)
        {
            var tcs = new TaskCompletionSource<bool>();
            
            // ֹͣ�κ����ڽ��еĶ���
            StopAnimation();

            // ���ö�������
            var duration = TimeSpan.FromMilliseconds(ZoomPanConstants.ZoomAnimationDurationMs);
            var easing = new QuadraticEaseOut(); // ʹ�ö��λ���Ч��
            var startZoom = state.Zoom;
            var startTime = DateTime.Now;

            // ������ʱ����ִ�ж���֡
            _zoomTimer = new System.Threading.Timer(_ =>
            {
                var elapsed = DateTime.Now - startTime;
                var progress = Math.Min(1.0, elapsed.TotalMilliseconds / duration.TotalMilliseconds);
                
                if (progress >= 1.0)
                {
                    // ������ɣ���������״̬
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

                // ���㵱ǰ֡������ֵ
                var easedProgress = easing.Ease(progress);
                var interpolatedZoom = startZoom + (state.TargetZoom - startZoom) * easedProgress;
                
                // �� UI �߳��ϸ���״̬
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