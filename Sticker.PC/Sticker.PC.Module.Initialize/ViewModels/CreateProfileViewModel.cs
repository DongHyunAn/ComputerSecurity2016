using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Sticker.PC.Infra;
using Sticker.PC.Infra.Class.Model;
using Sticker.PC.Infra.Class;
using Sticker.PC.Infra.Events;
using Sticker.PC.Infra.Service.NetworkService;
using Sticker.PC.Infra.StaticResources;
using Sticker.PC.Module.Initialize.Controller;
using Sticker.PC.Module.Initialize.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using Sticker.PC.Infra.Secure;
using System.Windows;

namespace Sticker.PC.Module.Initialize.ViewModels
{
    public class CreateProfileViewModel : BindableBase, INavigationAware
    {
        #region Variable

        enum CreateProfileState
        {
            Thumbnail,
            Nickname,
            Register
        }

        CreateProfileState _nowState;

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private INetworkService _networkService;

        public ICommand NavigateNextPage { get; set; }
        public ICommand ViewLoadedCommand { get; set; }

        private ListBoxController _listController { get; set; }
        private TextBoxController _tbController { get; set; }

        private bool _registerButtonEnable;
        public bool RegisterButtonEnable
        {
            get { return _registerButtonEnable; }
            set { SetProperty(ref _registerButtonEnable, value); }
        }

        private ObservableCollection<ProfileInfo> _profileList;
        public ObservableCollection<ProfileInfo> ProfileList
        {
            get { return _profileList; }
            set { SetProperty(ref _profileList, value); }
        } 

        #endregion

        public CreateProfileViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, INetworkService networkService)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _networkService = networkService;

            ProfileList = Repository.Instance.ProfileImageList;
            RegisterButtonEnable = false;

            NavigateNextPage = new DelegateCommand(navigate);
            ViewLoadedCommand = new DelegateCommand<object>(ViewLoadedCommandImply);
        }

        private void ViewLoadedCommandImply(object obj)
        {
            if (obj is UserControl)
            {
                _listController = new ListBoxController(obj as CreateProfile);
                _tbController = new TextBoxController(obj as CreateProfile);
                _nowState = CreateProfileState.Thumbnail;
            }
        }

        private void navigate()
        {
            if (_nowState == CreateProfileState.Register)
            {
                _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("AppList", UriKind.Relative));
            }
        }

        private void ReceiveTextDataEventImply(string obj)
        {
            if(_nowState==CreateProfileState.Nickname)
            {
                String decrypt = "";

                if (obj.Equals("") || obj.Equals(" ") || obj == null || obj.Equals(null))
                {
                    _tbController.SetTextProperty(decrypt);
                }
                else
                {
                    decrypt = AES128.AESDecrypt128(obj, AES128.key);
                    _tbController.SetTextProperty(AES128.AESDecrypt128(obj, AES128.key));
                    MessageBox.Show(obj);
                }
            }
        }

        private void KeyDownEventImply(KeyEvent.KeyType obj)
        {
            switch (obj)
            {
                case KeyEvent.KeyType.UP:
                case KeyEvent.KeyType.DOWN:
                case KeyEvent.KeyType.RIGHT:
                case KeyEvent.KeyType.LEFT:
                    {
                        if (_nowState == CreateProfileState.Thumbnail)
                        {
                            _listController.MoveSelectedItem(obj);
                        }
                    }
                    break;
                case KeyEvent.KeyType.SELECT:
                    {
                        switch (_nowState)
                        {
                            case CreateProfileState.Thumbnail:
                                {
                                    _listController.HoldNowSelection();
                                    _tbController.InitListBox();
                                    _networkService.SendMessageToPlayer(_networkService.GetMasterPlayer(), "Profile_KeyboardRequest");

                                    _nowState = CreateProfileState.Nickname;
                                }
                                break;
                            case CreateProfileState.Nickname:
                                {
                                    if (_tbController.GetTextProperty() != "")
                                    {
                                        _tbController.SetIsEnabled(false);
                                        RegisterButtonEnable = true;
                                        _nowState = CreateProfileState.Register;
                                    }
                                }
                                break;
                            case CreateProfileState.Register:
                                {
                                    Player player = _networkService.GetMasterPlayer();
                                    player.Nickname = _tbController.GetTextProperty();
                                    player.ThumbnailNum = _listController.GetSelectedItemImagePath();

                                    _networkService.SendMessageToPlayer(player, "Profile_Save_" + player.Nickname + "_" + player.ThumbnailNum.ToString());

                                    _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("AppList", UriKind.Relative));
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case KeyEvent.KeyType.CANCEL:
                    {
                        switch (_nowState)
                        {
                            case CreateProfileState.Thumbnail:
                                {
                                    //_regionManager.RequestNavigate(RegionName.MainRegion, new Uri("WaitDevice", UriKind.Relative));
                                }
                                break;
                            case CreateProfileState.Nickname:
                                {
                                    _listController.UnHoldNowSelection();
                                    _tbController.SetTextProperty("");
                                    _tbController.SetIsEnabled(false);
                                    _nowState = CreateProfileState.Thumbnail;
                                }
                                break;
                            case CreateProfileState.Register:
                                {
                                    RegisterButtonEnable = false;
                                    _tbController.InitListBox();
                                    _networkService.SendMessageToPlayer(_networkService.GetMasterPlayer(), "Profile_KeyboardRequest");
                                    _nowState = CreateProfileState.Nickname;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        #region INavigationAware Implements

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<MasterKeyDownEvent>().Subscribe(KeyDownEventImply);
            _eventAggregator.GetEvent<ReceiveTextDataEvent>().Subscribe(ReceiveTextDataEventImply);

            try
            {
                _listController.UnHoldNowSelection();
                _tbController.SetTextProperty("");
            }
            catch
            {
                // do nothing
            }
            
            _nowState = CreateProfileState.Thumbnail;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<MasterKeyDownEvent>().Unsubscribe(KeyDownEventImply);
            _eventAggregator.GetEvent<ReceiveTextDataEvent>().Unsubscribe(ReceiveTextDataEventImply);
        } 

        #endregion
    }
}
