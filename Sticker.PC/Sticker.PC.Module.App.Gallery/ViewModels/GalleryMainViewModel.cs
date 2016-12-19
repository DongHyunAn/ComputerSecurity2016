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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Sticker.PC.Module.App.Gallery.ViewModels
{
    public class GalleryMainViewModel : BindableBase, INavigationAware
    {
        private ObservableCollection<GalleryInfoWrapper> _galleryImagePageList;
        public ObservableCollection<GalleryInfoWrapper> GalleryImagePageList
        {
            get { return _galleryImagePageList; }
            set { SetProperty(ref _galleryImagePageList, value); }
        }

        IEventAggregator _eventaggregator;
        IRegionManager _regionManager;

        ListBoxRemoteController _listBoxController;

        public ICommand ViewLoadedCommand { get; set; }

        public GalleryMainViewModel(IEventAggregator eventaggregator, IRegionManager regionManager)
        {
            this._eventaggregator = eventaggregator;
            this._regionManager = regionManager;

            _galleryImagePageList = Repository.Instance.GalleryImagePageList;
            ViewLoadedCommand = new DelegateCommand<object>(ViewLoadedCommandImply);
        }

        private void ViewLoadedCommandImply(object obj)
        {
            if(obj is UserControl)
            {
                _listBoxController = new ListBoxRemoteController(obj as UserControl);
            }
        }

        private void KeyDownEventImply(KeyEvent.KeyType obj)
        {
            if (_regionManager.Regions[RegionName.MainRegion].ActiveViews.Count() != 0)
            {
                if (!(_regionManager.Regions[RegionName.MainRegion].ActiveViews.First() is GalleryMain))
                {
                    return;
                }
            }

            switch (obj)
            {
                case KeyEvent.KeyType.UP: _listBoxController.MoveSelectedItem(ListBoxRemoteController.Direction.Up); break;
                case KeyEvent.KeyType.DOWN: _listBoxController.MoveSelectedItem(ListBoxRemoteController.Direction.Down); break;
                case KeyEvent.KeyType.RIGHT: _listBoxController.MoveSelectedItem(ListBoxRemoteController.Direction.Right); break;
                case KeyEvent.KeyType.LEFT: _listBoxController.MoveSelectedItem(ListBoxRemoteController.Direction.Left); break;
                case KeyEvent.KeyType.SELECT:
                    {
                        GlobalEngine.Instance.GallerySelectPosition = _listBoxController.GetSelectedItemIndex();
                        _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("GalleryDetail", UriKind.Relative));
                    }
                    break;
                case KeyEvent.KeyType.CANCEL:
                    {
                        _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("AppList", UriKind.Relative));
                    }
                    break;
                default:
                    break;
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
