using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using ImageToMidi_v2.Services.Interfaces;

namespace ImageToMidi_v2.ViewModels
{
    /// <summary>
    /// 主窗口的视图模型，提供图片浏览和窗口管理功能
    /// </summary>
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IFileDialogService _fileDialogService;
        private readonly IWindowService? _windowService;

        /// <summary>
        /// 获取欢迎消息
        /// </summary>
        //public string Greeting { get; } = "Welcome to Avalonia!";
        // 节能酱：这还留着？这有什么用啊?
        
        /// <summary>
        /// 获取或设置当前选中的图片路径
        /// </summary>
        [ObservableProperty]
        private string? imagePath;
        
        /// <summary>
        /// 获取浏览图片的命令
        /// </summary>
        public ICommand BrowseImageCommand { get; }
        
        /// <summary>
        /// 获取最小化窗口的命令
        /// </summary>
        public ICommand MinimizeCommand => _windowService?.MinimizeCommand ?? new RelayCommand(() => { });
        
        /// <summary>
        /// 获取关闭窗口的命令
        /// </summary>
        public ICommand CloseCommand => _windowService?.CloseCommand ?? new RelayCommand(() => { });
        
        /// <summary>
        /// 获取切换窗口最大化状态的命令
        /// </summary>
        public ICommand MaximizeToggleCommand => _windowService?.MaximizeToggleCommand ?? new RelayCommand(() => { });
        
        /// <summary>
        /// 初始化 MainWindowViewModel 的新实例
        /// </summary>
        /// <param name="fileDialogService">文件对话框服务</param>
        /// <param name="windowService">窗口服务，可选参数</param>
        public MainWindowViewModel(IFileDialogService fileDialogService, IWindowService? windowService = null)
        {
            _fileDialogService = fileDialogService ?? throw new System.ArgumentNullException(nameof(fileDialogService));
            _windowService = windowService;
            BrowseImageCommand = new AsyncRelayCommand(BrowseImage);
        }
        
        /// <summary>
        /// 无参数构造函数，用于设计时支持
        /// </summary>
        public MainWindowViewModel() : this(new DesignTimeFileDialogService())
        {
        }
        
        /// <summary>
        /// 异步执行浏览图片操作
        /// </summary>
        /// <returns>表示异步操作的任务</returns>
        private async Task BrowseImage()
        {
            var selectedFilePath = await _fileDialogService.OpenImageFileAsync();
            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                ImagePath = selectedFilePath;
            }
        }
    }
    
    /// <summary>
    /// 设计时文件对话框服务实现，用于支持 XAML 设计器
    /// </summary>
    internal class DesignTimeFileDialogService : IFileDialogService
    {
        /// <summary>
        /// 设计时实现，总是返回 null
        /// </summary>
        /// <returns>返回 null 的任务</returns>
        public Task<string?> OpenImageFileAsync()
        {
            return Task.FromResult<string?>(null);
        }
    }
}
