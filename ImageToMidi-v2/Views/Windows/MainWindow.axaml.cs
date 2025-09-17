using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace ImageToMidi_v2.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // ����ͨ����ק�������ƶ�����
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                BeginMoveDrag(e);
            }
        }

        private void MinimizeButton_Click(object? sender, RoutedEventArgs e)
        {
            // ��С������
            WindowState = WindowState.Minimized;
        }

        private void ExitButton_Click(object? sender, RoutedEventArgs e)
        {
            // �ر�Ӧ�ó���
            Close();
        }
    }
}