using Microsoft.Practices.Unity;
using Prism.Regions;
using Prism.Unity;
using Sticker.PC.Infra.Container;
using Sticker.PC.Infra.Service.NetworkService;
using Sticker.PC.Infra.StaticResources;
using Sticker.PC.Views;
using System.Windows;

namespace Sticker.PC
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            this.RegisterTypeIfMissing(typeof(INetworkService), typeof(NetworkService), true);
            this.Container.RegisterInstance<IRegionManager>(new StickerRegionManager());
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            #region Sticker Base 

            this.ModuleCatalog.AddModule(new Prism.Modularity.ModuleInfo()
            {
                ModuleName = typeof(Module.Initialize.InitializeModule).Name,
                ModuleType = typeof(Module.Initialize.InitializeModule).AssemblyQualifiedName
            });

            this.ModuleCatalog.AddModule(new Prism.Modularity.ModuleInfo()
            {
                ModuleName = typeof(Module.Main.MainModule).Name,
                ModuleType = typeof(Module.Main.MainModule).AssemblyQualifiedName
            });

            #endregion

            #region Application

            this.ModuleCatalog.AddModule(new Prism.Modularity.ModuleInfo()
            {
                ModuleName = typeof(Module.App.Gallery.GalleryModule).Name,
                ModuleType = typeof(Module.App.Gallery.GalleryModule).AssemblyQualifiedName
            });

            this.ModuleCatalog.AddModule(new Prism.Modularity.ModuleInfo()
            {
                ModuleName = typeof(Module.App.Clock.ClockModule).Name,
                ModuleType = typeof(Module.App.Clock.ClockModule).AssemblyQualifiedName
            });

            this.ModuleCatalog.AddModule(new Prism.Modularity.ModuleInfo()
            {
                ModuleName = typeof(Module.App.Music.MusicModule).Name,
                ModuleType = typeof(Module.App.Music.MusicModule).AssemblyQualifiedName
            });

            this.ModuleCatalog.AddModule(new Prism.Modularity.ModuleInfo()
            {
                ModuleName = typeof(Module.App.Radio.RadioModule).Name,
                ModuleType = typeof(Module.App.Radio.RadioModule).AssemblyQualifiedName
            });

            this.ModuleCatalog.AddModule(new Prism.Modularity.ModuleInfo()
            {
                ModuleName = typeof(Module.App.RockPaperScissors.RockPaperScissorsModule).Name,
                ModuleType = typeof(Module.App.RockPaperScissors.RockPaperScissorsModule).AssemblyQualifiedName
            });

            this.ModuleCatalog.AddModule(new Prism.Modularity.ModuleInfo()
            {
                ModuleName = typeof(Module.App.OneCard.OneCardModule).Name,
                ModuleType = typeof(Module.App.OneCard.OneCardModule).AssemblyQualifiedName
            });

            #endregion
        }

    }
}
