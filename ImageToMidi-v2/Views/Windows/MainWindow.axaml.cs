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
            // 允许通过拖拽标题栏移动窗口
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                BeginMoveDrag(e);
            }
        }

        private void MinimizeButton_Click(object? sender, RoutedEventArgs e)
        {
            // 最小化窗口
            WindowState = WindowState.Minimized;
        }

        private void ExitButton_Click(object? sender, RoutedEventArgs e)
        {
            // 关闭应用程序
            Close();
        }
    }
}