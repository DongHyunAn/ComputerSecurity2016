using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Sticker.PC.Infra.Service.NetworkService;
using Sticker.PC.Infra.StaticResources;
using Sticker.PC.Module.App.OneCard.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sticker.PC.Module.App.OneCard.ViewModels
{
    public class OneCardLogoViewModel : BindableBase, INavigationAware
    {
        IRegionManager _regionManager;
        INetworkService _networkService;
        IEventAggregator _eventAggregator;

        public OneCardLogoViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, INetworkService networkService)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _networkService = networkService;
        }

        private async void NavigateWaitDevicePage()
        {
            _networkService.SetAppController(_networkService.GetMasterPlayer(), NetworkService.ControllerType.OneCardWait);
            await Task.Delay(TimeSpan.FromSeconds(2));
            _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("OneCardPrepare", UriKind.Relative));
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
