using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;

namespace ImageToMidi_v2.Views
{
    public partial class MainWindow : Window
    {
        private DateTime _lastClickTime = DateTime.MinValue;
        private const int DoubleClickTimeoutMs = 400; // ˫�����ʱ�䣨���룩
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
                    // ��⵽˫�� - �л����״̬
                    HandleDoubleClick();
                    _lastClickTime = DateTime.MinValue; // �����Ա����������
                    return;
                }

                _lastClickTime = currentTime;
                _isDragging = false;
                
                // ��ʼ��ק
                BeginMoveDrag(e);
            }
        }

        private void HandleDoubleClick()
        {
            // ˫���������л����/��ԭ״̬
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