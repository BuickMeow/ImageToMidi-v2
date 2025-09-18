using System;
using Avalonia;
using ImageToMidi_v2.Configuration;
using ImageToMidi_v2.Models;
using ImageToMidi_v2.Services.Interfaces;

namespace ImageToMidi_v2.Services.Implementation
{
    /// <summary>
    /// ���ź�ƽ�Ƽ�������ʵ�֣��ṩͼƬ���ź�ƽ����ص���ѧ���㹦��
    /// </summary>
    public class ZoomPanCalculator : IZoomPanCalculator
    {
        /// <summary>
        /// �������λ�ø�������ƫ������ʹ���Ų��������λ��Ϊ���Ľ���
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        /// <param name="oldZoom">�ɵ����ż���</param>
        /// <param name="newZoom">�µ����ż���</param>
        /// <param name="mousePosition">����ڿؼ��е�λ��</param>
        public void UpdateZoomOffsetAtMousePosition(ZoomPanState state, double oldZoom, double newZoom, Point mousePosition)
        {
            // ��֤�����������Ч��
            if (state.ImageSize.Width <= 0 || state.ImageSize.Height <= 0 || 
                state.ContainerSize.Width <= 0 || state.ContainerSize.Height <= 0) 
                return;

            // ����ͼƬ��ʾ�ߴ���������е�ƫ��λ��
            var displaySize = GetDisplaySize(state);
            var imageOffset = GetImageOffset(state);

            // �����λ��ת��Ϊ�����ͼƬ�Ĺ�һ������
            double relativeMouseX = (mousePosition.X - imageOffset.X) / displaySize.Width;
            double relativeMouseY = (mousePosition.Y - imageOffset.Y) / displaySize.Height;

            // ���ǵ�ǰ�����ź�ƽ�ƣ����������ͼƬԭʼ����ϵ�е�λ��
            double currentImageX = relativeMouseX / oldZoom - state.Offset.X / oldZoom;
            double currentImageY = relativeMouseY / oldZoom - state.Offset.Y / oldZoom;

            // �����µ�ƫ�ƣ�ʹ���λ�ñ�������ͬ��ͼƬλ����
            double newOffsetX = relativeMouseX / newZoom - currentImageX;
            double newOffsetY = relativeMouseY / newZoom - currentImageY;

            state.Offset = new Point(newOffsetX, newOffsetY);
            ClampOffset(state);
        }

        /// <summary>
        /// ���ڿؼ����ĸ�������ƫ������ʹ���Ų����Կؼ�����Ϊ��׼����
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        /// <param name="oldZoom">�ɵ����ż���</param>
        /// <param name="newZoom">�µ����ż���</param>
        public void UpdateZoomOffsetAtCenter(ZoomPanState state, double oldZoom, double newZoom)
        {
            if (state.ImageSize.Width <= 0 || state.ImageSize.Height <= 0) return;

            // �Կؼ�����Ϊ���ŵ�
            var centerPoint = new Point(state.ContainerSize.Width / 2, state.ContainerSize.Height / 2);
            UpdateZoomOffsetAtMousePosition(state, oldZoom, newZoom, centerPoint);
        }

        /// <summary>
        /// ����ƫ�����ں���Χ�ڣ���ֹͼƬ��ק����
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        public void ClampOffset(ZoomPanState state)
        {
            // ���ڵ�ǰ���ż���̬����߽�����
            double maxOffsetX = ZoomPanConstants.MaxOffset;
            double maxOffsetY = ZoomPanConstants.MaxOffset;

            if (state.Zoom > 1.0)
            {
                // ��ͼƬ���Ŵ�ʱ������������ק��Χ
                maxOffsetX *= state.Zoom;
                maxOffsetY *= state.Zoom;
            }

            // ��ƫ���������ڼ�����ı߽���
            state.Offset = new Point(
                Math.Max(-maxOffsetX, Math.Min(maxOffsetX, state.Offset.X)),
                Math.Max(-maxOffsetY, Math.Min(maxOffsetY, state.Offset.Y))
            );
        }

        /// <summary>
        /// ����ͼƬ�������е���ʾ�ߴ磬����ͼƬ��߱�
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        /// <returns>ͼƬ�������е�ʵ����ʾ�ߴ�</returns>
        public Size GetDisplaySize(ZoomPanState state)
        {
            double aspect = state.ImageSize.Width / state.ImageSize.Height;
            double containerAspect = state.ContainerSize.Width / state.ContainerSize.Height;
            
            // ����ͼƬ�������Ŀ�߱Ⱦ������ŷ�ʽ
            if (aspect > containerAspect)
            {
                // ͼƬ�Ͽ����������Ϊ׼
                return new Size(state.ContainerSize.Width, state.ContainerSize.Width / aspect);
            }
            else
            {
                // ͼƬ�ϸߣ��������߶�Ϊ׼
                return new Size(state.ContainerSize.Height * aspect, state.ContainerSize.Height);
            }
        }

        /// <summary>
        /// ����ͼƬ�������е�ƫ��λ�ã����ھ�����ʾ
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        /// <returns>ͼƬ������������Ͻǵ�ƫ��λ��</returns>
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