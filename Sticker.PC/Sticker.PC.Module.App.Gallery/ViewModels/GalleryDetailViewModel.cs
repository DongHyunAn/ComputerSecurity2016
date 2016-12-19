using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Sticker.PC.Infra;
using Sticker.PC.Infra.Class.Model;
using Sticker.PC.Infra.Events;
using Sticker.PC.Infra.StaticResources;
using Sticker.PC.Module.App.Gallery.Global;
using Sticker.PC.Module.App.Gallery.Global.Sticker.PC.Module.App.Gallery.Global;
using Sticker.PC.Module.App.Gallery.RemoteController;
using Sticker.PC.Module.App.Gallery.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sticker.PC.Module.App.Gallery.ViewModels
{
    public class GalleryDetailViewModel : BindableBase, INavigationAware
    {
        private ObservableCollection<GalleryFileInfo> _galleryImagePageList;
        public ObservableCollection<GalleryFileInfo> GalleryImagePageList
        {
            get { return _galleryImagePageList; }
            set { SetProperty(ref _galleryImagePageList, value); }
        }
        
        IEventAggregator _eventaggregator;
        IRegionManager _regionManager;

        ListBoxRemoteControllerDetail _listBoxController;

        public ICommand ViewLoadedCommand { get; set; }

        private string _imageSource;
        public string ImageSource
        {
            get { return _imageSource; }
            set { SetProperty(ref _imageSource, value); }
        }

        public GalleryDetailViewModel(IEventAggregator eventaggregator, IRegionManager regionManager)
        {
            this._eventaggregator = eventaggregator;
            this._regionManager = regionManager;

            GalleryImagePageList = new ObservableCollection<GalleryFileInfo>();

            for(int i=0;i< Repository.Instance.GalleryImagePageList.Count;i++)
            {
                GalleryInfoWrapper wrapperItem = Repository.Instance.GalleryImagePageList[i];
                
                for(int j=0;j< wrapperItem.GalleryFileList.Count;j++)
                {
                    GalleryImagePageList.Add(wrapperItem.GalleryFileList[j]);
                }
            }
            
            ViewLoadedCommand = new DelegateCommand<object>(ViewLoadedCommandImply);
        }

        private void ViewLoadedCommandImply(object obj)
        {
            if (obj != null)
            {
                ImageSource = GalleryImagePageList[GlobalEngine.Instance.GallerySelectPosition].ImageFilePath;
                _listBoxController = new ListBoxRemoteControllerDetail(obj as UserControl);
                IsControllerShown = false;
            }
        }

        private bool _isControllerShown;
        public bool IsControllerShown
        {
            get { return _isControllerShown; }
            set { SetProperty(ref _isControllerShown, value); }
        }

        private int wait5secound;

        private void KeyDownEventImply(KeyEvent.KeyType obj)
        {
            if (_regionManager.Regions[RegionName.MainRegion].ActiveViews.Count() != 0)
            {
                if (!(_regionManager.Regions[RegionName.MainRegion].ActiveViews.First() is GalleryDetail))
                {
                    return;
                }
            }

            wait5secound = 5;

            if (!IsControllerShown && obj != KeyEvent.KeyType.CANCEL)
            {
                IsControllerShown = true;
                (new Thread(() => {
                    for(;;)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        wait5secound--;

                        if (wait5secound <= 0) break;
                    }
                    IsControllerShown = false;
                })).Start();

                _listBoxController.MoveNowSelectedItemPosition();
                return;
            }

            switch (obj)
            {
                case KeyEvent.KeyType.UP:    _listBoxController.MoveSelectedItem(ListBoxRemoteControllerDetail.Direction.Up);    break;
                case KeyEvent.KeyType.DOWN:  _listBoxController.MoveSelectedItem(ListBoxRemoteControllerDetail.Direction.Down);  break;
                case KeyEvent.KeyType.RIGHT: _listBoxController.MoveSelectedItem(ListBoxRemoteControllerDetail.Direction.Right); break;
                case KeyEvent.KeyType.LEFT:  _listBoxController.MoveSelectedItem(ListBoxRemoteControllerDetail.Direction.Left);  break;
                case KeyEvent.KeyType.SELECT:
                    {
                        GlobalEngine.Instance.GallerySelectPosition = _listBoxController.GetSelectedItemIndex();
                        ImageSource = GalleryImagePageList[_listBoxController.GetSelectedItemIndex()].ImageFilePath;
                    } break;
                case KeyEvent.KeyType.CANCEL:
                    {
                        _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("GalleryMain", UriKind.Relative));
                        IsControllerShown = false;
                        wait5secound = 0;
                    } break;
                default:  break;
            }
        }

        #region INavigationAware Imply
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _eventaggregator.GetEvent<MasterKeyDownEvent>().Subscribe(KeyDownEventImply);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventaggregator.GetEvent<MasterKeyDownEvent>().Unsubscribe(KeyDownEventImply);
        } 
        #endregion
    }
}
