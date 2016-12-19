using Prism.Mvvm;
using Sticker.PC.Infra.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sticker.PC.Module.App.OneCard.Class
{
    public class OneCardPlayer : BindableBase
    {
        #region Properties

        private Player _player;
        public Player Player
        {
            get { return _player; }
            set { SetProperty(ref _player, value); }
        }

        private bool _isExist;
        public bool IsExist
        {
            get { return _isExist; }
            set { SetProperty(ref _isExist, value); }
        }

        #endregion

        public OneCardPlayer()
        {
            init();
        }

        public void init()
        {
            Player = null;
            IsExist = false;

            NextPlayer = null;
            PreviousPlayer = null;
            CardNum = "0";

            IsMyTurn = false;
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Cards.Clear();
            }));

            Cards.CollectionChanged -= Cards_CollectionChanged;
        }

        private void Cards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CardNum = Cards.Count.ToString();
        }

        public void SetPlayer(Player player)
        {
            if (player == null)
            {
                init();
            }
            else
            {
                Player = player;
                IsExist = true;
                Cards.CollectionChanged += Cards_CollectionChanged;
            }
        }
        
        //

        private ObservableCollection<string> _cards = new ObservableCollection<string>();
        public ObservableCollection<string> Cards
        {
            get { return _cards; }
            set { SetProperty(ref _cards, value); }
        }

        private string _cardNum;
        public string CardNum
        {
            get { return _cardNum; }
            set { SetProperty(ref _cardNum, value); }
        }

        private bool _isMyTurn;
        public bool IsMyTurn
        {
            get { return _isMyTurn; }
            set { SetProperty(ref _isMyTurn, value); }
        }

        public OneCardPlayer NextPlayer { get; set; }
        public OneCardPlayer PreviousPlayer { get; set; }
    }
}
