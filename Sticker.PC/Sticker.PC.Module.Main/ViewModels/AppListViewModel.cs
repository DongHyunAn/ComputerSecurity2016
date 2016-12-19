using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Sticker.PC.Infra;
using Sticker.PC.Infra.Class;
using Sticker.PC.Infra.Class.Model;
using Sticker.PC.Infra.Events;
using Sticker.PC.Infra.Service.NetworkService;
using Sticker.PC.Infra.StaticResources;
using Sticker.PC.Module.Main.RemoteController;
using Sticker.PC.Module.Main.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;


namespace Sticker.PC.Module.Main.ViewModels
{
    public class AppListViewModel : BindableBase, INavigationAware
    {
        IEventAggregator _eventAggregator;
        IRegionManager _regionManager;
        INetworkService _networkservice;

        public enum State
        {
            AppList,
            UserProfile
        }

        private State _state;

        RadCarouselRemoteController _radCarouselRemoteController;
        ListboxRemoteController _listboxRemoteController;
        
        public ICommand ViewLoadedCommand{ get; set; }
        public ICommand ControlCommand{ get; set; }

        private string _masterName;
        public string MasterName
        {
            get { return _masterName; }
            set { SetProperty(ref _masterName, value); }
        }
        private string _masterThumbnailUri;
        public string MasterThumbnailUri
        {
            get { return _masterThumbnailUri; }
            set { SetProperty(ref _masterThumbnailUri, value); }
        }

        private ObservableCollection<AppInfo> _appInfoList;
        public ObservableCollection<AppInfo> AppInfoList
        {
            get {
                if(_appInfoList == null)
                {
                    _appInfoList = Repository.Instance.AppInfoList;
                }
                return _appInfoList;
            }
            set { SetProperty(ref _appInfoList, value); }
        }

        public AppListViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, INetworkService networkservice)
        {
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._networkservice = networkservice;

            ViewLoadedCommand = new DelegateCommand<object>(ViewLoadedCommandImply);
            ControlCommand = new DelegateCommand<object>(ControlCommandImply);
        }

        private void GlobalRemoteImply(KeyEvent.KeyType obj)
        {
            switch (obj)
            {
                case KeyEvent.KeyType.UP:
                    {
                        if(_state==State.AppList)
                        {
                            _state = State.UserProfile;
                            _radCarouselRemoteController.Pause();
                            _listboxRemoteController.FocusOn();
                        }
                    } break;
                case KeyEvent.KeyType.DOWN:
                    {
                        if (_state == State.UserProfile)
                        {
                            _state = State.AppList;
                            _radCarouselRemoteController.FocusOn();
                            _listboxRemoteController.Pause();
                        }
                    } break;
                case KeyEvent.KeyType.RIGHT:
                    {
                        switch (_state)
                        {
                            case State.AppList: _radCarouselRemoteController.LineRight(); break;
                            case State.UserProfile: _listboxRemoteController.Right(); break;
                            default:
                                break;
                        }
                    }break;
                case KeyEvent.KeyType.LEFT:
                    {
                        switch (_state)
                        {
                            case State.AppList: _radCarouselRemoteController.LineLeft(); break;
                            case State.UserProfile: _listboxRemoteController.Left(); break;
                            default:
                                break;
                        }
                    }break;
                case KeyEvent.KeyType.SELECT:
                    {
                        switch (_state)
                        {
                            case State.AppList:
                                {
                                    if (_radCarouselRemoteController != null)
                                    {
                                        Uri uri = _radCarouselRemoteController.CurrentItemSelect();
                                        if (uri != null)
                                        {
                                            _regionManager.RequestNavigate(RegionName.MainRegion, uri);
                                        }
                                        else
                                        {
                                            _eventAggregator.GetEvent<NotifyToastEvent>().Publish("준비중입니다.");
                                        }
                                    }
                                }break;
                            case State.UserProfile:
                                {
                                    switch(_listboxRemoteController.SelectedItemIndex())
                                    {
                                        case 0: _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("CreateProfile", UriKind.Relative)); break;
                                        case 1: _eventAggregator.GetEvent<NotifyToastEvent>().Publish("준비중입니다."); break;
                                        case 2: _eventAggregator.GetEvent<ProgramShutDownEvent>().Publish("프로그램을 종료합니다..."); break;

                                        default:
                                            break;
                                    }
                                }break;
                            default:
                                break;
                        }
                    }
                    break;
                case KeyEvent.KeyType.CANCEL:
                    break;
                default:
                    break;
            }
        }
        
        private void ViewLoadedCommandImply(object obj)
        {
            if(obj is UserControl)
            {
                _radCarouselRemoteController = new RadCarouselRemoteController(obj as UserControl);
                _listboxRemoteController = new ListboxRemoteController(obj as UserControl);

                _networkservice.SetAppController(_networkservice.GetMasterPlayer(), NetworkService.ControllerType.Base);
            }
        }

        private void ControlCommandImply(object obj)
        {
            if(obj is string)
            {
                string command = obj as string;

                switch (command)
                {
                    case "Left":
                        {
                            _radCarouselRemoteController.LineLeft();
                        }
                        break;
                    case "Right":
                        {
                            _radCarouselRemoteController.LineRight();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        #region INavigation Implementation
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<MasterKeyDownEvent>().Subscribe(GlobalRemoteImply);

            Player master = _networkservice.GetMasterPlayer();
            MasterName = master.Nickname;
            MasterThumbnailUri = master.ThumbnailPath;

            _state = State.AppList;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<MasterKeyDownEvent>().Unsubscribe(GlobalRemoteImply);
        } 
        #endregion


    }
}
