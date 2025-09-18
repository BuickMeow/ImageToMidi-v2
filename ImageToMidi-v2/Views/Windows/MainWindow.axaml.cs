using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ImageToMidi_v2.Services.Implementation;
using ImageToMidi_v2.ViewModels;

namespace ImageToMidi_v2.Views
{
    /// <summary>
    /// 主窗口视图，提供用户界面和基本的窗口交互功能
    /// </summary>
    public partial class MainWindow : Window
    {
        private WindowService? _windowService;

        /// <summary>
        /// 初始化 MainWindow 的新实例
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗口加载完成时的处理，初始化服务和视图模型
        /// </summary>
        /// <param name="e">路由事件参数</param>
        protected override void OnLoaded(Avalonia.Interactivity.RoutedEventArgs e)
        {
            base.OnLoaded(e);
            
            // 在窗口加载后初始化窗口服务
            _windowService = new WindowService(this);
            
            // 创建文件对话框服务和窗口服务，然后创建 ViewModel
            var fileDialogService = new FileDialogService(StorageProvider);
            DataContext = new MainWindowViewModel(fileDialogService, _windowService);
        }

        /// <summary>
        /// 处理标题栏的鼠标按下事件，支持窗口拖拽和双击最大化
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">鼠标按下事件参数</param>
        private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            _windowService?.HandleTitleBarClick(e);
        }

        /// <summary>
        /// 处理最小化按钮点击事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">路由事件参数</param>
        private void MinimizeButton_Click(object? sender, RoutedEventArgs e)
        {
            _windowService?.Minimize();
        }

        /// <summary>
        /// 处理退出按钮点击事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">路由事件参数</param>
        private void ExitButton_Click(object? sender, RoutedEventArgs e)
        {
            _windowService?.Close();
        }
    }
}