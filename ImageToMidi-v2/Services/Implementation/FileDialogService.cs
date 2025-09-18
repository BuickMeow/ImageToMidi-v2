using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ImageToMidi_v2.Services.Interfaces;

namespace ImageToMidi_v2.Services.Implementation
{
    /// <summary>
    /// �ļ��Ի�������ʵ�֣�ʹ�� Avalonia �Ĵ洢�ṩ�����������ļ�ѡ��
    /// </summary>
    public class FileDialogService : IFileDialogService
    {
        private readonly IStorageProvider _storageProvider;

        /// <summary>
        /// ��ʼ�� FileDialogService ����ʵ��
        /// </summary>
        /// <param name="storageProvider">Avalonia �洢�ṩ����ʵ��</param>
        /// <exception cref="ArgumentNullException">�� storageProvider Ϊ null ʱ�׳�</exception>
        public FileDialogService(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));
        }

        /// <summary>
        /// �첽��ͼƬ�ļ�ѡ��Ի���
        /// </summary>
        /// <returns>
        /// ����һ�����񣬸�����Ľ����ѡ�е�ͼƬ�ļ�·����
        /// ����û�ȡ��ѡ���û��ѡ���ļ����򷵻� null��
        /// </returns>
        public async Task<string?> OpenImageFileAsync()
        {
            // ����ͼƬ�ļ����͹�������֧�ֳ�����ͼƬ��ʽ
            var fileTypeFilter = new FilePickerFileType("ͼ���ļ�")
            {
                Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif", "*.tiff" },
                AppleUniformTypeIdentifiers = new[] { "public.image" },
                MimeTypes = new[] { "image/*" }
            };

            // ���ļ�ѡ��Ի�������Ϊ��ѡģʽ
            var files = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "ѡ��ͼ���ļ�",
                AllowMultiple = false,
                FileTypeFilter = new[] { fileTypeFilter }
            });

            // ����ѡ���ļ��ı���·�������û��ѡ���ļ��򷵻� null
            return files.Count > 0 ? files.First().Path.LocalPath : null;
        }
    }
}