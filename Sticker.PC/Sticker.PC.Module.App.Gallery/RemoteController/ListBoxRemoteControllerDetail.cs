using Sticker.PC.Module.App.Gallery.Global;
using Sticker.PC.Module.App.Gallery.Global.Sticker.PC.Module.App.Gallery.Global;
using Sticker.PC.Module.App.Gallery.ItemAdorner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Sticker.PC.Module.App.Gallery.RemoteController
{
    public class ListBoxRemoteControllerDetail
    {
        ListBox _listBox;

        public ListBoxRemoteControllerDetail(ListBox listBox)
        {
            InitListBox(listBox);
        }

        public ListBoxRemoteControllerDetail(ContentControl contentControl)
        {
            if (contentControl.Content is Panel)
            {
                FindListBox(contentControl.Content as Panel);
            }
        }

        private void FindListBox(Panel panel)
        {
            foreach (FrameworkElement item in panel.Children)
            {
                if (item is ListBox)
                {
                    InitListBox(item as ListBox);
                    return;
                }

                if (item is Panel)
                {
                    FindListBox(item as Panel);
                }
            }
        }

        private void InitListBox(ListBox listBox)
        {
            _listBox = listBox;

            for (int i = 0; i < _listBox.Items.Count; i++)
            {
                ListBoxItem lbItem = _listBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                if (lbItem != null)
                {
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(lbItem);
                    ItemHighlightAdorner myAdorner = new ItemHighlightAdorner(lbItem, ItemHighlightAdorner.AdornerType.Detail);

                    if (adornerLayer.GetAdorners(lbItem) != null)
                    {
                        foreach (Adorner adorner in adornerLayer.GetAdorners(lbItem))
                        {
                            adornerLayer.Remove(adorner);
                        }
                    }
                    adornerLayer.Add(myAdorner);

                    Binding myBinding = new Binding();
                    myBinding.Source = lbItem;
                    myBinding.Path = new PropertyPath("IsSelected");
                    myBinding.Mode = BindingMode.OneWay;
                    myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    BindingOperations.SetBinding(myAdorner, ItemHighlightAdorner.IsSelectedProperty, myBinding);
                }
            }

            if (_listBox.Items.Count != 0)
            {
                _listBox.ScrollIntoView(_listBox.Items[_listBox.Items.Count - 1]);
            }

            int initPosition = GlobalEngine.Instance.GallerySelectPosition;
            _listBox.SelectedItem = _listBox.Items[initPosition];
        }

        public int GetSelectedItemIndex()
        {
            return _listBox.Items.IndexOf(_listBox.SelectedItem);
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public void MoveNowSelectedItemPosition()
        {
            _listBox.ScrollIntoView(_listBox.SelectedItem);
        }

        public void MoveSelectedItem(Direction type)
        {
            int nowPosition = _listBox.Items.IndexOf(_listBox.SelectedItem);

            switch (type)
            {
                case Direction.Up: break;
                case Direction.Down: break;
                case Direction.Left:
                    {
                        if (nowPosition > 0)
                        {
                            _listBox.SelectedItem = _listBox.Items[nowPosition - 1];
                            _listBox.ScrollIntoView(_listBox.SelectedItem);
                        }
                    }
                    break;
                case Direction.Right:
                    {
                        if (nowPosition < _listBox.Items.Count - 1 )
                        {
                            _listBox.SelectedItem = _listBox.Items[nowPosition + 1];
                            _listBox.ScrollIntoView(_listBox.SelectedItem);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
