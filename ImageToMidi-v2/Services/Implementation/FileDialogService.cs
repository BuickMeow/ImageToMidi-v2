using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ImageToMidi_v2.Services.Interfaces;

namespace ImageToMidi_v2.Services.Implementation
{
    /// <summary>
    /// 文件对话框服务的实现，使用 Avalonia 的存储提供程序来处理文件选择
    /// </summary>
    public class FileDialogService : IFileDialogService
    {
        private readonly IStorageProvider _storageProvider;

        /// <summary>
        /// 初始化 FileDialogService 的新实例
        /// </summary>
        /// <param name="storageProvider">Avalonia 存储提供程序实例</param>
        /// <exception cref="ArgumentNullException">当 storageProvider 为 null 时抛出</exception>
        public FileDialogService(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));
        }

        /// <summary>
        /// 异步打开图片文件选择对话框
        /// </summary>
        /// <returns>
        /// 返回一个任务，该任务的结果是选中的图片文件路径。
        /// 如果用户取消选择或没有选择文件，则返回 null。
        /// </returns>
        public async Task<string?> OpenImageFileAsync()
        {
            // 定义图片文件类型过滤器，支持常见的图片格式
            var fileTypeFilter = new FilePickerFileType("图像文件")
            {
                Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif", "*.tiff" },
                AppleUniformTypeIdentifiers = new[] { "public.image" },
                MimeTypes = new[] { "image/*" }
            };

            // 打开文件选择对话框，配置为单选模式
            var files = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "选择图像文件",
                AllowMultiple = false,
                FileTypeFilter = new[] { fileTypeFilter }
            });

            // 返回选中文件的本地路径，如果没有选择文件则返回 null
            return files.Count > 0 ? files.First().Path.LocalPath : null;
        }
    }
}