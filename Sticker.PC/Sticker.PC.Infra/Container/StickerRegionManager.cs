using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Sticker.PC.Infra.Container
{
    public class StickerRegionManager : IRegionManager
    {
        #region Variable

        RegionManager _regionManager;
        private double _duration = 0.2;

        enum NavigationAnimateAutoType
        {
            FadeIn,
            FadeOut
        }

        #endregion

        #region Constructor
        public StickerRegionManager()
        {
            _regionManager = new RegionManager();
        } 
        #endregion

        // Custom RequestNavigate

        public void RequestNavigate(string regionName, Uri source)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                NavigationWithAnimation(regionName, source);
            }));
        }

        private async void NavigationWithAnimation(string regionName, Uri source)
        {
            RouteFrameworkElementAndAnimate(GetActiveViewFromRegion(regionName), NavigationAnimateAutoType.FadeOut);

            await Task.Delay(TimeSpan.FromSeconds(_duration));
            _regionManager.RequestNavigate(regionName, source);

            RouteFrameworkElementAndAnimate(GetActiveViewFromRegion(regionName), NavigationAnimateAutoType.FadeIn);
        }

        private UserControl GetActiveViewFromRegion(string regionName)
        {
            var view = _regionManager.Regions[regionName].ActiveViews.FirstOrDefault();
            if (view is UserControl)
            {
                return view as UserControl;
            }
            return null;
        }

        private void RouteFrameworkElementAndAnimate(UserControl userControl, NavigationAnimateAutoType type)
        {
            if(userControl==null)
            {
                return;
            }

            if (userControl.Content is Panel)
            {
                Panel userControlPanel = userControl.Content as Panel;

                foreach (FrameworkElement element in userControlPanel.Children)
                {
                    SetAnimationInItem(element, type);
                }
            }
        }

        private void SetAnimationInItem(FrameworkElement element, NavigationAnimateAutoType type)
        {
            if (element.Tag == null)
            {
                element.BeginAnimation(FrameworkElement.OpacityProperty, GetAnimation(type));
            }else
            {
                if(element.Tag.ToString().Length!=2)
                {
                    return;
                }

                switch (element.Tag.ToString().ElementAt((int)type))
                {
                    case 'I': element.BeginAnimation(FrameworkElement.OpacityProperty, GetAnimation(NavigationAnimateAutoType.FadeIn)); return;
                    case 'O': element.BeginAnimation(FrameworkElement.OpacityProperty, GetAnimation(NavigationAnimateAutoType.FadeOut)); return;
                    case 'N': 
                    default:
                        return;
                }
            }
        }

        private DoubleAnimationBase GetAnimation(NavigationAnimateAutoType type)
        {
            switch (type)
            {
                case NavigationAnimateAutoType.FadeIn: return new DoubleAnimation(0, 1, TimeSpan.FromSeconds(_duration));
                case NavigationAnimateAutoType.FadeOut: return new DoubleAnimation(1, 0, TimeSpan.FromSeconds(_duration));
                default: return null;
            }
        }

        #region IRegionManager Implements

        public IRegionCollection Regions
        {
            get
            {
                return _regionManager.Regions;
            }
        }

        public IRegionManager AddToRegion(string regionName, object view)
        {
            return _regionManager.AddToRegion(regionName, view);
        }

        public IRegionManager CreateRegionManager()
        {
            return _regionManager.CreateRegionManager();
        }

        public IRegionManager RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
        {
            return _regionManager.RegisterViewWithRegion(regionName, getContentDelegate);
        }

        public IRegionManager RegisterViewWithRegion(string regionName, Type viewType)
        {
            return _regionManager.RegisterViewWithRegion(regionName, viewType);
        }

        public void RequestNavigate(string regionName, string source)
        {
            _regionManager.RequestNavigate(regionName, source);
        }

        public void RequestNavigate(string regionName, string source, Action<NavigationResult> navigationCallback)
        {
            _regionManager.RequestNavigate(regionName, source, navigationCallback);
        }

        public void RequestNavigate(string regionName, Uri target, NavigationParameters navigationParameters)
        {
            _regionManager.RequestNavigate(regionName, target, navigationParameters);
        }

        public void RequestNavigate(string regionName, string target, NavigationParameters navigationParameters)
        {
            _regionManager.RequestNavigate(regionName, target, navigationParameters);
        }

        public void RequestNavigate(string regionName, Uri source, Action<NavigationResult> navigationCallback)
        {
            _regionManager.RequestNavigate(regionName, source, navigationCallback);
        }

        public void RequestNavigate(string regionName, string target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            _regionManager.RequestNavigate(regionName, target, navigationCallback, navigationParameters);
        }

        public void RequestNavigate(string regionName, Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            _regionManager.RequestNavigate(regionName, target, navigationCallback, navigationParameters);
        }
        #endregion
    }
}
