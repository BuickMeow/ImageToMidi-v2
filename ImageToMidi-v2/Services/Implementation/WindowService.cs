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
    /// 提供窗口管理功能的服务，包括最小化、关闭、最大化切换和标题栏拖拽处理
    /// </summary>
    public class WindowService : IWindowService
    {
        private readonly Window _window;
        private DateTime _lastClickTime = DateTime.MinValue;

        /// <summary>
        /// 最小化窗口的命令
        /// </summary>
        public ICommand MinimizeCommand { get; }

        /// <summary>
        /// 关闭窗口的命令
        /// </summary>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// 切换窗口最大化状态的命令
        /// </summary>
        public ICommand MaximizeToggleCommand { get; }

        /// <summary>
        /// 初始化 WindowService 的新实例
        /// </summary>
        /// <param name="window">要管理的窗口实例</param>
        public WindowService(Window window)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            MinimizeCommand = new RelayCommand(Minimize);
            CloseCommand = new RelayCommand(Close);
            MaximizeToggleCommand = new RelayCommand(ToggleMaximize);
        }

        /// <summary>
        /// 最小化窗口
        /// </summary>
        public void Minimize()
        {
            _window.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void Close()
        {
            _window.Close();
        }

        /// <summary>
        /// 切换窗口的最大化状态
        /// </summary>
        public void ToggleMaximize()
        {
            _window.WindowState = _window.WindowState == WindowState.Maximized 
                ? WindowState.Normal 
                : WindowState.Maximized;
        }

        /// <summary>
        /// 开始拖拽窗口（此方法当前为空实现，拖拽在事件处理中直接处理）
        /// </summary>
        public void BeginDrag()
        {
            // 拖拽逻辑在 HandleTitleBarClick 中直接处理
        }

        /// <summary>
        /// 处理标题栏点击事件，支持双击最大化和拖拽功能
        /// </summary>
        /// <param name="e">指针按下事件参数</param>
        /// <returns>如果事件被处理则返回 true，否则返回 false</returns>
        public bool HandleTitleBarClick(PointerPressedEventArgs e)
        {
            if (!e.GetCurrentPoint(_window).Properties.IsLeftButtonPressed)
                return false;

            var currentTime = DateTime.Now;
            var timeSinceLastClick = (currentTime - _lastClickTime).TotalMilliseconds;

            if (timeSinceLastClick <= ZoomPanConstants.DoubleClickTimeoutMs)
            {
                // 检测到双击 - 切换窗口状态
                ToggleMaximize();
                _lastClickTime = DateTime.MinValue; // 重置以避免三次点击
                return true;
            }

            _lastClickTime = currentTime;

            // 开始拖拽
            _window.BeginMoveDrag(e);
            return true;
        }
    }
}