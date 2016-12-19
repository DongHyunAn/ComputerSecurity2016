using Sticker.PC.Infra.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Prism.Mvvm;
using System.Windows;

namespace Sticker.PC.Module.App.RockPaperScissors.Class
{
    public class RPSPlayer : BindableBase
    {
        Player _player;

        public enum RPS
        {
            Rock,
            Paper,
            Scissors,
            None
        }

        #region Properties
        public RPS RpsType { get; set; }

        private string _nickname;
        public string Nickname
        {
            get { return _nickname; }
            set { SetProperty(ref _nickname, value); }
        }

        private string _thumbnailUriPath;
        public string ThumbnailUriPath
        {
            get { return _thumbnailUriPath; }
            set { SetProperty(ref _thumbnailUriPath, value); }
        }

        private string _looserUriPath;
        public string LooserUriPath
        {
            get { return _looserUriPath; }
            set { SetProperty(ref _looserUriPath, value); }
        }

        private bool _isNotExist;
        public bool IsNotExist
        {
            get { return _isNotExist; }
            set { SetProperty(ref _isNotExist, value); }
        }

        private Visibility _backCardVisible;
        public Visibility IsBackCardVisible
        {
            get { return _backCardVisible; }
            set { SetProperty(ref _backCardVisible, value); }
        }

        private string _selectedCardPath;
        public string SelectedCardPath
        {
            get { return _selectedCardPath; }
            set { SetProperty(ref _selectedCardPath, value); }
        }

        private CardItem _playerCardItem;
        public CardItem PlayerCardItem
        {
            get { return _playerCardItem; }
            set { SetProperty(ref _playerCardItem, value); }
        }

        private Visibility _playerCardItemVisible;
        public Visibility PlayerCardItemVisible
        {
            get { return _playerCardItemVisible; }
            set { SetProperty(ref _playerCardItemVisible, value); }
        }

        #endregion

        public RPSPlayer()
        {
            init();
        }

        public void init()
        {
            _player = null;
            RpsType = RPS.None;
            Nickname = "";
            ThumbnailUriPath = "";
            LooserUriPath = "";

            IsNotExist = true;
            
            IsBackCardVisible = Visibility.Hidden;
            PlayerCardItemVisible = Visibility.Hidden;

            SelectedCardPath = "";
            PlayerCardItem = null;
        }

        public void SetPlayer(Player player)
        {
            if(player==null)
            {
                init();
            }
            else
            {
                _player = player;
                Nickname = _player.Nickname;

                ThumbnailUriPath = "/Sticker.PC.Infra;component/Resources/Images/RPS/Avatar/player_" + _player.ThumbnailNum.ToString() + ".png";
                LooserUriPath = "/Sticker.PC.Infra;component/Resources/Images/RPS/Avatar/loser_" + _player.ThumbnailNum.ToString() + ".png";
                IsNotExist = false;
            }
        }

        public void SetSelectedCard(RPS type)
        {
            IsBackCardVisible = Visibility.Visible;
            PlayerCardItem = new CardItem("/Sticker.PC.Infra;component/Resources/Images/RPS/img_card_back.png");
            RpsType = type;

            switch (type)
            {
                case RPS.Rock: SelectedCardPath = "/Sticker.PC.Infra;component/Resources/Images/RPS/img_card_Rock.png"; break;
                case RPS.Paper: SelectedCardPath = "/Sticker.PC.Infra;component/Resources/Images/RPS/img_card_paper.png"; break;
                case RPS.Scissors: SelectedCardPath = "/Sticker.PC.Infra;component/Resources/Images/RPS/img_card_scissors.png"; break;
                case RPS.None:
                    {
                        SelectedCardPath = "";
                        IsBackCardVisible = Visibility.Hidden;
                        PlayerCardItemVisible = Visibility.Hidden;
                    }
                    break;
                default: break;
            }
        }

        public void SetBackCardVisibleHidden()
        {
            if (RpsType != RPS.None)
            {
                PlayerCardItemVisible = Visibility.Visible;
                IsBackCardVisible = Visibility.Hidden;

                PlayerCardItem = new CardItem(SelectedCardPath);
            }
        }

        public Player GetPlayer()
        {
            return _player;
        }
    }
}
