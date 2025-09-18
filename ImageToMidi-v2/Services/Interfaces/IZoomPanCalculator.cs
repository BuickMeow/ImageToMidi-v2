using Avalonia;
using ImageToMidi_v2.Models;

namespace ImageToMidi_v2.Services.Interfaces
{
    /// <summary>
    /// ���ź�ƽ�Ƽ������ӿڣ��ṩͼƬ���ź�ƽ����ص���ѧ���㹦��
    /// ���м��㶼�Ը��Ե�����Ϊԭ�㣬ͳһ����ϵͳ
    /// </summary>
    public interface IZoomPanCalculator
    {
        /// <summary>
        /// �������λ�ø�������ƫ������ʹ���Ų��������λ��Ϊ���Ľ���
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        /// <param name="oldZoom">�ɵ����ż���</param>
        /// <param name="newZoom">�µ����ż���</param>
        /// <param name="mousePosition">����ڿؼ��е�λ��</param>
        void UpdateZoomOffsetAtMousePosition(ZoomPanState state, double oldZoom, double newZoom, Point mousePosition);
        
        /// <summary>
        /// ���ڿؼ����ĸ�������ƫ������ʹ���Ų����Կؼ�����Ϊ��׼����
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        /// <param name="oldZoom">�ɵ����ż���</param>
        /// <param name="newZoom">�µ����ż���</param>
        void UpdateZoomOffsetAtCenter(ZoomPanState state, double oldZoom, double newZoom);
        
        /// <summary>
        /// ������ڹ̶������ƶ���ƫ��������
        /// ʹͼƬ�ƶ�������ƶ�����1:1�����ع�ϵ���������ż���Ӱ��
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        /// <param name="pixelOffset">����ƶ�������ƫ����</param>
        /// <returns>���º��ƫ����</returns>
        Point UpdateOffsetByPixelMovement(ZoomPanState state, Vector pixelOffset);
        
        /// <summary>
        /// ����ƫ�����ں���Χ�ڣ���ֹͼƬ��ק����
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        void ClampOffset(ZoomPanState state);
        
        /// <summary>
        /// ����ͼƬ�������е���ʾ�ߴ�
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        /// <returns>ͼƬ�������е�ʵ����ʾ�ߴ�</returns>
        Size GetDisplaySize(ZoomPanState state);
        
        /// <summary>
        /// ��ȡ����������ϵ��UI����ϵ��ת��ƫ��
        /// ���ڽ��������ĵ�����ת��ΪUIϵͳ��Ҫ�����Ͻ�ԭ������
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        /// <returns>���ĵ�UI����ϵ��ת��ƫ��</returns>
        Point GetCenterToUIOffset(ZoomPanState state);
    }
}