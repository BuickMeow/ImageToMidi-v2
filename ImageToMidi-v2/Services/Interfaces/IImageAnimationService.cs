using System;
using System.Threading.Tasks;
using Avalonia;
using ImageToMidi_v2.Models;

namespace ImageToMidi_v2.Services.Interfaces
{
    /// <summary>
    /// ͼƬ��������ӿڣ��ṩ���Ŷ�������
    /// </summary>
    public interface IImageAnimationService
    {
        /// <summary>
        /// �첽ִ�л���ָ��λ�õ����Ŷ���
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        /// <param name="mousePosition">��������λ��</param>
        /// <param name="onUpdate">ÿ�ζ�������ʱ�Ļص�����</param>
        /// <param name="onComplete">�������ʱ�Ļص�����</param>
        /// <returns>��ʾ��������������</returns>
        Task AnimateZoomToPositionAsync(ZoomPanState state, Point mousePosition, Action onUpdate, Action onComplete);
        
        /// <summary>
        /// �첽ִ���Կؼ�����Ϊ��׼�����Ŷ���
        /// </summary>
        /// <param name="state">����ƽ��״̬����</param>
        /// <param name="onUpdate">ÿ�ζ�������ʱ�Ļص�����</param>
        /// <param name="onComplete">�������ʱ�Ļص�����</param>
        /// <returns>��ʾ��������������</returns>
        Task AnimateZoomToCenterAsync(ZoomPanState state, Action onUpdate, Action onComplete);
        
        /// <summary>
        /// ֹͣ��ǰ����ִ�еĶ���
        /// </summary>
        void StopAnimation();
    }
}