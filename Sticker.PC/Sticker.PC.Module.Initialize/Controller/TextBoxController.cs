using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sticker.PC.Module.Initialize.Controller
{
    public class TextBoxController
    {
        TextBox _textBox;

        public TextBoxController(TextBox listBox)
        {
            _textBox = listBox;
        }

        public TextBoxController(ContentControl contentControl)
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
                if (item is TextBox)
                {
                    _textBox = item as TextBox;
                    return;
                }

                if (item is Panel)
                {
                    FindListBox(item as Panel);
                }
            }
        }

        public void SetTextProperty(string str)
        {
            _textBox.Text = str;
        }

        public string GetTextProperty()
        {
            return _textBox.Text;
        }

        public void InitListBox()
        {
            _textBox.IsEnabled = true;

            _textBox.Text = "";
            _textBox.Focus();
            Keyboard.Focus(_textBox);
        }

        public void SetIsEnabled(bool isEnable)
        {
            _textBox.IsEnabled = isEnable;
        }
    }
}
