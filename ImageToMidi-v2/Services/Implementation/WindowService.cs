using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using ImageToMidi_v2.Configuration;
using ImageToMidi_v2.Services.Interfaces;

namespace ImageToMidi_v2.Services.Implementation
{
    /// <summary>
    /// �ṩ���ڹ����ܵķ��񣬰�����С�����رա�����л��ͱ�������ק����
    /// </summary>
    public class WindowService : IWindowService
    {
        private readonly Window _window;
        private DateTime _lastClickTime = DateTime.MinValue;

        /// <summary>
        /// ��С�����ڵ�����
        /// </summary>
        public ICommand MinimizeCommand { get; }

        /// <summary>
        /// �رմ��ڵ�����
        /// </summary>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// �л��������״̬������
        /// </summary>
        public ICommand MaximizeToggleCommand { get; }

        /// <summary>
        /// ��ʼ�� WindowService ����ʵ��
        /// </summary>
        /// <param name="window">Ҫ����Ĵ���ʵ��</param>
        public WindowService(Window window)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            MinimizeCommand = new RelayCommand(Minimize);
            CloseCommand = new RelayCommand(Close);
            MaximizeToggleCommand = new RelayCommand(ToggleMaximize);
        }

        /// <summary>
        /// ��С������
        /// </summary>
        public void Minimize()
        {
            _window.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// �رմ���
        /// </summary>
        public void Close()
        {
            _window.Close();
        }

        /// <summary>
        /// �л����ڵ����״̬
        /// </summary>
        public void ToggleMaximize()
        {
            _window.WindowState = _window.WindowState == WindowState.Maximized 
                ? WindowState.Normal 
                : WindowState.Maximized;
        }

        /// <summary>
        /// ��ʼ��ק���ڣ��˷�����ǰΪ��ʵ�֣���ק���¼�������ֱ�Ӵ���
        /// </summary>
        public void BeginDrag()
        {
            // ��ק�߼��� HandleTitleBarClick ��ֱ�Ӵ���
        }

        /// <summary>
        /// �������������¼���֧��˫����󻯺���ק����
        /// </summary>
        /// <param name="e">ָ�밴���¼�����</param>
        /// <returns>����¼��������򷵻� true�����򷵻� false</returns>
        public bool HandleTitleBarClick(PointerPressedEventArgs e)
        {
            if (!e.GetCurrentPoint(_window).Properties.IsLeftButtonPressed)
                return false;

            var currentTime = DateTime.Now;
            var timeSinceLastClick = (currentTime - _lastClickTime).TotalMilliseconds;

            if (timeSinceLastClick <= ZoomPanConstants.DoubleClickTimeoutMs)
            {
                // ��⵽˫�� - �л�����״̬
                ToggleMaximize();
                _lastClickTime = DateTime.MinValue; // �����Ա������ε��
                return true;
            }

            _lastClickTime = currentTime;

            // ��ʼ��ק
            _window.BeginMoveDrag(e);
            return true;
        }
    }
}