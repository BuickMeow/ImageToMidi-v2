using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;

namespace ImageToMidi_v2.Views
{
    public partial class MainWindow : Window
    {
        private DateTime _lastClickTime = DateTime.MinValue;
        private const int DoubleClickTimeoutMs = 400; // 双击间隔时间（毫秒）
        private bool _isDragging = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                var currentTime = DateTime.Now;
                var timeSinceLastClick = (currentTime - _lastClickTime).TotalMilliseconds;

                if (timeSinceLastClick <= DoubleClickTimeoutMs)
                {
                    // 检测到双击 - 切换最大化状态
                    HandleDoubleClick();
                    _lastClickTime = DateTime.MinValue; // 重置以避免连续点击
                    return;
                }

                _lastClickTime = currentTime;
                _isDragging = false;
                
                // 开始拖拽
                BeginMoveDrag(e);
            }
        }

        private void HandleDoubleClick()
        {
            // 双击标题栏切换最大化/还原状态
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
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