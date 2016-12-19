using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Sticker.PC.Infra.Class;
using Sticker.PC.Infra.Events;
using Sticker.PC.Infra.Service.NetworkService;
using Sticker.PC.Infra.StaticResources;
using System;
using System.Threading;
using System.Windows;

namespace Sticker.PC.Module.Initialize.ViewModels
{
    public class WaitDeviceViewModel : BindableBase, INavigationAware
    {
        private IRegionManager _regionManager;
        private INetworkService _networkService;
        private IEventAggregator _eventAggregator;

        private string _statement;
        public string Statement
        {
            get { return _statement; }
            set { SetProperty(ref _statement, value); }
        }

        public WaitDeviceViewModel(IRegionManager regionManager, INetworkService networkService, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _networkService = networkService;
            _eventAggregator = eventAggregator;
        }

        private void NetworkStatusNotifyImply(string obj)
        {
            Statement = obj;
        }

        private void GetMasterDevice(object state)
        {
            Player master;
            while(true)
            {
                if(_networkService.GetMasterPlayer()!=null)
                {
                    master = _networkService.GetMasterPlayer();
                    break;
                }

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            _networkService.SetAppController(master, NetworkService.ControllerType.Base);

            if (_networkService.GetMasterPlayer().ThumbnailNum == 0)
            {
                _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("CreateProfile", UriKind.Relative));
            }
            else
            {
                _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("AppList", UriKind.Relative));
            }
        }

        #region INavigaitonAware Implements

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            ThreadPool.QueueUserWorkItem(GetMasterDevice);
            _eventAggregator.GetEvent<NetworkStatusNotifyEvent>().Subscribe(NetworkStatusNotifyImply);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<NetworkStatusNotifyEvent>().Unsubscribe(NetworkStatusNotifyImply);
        } 

        #endregion
    }
}
