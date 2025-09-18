using System.Windows.Input;
using Avalonia.Input;

namespace ImageToMidi_v2.Services.Interfaces
{
    /// <summary>
    /// ���ڷ���ӿڣ��ṩ���ڹ���Ϳ��ƹ���
    /// </summary>
    public interface IWindowService
    {
        /// <summary>
        /// ��ȡ��С�����ڵ�����
        /// </summary>
        ICommand MinimizeCommand { get; }
        
        /// <summary>
        /// ��ȡ�رմ��ڵ�����
        /// </summary>
        ICommand CloseCommand { get; }
        
        /// <summary>
        /// ��ȡ�л��������״̬������
        /// </summary>
        ICommand MaximizeToggleCommand { get; }
        
        /// <summary>
        /// ��С������
        /// </summary>
        void Minimize();
        
        /// <summary>
        /// �رմ���
        /// </summary>
        void Close();
        
        /// <summary>
        /// �л����ڵ����״̬������󻯺���ͨ״̬֮���л���
        /// </summary>
        void ToggleMaximize();
        
        /// <summary>
        /// ��ʼ��ק����
        /// </summary>
        void BeginDrag();

        /// <summary>
        /// �������������¼���֧��˫����󻯺���ק����
        /// </summary>
        /// <param name="e">ָ�밴���¼�����</param>
        /// <returns>����¼��������򷵻� true�����򷵻� false</returns>
        bool HandleTitleBarClick(PointerPressedEventArgs e);
    }
}