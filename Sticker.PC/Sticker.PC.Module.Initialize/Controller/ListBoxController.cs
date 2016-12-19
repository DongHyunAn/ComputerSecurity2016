using Sticker.PC.Infra.Class.Model;
using Sticker.PC.Infra.Events;
using Sticker.PC.Module.Initialize.ProfileAdorner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace Sticker.PC.Module.Initialize.Controller
{
    public class ListBoxController
    {
        ListBox _listBox;

        public ListBoxController(ListBox listBox)
        {
            InitListBox(listBox);
        }

        public ListBoxController(ContentControl contentControl)
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
                    AdornerLayer layer = AdornerLayer.GetAdornerLayer(lbItem);

                    if(!(lbItem.DataContext is ProfileInfo))
                    {
                        continue;
                    }

                    ProfileInfo model = lbItem.DataContext as ProfileInfo;
                    ProfileThumbnailAdorner myAdorner = new ProfileThumbnailAdorner(lbItem, new Uri(@model.ThumbnailPath, UriKind.Relative));

                    if (layer.GetAdorners(lbItem) != null)
                    {
                        foreach (Adorner adorner in layer.GetAdorners(lbItem))
                        {
                            layer.Remove(adorner);
                        }
                    }
                    layer.Add(myAdorner);

                    Binding selectedBind = new Binding();
                    selectedBind.Source = lbItem;
                    selectedBind.Path = new PropertyPath("IsSelected");
                    selectedBind.Mode = BindingMode.OneWay;
                    selectedBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    BindingOperations.SetBinding(myAdorner, ProfileThumbnailAdorner.IsSelectedProperty, selectedBind);

                    Binding decisionBind = new Binding();
                    decisionBind.Source = lbItem;
                    decisionBind.Path = new PropertyPath("Tag");
                    decisionBind.Mode = BindingMode.OneWay;
                    decisionBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    BindingOperations.SetBinding(myAdorner, ProfileThumbnailAdorner.IsFinalDecisionProperty, decisionBind);
                }
            }

            _listBox.SelectedItem = _listBox.Items[0];
        }

        public int GetSelectedItemImagePath()
        {
            ProfileInfo model = _listBox.Items[SaveSelectionNum] as ProfileInfo;
            return model.ThumbnailNum;
        }

        public void MoveNowSelectedItemPosition()
        {
            _listBox.ScrollIntoView(_listBox.SelectedItem);
        }

        private int SaveSelectionNum;

        public void HoldNowSelection()
        {
            object item = _listBox.ItemContainerGenerator.ContainerFromIndex(_listBox.SelectedIndex);

            if (item is ListBoxItem)
            {
                SaveSelectionNum = _listBox.SelectedIndex;
                ListBoxItem listBoxItem = item as ListBoxItem;
                _listBox.SelectedItem = null;
                listBoxItem.Tag = "D";
            }
        }

        public void UnHoldNowSelection()
        {
            object item = _listBox.ItemContainerGenerator.ContainerFromIndex(SaveSelectionNum);

            if (item is ListBoxItem)
            {
                ListBoxItem listBoxItem = item as ListBoxItem;
                listBoxItem.Tag = "";

                _listBox.SelectedItem = _listBox.Items[SaveSelectionNum];
            }
        }

        public void MoveSelectedItem(KeyEvent.KeyType obj)
        {
            int nowPosition = _listBox.Items.IndexOf(_listBox.SelectedItem);

            switch (obj)
            {
                case KeyEvent.KeyType.UP:
                    {
                        if (nowPosition >= 3)
                        {
                            _listBox.SelectedItem = _listBox.Items[nowPosition - 3];
                            _listBox.ScrollIntoView(_listBox.SelectedItem);
                        }
                    }
                    break;
                case KeyEvent.KeyType.DOWN:
                    {
                        if (nowPosition < 6)
                        {
                            _listBox.SelectedItem = _listBox.Items[nowPosition + 3];
                            _listBox.ScrollIntoView(_listBox.SelectedItem);
                        }
                    }
                    break;
                case KeyEvent.KeyType.RIGHT:
                    {
                        if (nowPosition % 3 != 2)
                        {
                            _listBox.SelectedItem = _listBox.Items[nowPosition + 1];
                            _listBox.ScrollIntoView(_listBox.SelectedItem);
                        }
                    }
                    break;
                case KeyEvent.KeyType.LEFT:
                    {
                        if (nowPosition % 3 != 0)
                        {
                            _listBox.SelectedItem = _listBox.Items[nowPosition - 1];
                            _listBox.ScrollIntoView(_listBox.SelectedItem);
                        }
                    }
                    break;
                case KeyEvent.KeyType.SELECT:
                case KeyEvent.KeyType.CANCEL:
                default:
                    break;
            }
        }
    }
}
