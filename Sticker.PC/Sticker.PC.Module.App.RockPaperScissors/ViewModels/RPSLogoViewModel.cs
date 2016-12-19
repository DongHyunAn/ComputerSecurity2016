using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Sticker.PC.Infra.Container;
using Sticker.PC.Infra.Events;
using Sticker.PC.Infra.Service.NetworkService;
using Sticker.PC.Infra.StaticResources;
using Sticker.PC.Module.App.RockPaperScissors.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticker.PC.Module.App.RockPaperScissors.ViewModels
{
    public class RPSLogoViewModel : BindableBase, INavigationAware
    {
        IRegionManager _regionManager;
        INetworkService _networkService;
        IEventAggregator _eventAggregator;

        public RPSLogoViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, INetworkService networkService)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _networkService = networkService;
        }

        // Navigate
        private async void NavigateWaitDevicePage()
        {
            _networkService.SetAppController(_networkService.GetMasterPlayer(), NetworkService.ControllerType.RPSWait);

            // Splash
            await Task.Delay(TimeSpan.FromSeconds(2));

            _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("RPSPrepare", UriKind.Relative));
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
            GameEngine.getInstance.initGameEngine(_networkService, _eventAggregator, _regionManager);
            NavigateWaitDevicePage();
        }

        #endregion

    }
}
