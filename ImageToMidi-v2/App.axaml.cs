using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using ImageToMidi_v2.Views;

namespace ImageToMidi_v2
{
    /// <summary>
    /// Ӧ�ó������࣬����Ӧ�ó���ĳ�ʼ��������
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// ��ʼ��Ӧ�ó��򣬼��� XAML ��Դ
        /// </summary>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// ��ܳ�ʼ�����ʱ�Ĵ������������ںͽ����ظ���֤
        /// </summary>
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // ���� Avalonia �� CommunityToolkit ���ظ���֤
                // ��ϸ��Ϣ: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                
                // ���������� - �����ڴ��ڵ� OnLoaded �¼��г�ʼ��
                var mainWindow = new MainWindow();
                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }

        /// <summary>
        /// ���� Avalonia ������ע����֤����������� CommunityToolkit ����֤�ظ�
        /// </summary>
        private void DisableAvaloniaDataAnnotationValidation()
        {
            // ��ȡҪ�Ƴ�����֤�������
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // �Ƴ�ÿ���ҵ��Ĳ��
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}