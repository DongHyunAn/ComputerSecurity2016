using Prism.Mvvm;
using System;
using System.Windows.Threading;
using System.Globalization;
using System.Windows.Input;
using Prism.Commands;
using System.Collections.ObjectModel;
using Sticker.PC.Infra.Class.Model;
using Sticker.PC.Infra;
using NAudio.Wave;
using System.Windows.Controls;
using System.Collections.Generic;
using Prism.Events;
using Sticker.PC.Infra.Events;
using Prism.Regions;
using Sticker.PC.Infra.StaticResources;
using Sticker.PC.Module.App.Clock.Views;
using System.IO;
using System.Linq;
using Sticker.PC.Module.App.Clock.Class;

namespace Sticker.PC.Module.App.Clock.ViewModels
{
    public class ClockMusicViewModel : BindableBase
    {
        IRegionManager _regionManager;
        MusicPlayer _player;
        DispatcherTimer _timer;

        #region Property

        private string currentTimeView;
        public string CurrentTimeView
        {
            get { return currentTimeView; }
            set { SetProperty(ref currentTimeView, value); }
        }
        private string currentDateView;
        public string CurrentDateView
        {
            get { return currentDateView; }
            set { SetProperty(ref currentDateView, value); }
        }

        private ObservableCollection<MusicFileInfo> _musicList;
        public ObservableCollection<MusicFileInfo> MusicList
        {
            get { return _musicList; }
            set { SetProperty(ref _musicList, value); }
        }
        #endregion

        private bool _isMusicPlayerRunning;
        public bool IsMusicPlayerRunning
        {
            get { return _isMusicPlayerRunning; }
            set
            {
               SetProperty(ref _isMusicPlayerRunning, value);
               PlayerIconViewer = IsMusicPlayerRunning & IsPlaying;
            }
        }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                SetProperty(ref _isPlaying, value);
                PlayerIconViewer = IsMusicPlayerRunning & IsPlaying;
            }
        }

        private bool _playerIconViewer;
        public bool PlayerIconViewer
        {
            get { return _playerIconViewer; }
            set { SetProperty(ref _playerIconViewer, value); }
        }

        private string _playMusicInfo;
        public string PlayMusicInfo
        {
            get { return _playMusicInfo; }
            set { SetProperty(ref _playMusicInfo, value); }
        }

        public ICommand ViewLoadedCommand { get; set; }

        public ClockMusicViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _regionManager = regionManager;

            _timer = new DispatcherTimer();
            InitTimer();
            
            IsPlaying = false;
            IsMusicPlayerRunning = false;
            PlayMusicInfo = "";

            _player = new MusicPlayer();
            _player.AttachPlayInfoChangeHandler(new Action<string>((string obj)=>
            {
                PlayMusicInfo = obj;
            }));

            eventAggregator.GetEvent<MasterKeyDownEvent>().Subscribe(KeyDownEventImply);
            ViewLoadedCommand = new DelegateCommand(ViewLoadedCommandImply);
        }

        private void ViewLoadedCommandImply()
        {
            if(_timer != null)
            {
                _timer.Start();
            }
        }

        #region Clock
        private void InitTimer()
        {
            _timer.Interval = TimeSpan.FromSeconds(0.1);
            _timer.Tick += (object sender, EventArgs e) => 
            {
                CurrentTimeView = DateTime.Now.ToString(@"hh \: mm \: ss");
            };
            _timer.Tick += (object sender, EventArgs e) =>
            {
                CurrentTimeView = DateTime.Now.ToString(@"hh \: mm \: ss");
                CurrentDateView = DateTime.Now.ToString(@"ddd   MMMM   dd'th'   yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            };
            _timer.Start();
        }
        
        #endregion

        private void KeyDownEventImply(KeyEvent.KeyType obj)
        {
            if (_regionManager.Regions[RegionName.MainRegion].ActiveViews.Count() != 0)
            {
                if (!(_regionManager.Regions[RegionName.MainRegion].ActiveViews.First() is ClockMusic))
                {
                    return;
                }
            }

            switch (obj)
            {
                case KeyEvent.KeyType.UP:
                    break;
                case KeyEvent.KeyType.DOWN:
                    break;
                case KeyEvent.KeyType.RIGHT:
                    if(IsMusicPlayerRunning)
                    {
                        _player.Next();
                        IsPlaying = _player.IsPlayable;
                    }
                    break;
                case KeyEvent.KeyType.LEFT:
                    if (IsMusicPlayerRunning)
                    {
                        _player.Previous();
                        IsPlaying = _player.IsPlayable;
                    }
                    break;
                case KeyEvent.KeyType.SELECT:
                    {
                        if(!IsPlaying)
                        {
                            _player.Play();
                            IsPlaying = _player.IsPlayable;
                            IsMusicPlayerRunning = true;
                        }
                        else
                        {
                            _player.Stop();
                            IsPlaying = false;
                        }
                    }
                    break;
                case KeyEvent.KeyType.CANCEL:
                    _player.DisposePlayer();
                    IsPlaying = false;
                    IsMusicPlayerRunning = false;

                    if(_timer.IsEnabled)
                    {
                        _timer.Stop();
                    }

                    _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("AppList", UriKind.Relative));
                    break;
                default:
                    break;
            }
        }
    }
}
