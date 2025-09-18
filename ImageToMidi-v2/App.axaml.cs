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
    /// 应用程序主类，负责应用程序的初始化和配置
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 初始化应用程序，加载 XAML 资源
        /// </summary>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// 框架初始化完成时的处理，设置主窗口和禁用重复验证
        /// </summary>
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // 禁用 Avalonia 和 CommunityToolkit 的重复验证
                // 详细信息: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                
                // 创建主窗口 - 服务将在窗口的 OnLoaded 事件中初始化
                var mainWindow = new MainWindow();
                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }

        /// <summary>
        /// 禁用 Avalonia 的数据注解验证插件，避免与 CommunityToolkit 的验证重复
        /// </summary>
        private void DisableAvaloniaDataAnnotationValidation()
        {
            // 获取要移除的验证插件数组
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // 移除每个找到的插件
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}