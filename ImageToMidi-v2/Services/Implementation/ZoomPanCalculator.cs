using System;
using Avalonia;
using ImageToMidi_v2.Configuration;
using ImageToMidi_v2.Models;
using ImageToMidi_v2.Services.Interfaces;

namespace ImageToMidi_v2.Services.Implementation
{
    /// <summary>
    /// ���ź�ƽ�Ƽ�������ʵ�֣��ṩͼƬ���ź�ƽ����ص���ѧ���㹦��
    /// ���м��㶼�Ը��Ե�����Ϊԭ�㣬ͳһ����ϵͳ
    /// </summary>
    public class ZoomPanCalculator : IZoomPanCalculator
    {
        /// <summary>
        /// ��UI�������ת��Ϊ������������ϵ
        /// </summary>
        /// <param name="uiPosition">UI����ϵ�µ�λ�ã����Ͻ�Ϊԭ�㣩</param>
        /// <param name="containerSize">������С</param>
        /// <returns>����������Ϊԭ�������</returns>
        private Point UIToContainerCenter(Point uiPosition, Size containerSize)
        {
            return new Point(
                uiPosition.X - containerSize.Width / 2,
                uiPosition.Y - containerSize.Height / 2
            );
        }

        /// <summary>
        /// ��������������ת��ΪͼƬ�������꣨��һ����
        /// </summary>
        /// <param name="containerPos">������������ϵ�µ�λ��</param>
        /// <param name="state">����ƽ��״̬����</param>
        /// <returns>��ͼƬ����Ϊԭ��Ĺ�һ������</returns>
        private Point ContainerCenterToImageCenter(Point containerPos, ZoomPanState state)
        {
            var displaySize = GetDisplaySize(state);
            
            return new Point(
                containerPos.X / displaySize.Width,
                containerPos.Y / displaySize.Height
            );
        }

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

            // 1. UI���� -> ������������
            var containerCenterPos = UIToContainerCenter(mousePosition, state.ContainerSize);
            
            // 2. ������������ -> ͼƬ�������꣨��һ����
            var imageCenterPos = ContainerCenterToImageCenter(containerCenterPos, state);
            
            // 3. ���ǵ�ǰ���ź�ƫ�Ƶ�ͼƬ��������
            var contentPos = new Point(
                imageCenterPos.X / Math.Max(1.0, oldZoom) + state.Offset.X,
                imageCenterPos.Y / Math.Max(1.0, oldZoom) + state.Offset.Y
            );
            
            // 4. ���ż��㣨�����������겻�䣩
            double scaleMult = newZoom / oldZoom;
            state.Offset = new Point(
                (state.Offset.X - contentPos.X) * scaleMult + contentPos.X,
                (state.Offset.Y - contentPos.Y) * scaleMult + contentPos.Y
            );

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

            // �����������ţ�����Ҫ�ı�ƫ����
            // ��Ϊ�������ľ����������ģ���ƫ����������������ĵ�
            ClampOffset(state);
        }

        /// <summary>
        /// ������ڹ̶������ƶ���ƫ��������
        /// ʹͼƬ�ƶ�������ƶ�����1:1�����ع�ϵ���������ż���Ӱ��
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        /// <param name="pixelOffset">����ƶ�������ƫ����</param>
        /// <returns>���º��ƫ����</returns>
        public Point UpdateOffsetByPixelMovement(ZoomPanState state, Vector pixelOffset)
        {
            if (state.ImageSize.Width <= 0 || state.ImageSize.Height <= 0 || 
                state.ContainerSize.Width <= 0 || state.ContainerSize.Height <= 0)
                return state.Offset;

            // ������ʾ�ߴ�
            var displaySize = GetDisplaySize(state);
            
            // �ؼ��޸����������ű��ʣ�ȷ����UI�任��ʽƥ��
            // UI��ʽ��actualPixelOffset = normalizedOffset �� displaySize �� zoom
            // �����Ƶ���normalizedOffset = pixelOffset / (displaySize �� zoom)
            // ����ȷ��1:1�������ƶ���ϵ��ͬʱ��任ϵͳ����
            var normalizedOffset = new Point(
                pixelOffset.X / (displaySize.Width * state.Zoom),
                pixelOffset.Y / (displaySize.Height * state.Zoom)
            );

            // ����ƫ����
            return new Point(
                state.Offset.X + normalizedOffset.X,
                state.Offset.Y + normalizedOffset.Y
            );
        }

        /// <summary>
        /// ����ƫ�����ں���Χ�ڣ���ֹͼƬ��ק����
        /// ʹ�ù̶��Ĺ�һ���߽磬��ʵ�������ƶ���Χ��������Ȼ����
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        public void ClampOffset(ZoomPanState state)
        {
            // ʹ�ü򵥵Ĺ̶���һ���߽磬����ɰ汾һ��
            // ���ģ�͵�����֮�����ڣ�
            // ʵ������ƫ�� = ��һ��ƫ�� �� ��ʾ�ߴ� �� ���ű���
            // �����������ӣ��û����ƶ���ʵ�����ؾ����Զ�����
            const double maxNormalizedOffset = 0.5;
            
            state.Offset = new Point(
                Math.Max(-maxNormalizedOffset, Math.Min(maxNormalizedOffset, state.Offset.X)),
                Math.Max(-maxNormalizedOffset, Math.Min(maxNormalizedOffset, state.Offset.Y))
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
        /// ��ȡ����������ϵ��UI����ϵ��ת��ƫ��
        /// ���ڽ��������ĵ�����ת��ΪUIϵͳ��Ҫ�����Ͻ�ԭ������
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        /// <returns>���ĵ�UI����ϵ��ת��ƫ��</returns>
        public Point GetCenterToUIOffset(ZoomPanState state)
        {
            return new Point(
                state.ContainerSize.Width / 2,
                state.ContainerSize.Height / 2
            );
        }
    }
}