using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sticker.PC.Module.Main.RemoteController
{
    public class ListboxRemoteController
    {
        ListBox _listBox;

        public ListboxRemoteController(ListBox listBox)
        {
            InitListBox(listBox);
        }

        public ListboxRemoteController(ContentControl contentControl)
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
            Pause();
        }

        public void Right()
        {
            if(_listBox.SelectedItem!=null)
            {
                int position = _listBox.Items.IndexOf(_listBox.SelectedItem);
                if (position < _listBox.Items.Count - 1 )
                {
                    _listBox.SelectedItem = _listBox.Items[position + 1];
                }
            }
        }

        public void Left()
        {
            if (_listBox.SelectedItem != null)
            {
                int position = _listBox.Items.IndexOf(_listBox.SelectedItem);
                if (position > 0)
                {
                    _listBox.SelectedItem = _listBox.Items[position - 1];
                }
            }
        }

        public int SelectedItemIndex()
        {
            if(_listBox.SelectedItem==null)
            {
                return -1;
            }
            return _listBox.Items.IndexOf(_listBox.SelectedItem);
        }

        public void FocusOn()
        {
            _listBox.SelectedItem = _listBox.Items[0];
        }

        public void Pause()
        {
            _listBox.SelectedItem = null;
        }
    }
}
