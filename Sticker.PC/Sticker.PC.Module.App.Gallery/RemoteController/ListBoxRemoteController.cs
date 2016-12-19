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
    public class ListBoxRemoteController
    {
        ListBox _listBox;
        List<ListBox> _listBoxItem;

        public ListBoxRemoteController(ListBox listBox)
        {
            InitListBox(listBox);
        }

        public ListBoxRemoteController(ContentControl contentControl)
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
            _listBoxItem = new List<ListBox>();

            for (int i=0;i<_listBox.Items.Count;i++)
            {
                ListBoxItem item = _listBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                if(item!=null)
                {
                    ListBox innerListbox = FindDescendant<ListBox>(item);
                    if(innerListbox != null)
                    {
                        _listBoxItem.Add(innerListbox);
                    }
                }
            }
            
            foreach(ListBox item in _listBoxItem)
            {
                for(int i=0;i<item.Items.Count;i++)
                {
                    ListBoxItem lbItem = item.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    if(lbItem!=null)
                    {
                        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(lbItem);
                        ItemHighlightAdorner myAdorner = new ItemHighlightAdorner(lbItem, ItemHighlightAdorner.AdornerType.Main);

                        if(adornerLayer.GetAdorners(lbItem) != null)
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
            }

            int initPosition = GlobalEngine.Instance.GallerySelectPosition;

            int itemNum = initPosition % 21;
            int page = (initPosition - itemNum) / 21;

            _listBox.SelectedItem = _listBox.Items[page];
            _listBoxItem[page].SelectedItem = _listBoxItem[page].Items[itemNum];
            _listBox.ScrollIntoView(_listBox.SelectedItem);
        }

        public T FindDescendant<T>(DependencyObject obj) where T : DependencyObject
        {
            // Check if this object is the specified type
            if (obj is T)
                return obj as T;
            // Check for children
            int childrenCount = VisualTreeHelper.GetChildrenCount(obj);
            if (childrenCount < 1)
                return null;
            // First check all the children
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T)
                    return child as T;
            }
            // Then check the childrens children
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = FindDescendant<T>(VisualTreeHelper.GetChild(obj, i));
                if (child != null && child is T)
                    return child as T;
            }
            return null;
        }

        public void NextPage()
        {
            int nowPosition = _listBox.Items.IndexOf(_listBox.SelectedItem);
            if (nowPosition < _listBox.Items.Count - 1)
            {
                _listBox.SelectedItem = _listBox.Items[nowPosition + 1];
                _listBox.ScrollIntoView(_listBox.SelectedItem);
            }
        }

        public void PreviousPage()
        {
            int nowPosition = _listBox.Items.IndexOf(_listBox.SelectedItem);
            if (nowPosition > 0)
            {
                _listBox.SelectedItem = _listBox.Items[nowPosition - 1];
                _listBox.ScrollIntoView(_listBox.SelectedItem);
            }
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public int GetSelectedItemIndex()
        {
            int nowPosition = _listBox.Items.IndexOf(_listBox.SelectedItem);
            int nowItemsPosition = _listBoxItem[nowPosition].Items.IndexOf(_listBoxItem[nowPosition].SelectedItem);

            return nowPosition * 21 + nowItemsPosition;
        }

        public void MoveSelectedItem(Direction type)
        {
            int nowPosition = _listBox.Items.IndexOf(_listBox.SelectedItem);
            int nowItemsPosition = _listBoxItem[nowPosition].Items.IndexOf(_listBoxItem[nowPosition].SelectedItem);

            switch (type)
            {
                case Direction.Up:
                    {
                        if(nowItemsPosition>6)
                        {
                            _listBoxItem[nowPosition].SelectedItem = _listBoxItem[nowPosition].Items[nowItemsPosition - 7];
                        }
                    } break;
                case Direction.Down:
                    {
                        if (nowItemsPosition < 14)
                        {
                            if (nowItemsPosition + 7 < _listBoxItem[nowPosition].Items.Count)
                            {
                                _listBoxItem[nowPosition].SelectedItem = _listBoxItem[nowPosition].Items[nowItemsPosition + 7];
                            }
                        }
                    }
                    break;
                case Direction.Left:
                    {
                        if(nowItemsPosition%7==0)
                        {
                            if(nowPosition!=0)
                            {
                                _listBoxItem[nowPosition].SelectedItem = null;
                                _listBox.SelectedItem = _listBox.Items[nowPosition - 1];
                                _listBox.ScrollIntoView(_listBox.SelectedItem);
                                _listBoxItem[nowPosition - 1].SelectedItem = _listBoxItem[nowPosition - 1].Items[nowItemsPosition + 6];
                            }
                        }
                        else 
                        {
                            _listBoxItem[nowPosition].SelectedItem = _listBoxItem[nowPosition].Items[nowItemsPosition - 1];
                        }
                    }
                    break;
                case Direction.Right:
                    {
                        if (nowItemsPosition % 7 == 6)
                        {
                            if (nowPosition != _listBox.Items.Count-1)
                            {
                                _listBoxItem[nowPosition].SelectedItem = null;
                                _listBox.SelectedItem = _listBox.Items[nowPosition + 1];
                                _listBox.ScrollIntoView(_listBox.SelectedItem);
                                _listBoxItem[nowPosition + 1].SelectedItem = _listBoxItem[nowPosition + 1].Items[nowItemsPosition - 6];
                                
                            }
                        }
                        else
                        {
                            if (nowItemsPosition + 1 < _listBoxItem[nowPosition].Items.Count)
                            {
                                _listBoxItem[nowPosition].SelectedItem = _listBoxItem[nowPosition].Items[nowItemsPosition + 1];
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
