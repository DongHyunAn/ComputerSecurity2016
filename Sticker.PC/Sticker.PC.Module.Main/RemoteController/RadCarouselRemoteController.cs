using Sticker.PC.Infra.Class.Model;
using Sticker.PC.Module.Main.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace Sticker.PC.Module.Main.RemoteController
{
    public class RadCarouselRemoteController 
    {
        RadCarousel _radCarousel;

        public int ItemCount { get; set; }
        public int ItemPerPage { get; set; }

        #region RadCarousel Initialize

        public RadCarouselRemoteController(RadCarousel radCarousel)
        {
            InitRadCrousel(radCarousel);
        }

        public RadCarouselRemoteController(ContentControl contentControl)
        {
            if (contentControl.Content is Panel)
            {
                FindRadCarouselInPanel(contentControl.Content as Panel);
            }
        }

        private void FindRadCarouselInPanel(Panel panel)
        {
            foreach (FrameworkElement item in panel.Children)
            {
                if (item is RadCarousel)
                {
                    InitRadCrousel(item as RadCarousel);
                    return;
                }

                if (item is Panel)
                {
                    FindRadCarouselInPanel(item as Panel);
                }
            }
        }

        private void InitRadCrousel(RadCarousel radCarousel)
        {
            this._radCarousel = radCarousel;
            ItemCount = _radCarousel.Items.Count;

            if (ItemCount >= 9)
            {
                _radCarousel.FindCarouselPanel().ItemsPerPage = 9;
                this.ItemPerPage = _radCarousel.FindCarouselPanel().ItemsPerPage;
            }
            else
            {
                _radCarousel.FindCarouselPanel().ItemsPerPage = ItemCount - (1 - ItemCount % 2);
                this.ItemPerPage = _radCarousel.FindCarouselPanel().ItemsPerPage;
            }

            SetReflectionSettings(true);

            _radCarousel.FindCarouselPanel().IsOpacityEnabled = false;
            _radCarousel.SelectedItem = _radCarousel.Items[ItemCount/2];
            _radCarousel.BringDataItemIntoView(_radCarousel.SelectedItem);
        }

        public void SetReflectionSettings(bool visible)
        {
            if (visible)
            {
                _radCarousel.ReflectionSettings.Visibility = Visibility.Visible;
                _radCarousel.ReflectionSettings.OffsetY = 0;
                _radCarousel.ReflectionSettings.Opacity = 0.25;
                _radCarousel.ReflectionSettings.HiddenPercentage = 0.25;
            }
            else
            {
                _radCarousel.ReflectionSettings.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        public void LineLeft()
        {
            int position = _radCarousel.Items.IndexOf(_radCarousel.SelectedItem);
            if (position > 0)
            {
                _radCarousel.SelectedItem = _radCarousel.Items[position - 1];
            }
        }

        public void LineRight()
        {
            int position = _radCarousel.Items.IndexOf(_radCarousel.SelectedItem);
            if (position < ItemCount - 1)
            {
                _radCarousel.SelectedItem = _radCarousel.Items[position + 1];
            }
        }

        public Uri CurrentItemSelect()
        {
            if (_radCarousel.SelectedItem is AppInfo)
            {
                var currentItem = _radCarousel.SelectedItem as AppInfo;
                return currentItem.ModuleUri;
            }
            return null;
        }

        int _pausePosition;

        public void Pause()
        {
            _pausePosition = _radCarousel.Items.IndexOf(_radCarousel.SelectedItem);
            _radCarousel.SelectedItem = null;
        }

        public void FocusOn()
        {
            _radCarousel.SelectedItem = _radCarousel.Items[_pausePosition];
        }

    }
}
