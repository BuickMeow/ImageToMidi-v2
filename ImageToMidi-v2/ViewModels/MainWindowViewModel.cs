using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Platform.Storage;
using System.Linq;
using System.Threading.Tasks;

namespace ImageToMidi_v2.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public string Greeting { get; } = "Welcome to Avalonia!";
        
        [ObservableProperty]
        private string? imagePath;
        
        public ICommand BrowseImageCommand { get; }
        
        // 存储引用以访问文件对话框
        public IStorageProvider? StorageProvider { get; set; }
        
        public MainWindowViewModel()
        {
            BrowseImageCommand = new AsyncRelayCommand(BrowseImage);
        }
        
        private async Task BrowseImage()
        {
            if (StorageProvider == null) return;
            
            // 定义文件类型过滤器
            var fileTypeFilter = new FilePickerFileType("图像文件")
            {
                Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif", "*.tiff" },
                AppleUniformTypeIdentifiers = new[] { "public.image" },
                MimeTypes = new[] { "image/*" }
            };

            // 打开文件选择对话框
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "选择图像文件",
                AllowMultiple = false,
                FileTypeFilter = new[] { fileTypeFilter }
            });

            // 处理选中的文件
            if (files.Count > 0)
            {
                var selectedFile = files.First();
                ImagePath = selectedFile.Path.LocalPath;
            }
        }
    }
}
