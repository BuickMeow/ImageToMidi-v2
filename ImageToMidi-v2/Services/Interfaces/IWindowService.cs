using System.Windows.Input;
using Avalonia.Input;

namespace ImageToMidi_v2.Services.Interfaces
{
    /// <summary>
    /// 窗口服务接口，提供窗口管理和控制功能
    /// </summary>
    public interface IWindowService
    {
        /// <summary>
        /// 获取最小化窗口的命令
        /// </summary>
        ICommand MinimizeCommand { get; }
        
        /// <summary>
        /// 获取关闭窗口的命令
        /// </summary>
        ICommand CloseCommand { get; }
        
        /// <summary>
        /// 获取切换窗口最大化状态的命令
        /// </summary>
        ICommand MaximizeToggleCommand { get; }
        
        /// <summary>
        /// 最小化窗口
        /// </summary>
        void Minimize();
        
        /// <summary>
        /// 关闭窗口
        /// </summary>
        void Close();
        
        /// <summary>
        /// 切换窗口的最大化状态（在最大化和普通状态之间切换）
        /// </summary>
        void ToggleMaximize();
        
        /// <summary>
        /// 开始拖拽窗口
        /// </summary>
        void BeginDrag();

        /// <summary>
        /// 处理标题栏点击事件，支持双击最大化和拖拽功能
        /// </summary>
        /// <param name="e">指针按下事件参数</param>
        /// <returns>如果事件被处理则返回 true，否则返回 false</returns>
        bool HandleTitleBarClick(PointerPressedEventArgs e);
    }
}