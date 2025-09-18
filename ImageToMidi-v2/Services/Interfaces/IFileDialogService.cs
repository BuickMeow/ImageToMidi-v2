using System.Threading.Tasks;

namespace ImageToMidi_v2.Services.Interfaces
{
    /// <summary>
    /// �ļ��Ի������ӿڣ��ṩ��ͼƬ�ļ��Ĺ���
    /// </summary>
    public interface IFileDialogService
    {
        /// <summary>
        /// �첽��ͼƬ�ļ�ѡ��Ի���
        /// </summary>
        /// <returns>
        /// ����һ�����񣬸�����Ľ����ѡ�е�ͼƬ�ļ�·����
        /// ����û�ȡ��ѡ���û��ѡ���ļ����򷵻� null��
        /// </returns>
        Task<string?> OpenImageFileAsync();
    }
}