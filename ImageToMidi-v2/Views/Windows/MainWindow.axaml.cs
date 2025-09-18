using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ImageToMidi_v2.Services.Implementation;
using ImageToMidi_v2.ViewModels;

namespace ImageToMidi_v2.Views
{
    /// <summary>
    /// ��������ͼ���ṩ�û�����ͻ����Ĵ��ڽ�������
    /// </summary>
    public partial class MainWindow : Window
    {
        private WindowService? _windowService;

        /// <summary>
        /// ��ʼ�� MainWindow ����ʵ��
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ���ڼ������ʱ�Ĵ�����ʼ���������ͼģ��
        /// </summary>
        /// <param name="e">·���¼�����</param>
        protected override void OnLoaded(Avalonia.Interactivity.RoutedEventArgs e)
        {
            base.OnLoaded(e);
            
            // �ڴ��ڼ��غ��ʼ�����ڷ���
            _windowService = new WindowService(this);
            
            // �����ļ��Ի������ʹ��ڷ���Ȼ�󴴽� ViewModel
            var fileDialogService = new FileDialogService(StorageProvider);
            DataContext = new MainWindowViewModel(fileDialogService, _windowService);
        }

        /// <summary>
        /// �������������갴���¼���֧�ִ�����ק��˫�����
        /// </summary>
        /// <param name="sender">�¼�������</param>
        /// <param name="e">��갴���¼�����</param>
        private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            _windowService?.HandleTitleBarClick(e);
        }

        /// <summary>
        /// ������С����ť����¼�
        /// </summary>
        /// <param name="sender">�¼�������</param>
        /// <param name="e">·���¼�����</param>
        private void MinimizeButton_Click(object? sender, RoutedEventArgs e)
        {
            _windowService?.Minimize();
        }

        /// <summary>
        /// �����˳���ť����¼�
        /// </summary>
        /// <param name="sender">�¼�������</param>
        /// <param name="e">·���¼�����</param>
        private void ExitButton_Click(object? sender, RoutedEventArgs e)
        {
            _windowService?.Close();
        }
    }
}