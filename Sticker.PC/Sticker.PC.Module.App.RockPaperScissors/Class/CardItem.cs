using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticker.PC.Module.App.RockPaperScissors.Class
{
    public class CardItem : BindableBase
    {
        private string _imageUri;
        public string ImageUri
        {
            get { return _imageUri; }
            set { SetProperty(ref _imageUri, value); }
        }

        public CardItem(string str)
        {
            ImageUri = str;
        }
    }
}
