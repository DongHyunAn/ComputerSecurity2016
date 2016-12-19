using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Sticker.PC.Infra.Container;
using Sticker.PC.Infra.Events;
using Sticker.PC.Infra.StaticResources;
using System;
using System.Threading.Tasks;

namespace Sticker.PC.Module.Initialize.ViewModels
{
    public class LogoViewModel : BindableBase, INavigationAware
    {
        private IRegionManager _regionManager;

        public LogoViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        // Navigate
        private async void NavigateWaitDevicePage()
        {
            // Splash
            await Task.Delay(TimeSpan.FromSeconds(1.5));

            _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("WaitDevice", UriKind.Relative));
        }

        #region INavigationAware Implements

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            NavigateWaitDevicePage();
        }

        #endregion

    }
}
