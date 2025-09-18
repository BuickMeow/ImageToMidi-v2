using System.Threading.Tasks;

namespace ImageToMidi_v2.Services.Interfaces
{
    /// <summary>
    /// 文件对话框服务接口，提供打开图片文件的功能
    /// </summary>
    public interface IFileDialogService
    {
        /// <summary>
        /// 异步打开图片文件选择对话框
        /// </summary>
        /// <returns>
        /// 返回一个任务，该任务的结果是选中的图片文件路径。
        /// 如果用户取消选择或没有选择文件，则返回 null。
        /// </returns>
        Task<string?> OpenImageFileAsync();
    }
}