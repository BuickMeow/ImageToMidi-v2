using Avalonia;

namespace ImageToMidi_v2.Models
{
    /// <summary>
    /// ���ź�ƽ��״̬������ģ�ͣ���װ��ͼƬ���ſؼ�������״̬��Ϣ
    /// </summary>
    public class ZoomPanState
    {
        /// <summary>
        /// ��ȡ�����õ�ǰ�����ż���
        /// </summary>
        /// <value>���ż���1.0 ��ʾԭʼ��С</value>
        public double Zoom { get; set; } = 1.0;
        
        /// <summary>
        /// ��ȡ������Ŀ�����ż������ڶ������ɣ�
        /// </summary>
        /// <value>Ŀ�����ż���1.0 ��ʾԭʼ��С</value>
        public double TargetZoom { get; set; } = 1.0;
        
        /// <summary>
        /// ��ȡ������ͼƬ��ƫ��λ��
        /// </summary>
        /// <value>������������ĵ�ƫ����</value>
        public Point Offset { get; set; } = new(0, 0);
        
        /// <summary>
        /// ��ȡ�����������Ĵ�С
        /// </summary>
        /// <value>����ͼƬ�������ؼ��ĳߴ�</value>
        public Size ContainerSize { get; set; } = new(1, 1);
        
        /// <summary>
        /// ��ȡ������ͼƬ��ԭʼ��С
        /// </summary>
        /// <value>ͼƬ�����سߴ�</value>
        public Size ImageSize { get; set; } = new(1, 1);
        
        // ��괦��״̬
        
        /// <summary>
        /// ��ȡ������ָʾ����Ƿ�δ�ƶ���ֵ
        /// </summary>
        /// <value>�������ڰ��º�δ�ƶ���Ϊ true������Ϊ false</value>
        public bool MouseNotMoved { get; set; } = true;
        
        /// <summary>
        /// ��ȡ������ָʾ��갴ť�Ƿ��µ�ֵ
        /// </summary>
        /// <value>�����갴ť��ǰ��������Ϊ true������Ϊ false</value>
        public bool MouseIsDown { get; set; } = false;
        
        /// <summary>
        /// ��ȡ����������ƶ���ʼʱ��λ��
        /// </summary>
        /// <value>��갴��ʱ������λ��</value>
        public Point MouseMoveStart { get; set; }
        
        /// <summary>
        /// ��ȡ����������ƶ���ʼʱ��ƫ����
        /// </summary>
        /// <value>��갴��ʱͼƬ��ƫ��λ��</value>
        public Point OffsetStart { get; set; }
    }
}