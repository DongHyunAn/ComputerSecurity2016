using Id3;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Shell32;
using Sticker.PC.Infra.Class;
using Sticker.PC.Infra.Events;
using Sticker.PC.Infra.Service.NetworkService;
using Sticker.PC.Infra.StaticResources;
using Sticker.PC.Module.App.Music.Dummy;
using Sticker.PC.Module.App.Music.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Sticker.PC.Module.App.Music.ViewModels
{
    public class MusicMainViewModel : BindableBase, INavigationAware
    {
        IRegionManager _regionManager;
        IEventAggregator _eventAggregator;
        INetworkService _networkService;

        #region PropertyRegion

        private bool playEnable;
        public bool PlayEnable
        {
            get { return  playEnable; }
            set { SetProperty(ref  playEnable, value); }
        }
        private bool randomCheck;
        public bool RandomCheck
        {
            get { return randomCheck; }
            set { SetProperty(ref randomCheck, value); }
        }
        private bool oneSongCheck;
        public bool OneSongCheck
        {
            get { return oneSongCheck; }
            set { SetProperty(ref oneSongCheck, value); }
        }
        private bool doublePlayCheck;
        public bool DoublePlayCheck
        {
            get { return doublePlayCheck; }
            set { SetProperty(ref doublePlayCheck, value); }
        }
        private string totalTimeString;
        public string TotalTimeString
        {
            get { return totalTimeString; }
            set { SetProperty(ref totalTimeString, value); }
        }

        private double currentSound;
        public double CurrentSound
        {
            get { return currentSound; }
            set
            {
                mp.waveOut.Volume = (float)currentSound/100;
                SetProperty(ref currentSound, value);
            }
        }
        private double totalTime;
        public double TotalTime
        {
            get { return totalTime; }
            set { SetProperty(ref totalTime, value); }
        }

        private string currentTimeString;
        public string CurrentTimeString
        {
            get { return currentTimeString; }
            set { SetProperty(ref currentTimeString, value); }
        }

        private double currentTime;
        public double CurrentTime
        {
            get { return currentTime; }
            set { SetProperty(ref currentTime, value); }
        }
        private string musicName;
        public string MusicName
        {
            get { return musicName; }
            set { SetProperty(ref musicName, value); }
        }

        private string playText;
        public string PlayText
        {
            get { return playText; }
            set { SetProperty(ref playText, value); }
        }

        private int selectMusicIndex = -20;
        public int SelectMusicIndex
        {
            get { return selectMusicIndex; }
            set { SetProperty(ref selectMusicIndex, value); }
        }

        private bool pCheckBox;
        public bool PCheckBox
        {
            get { return pCheckBox; }
            set { SetProperty(ref pCheckBox, value); }
        }
        private ObservableCollection<MusicString> musicList;
        public ObservableCollection<MusicString> MusicList
        {
            get { return musicList; }
            set { SetProperty(ref musicList, value); }
        }

        private double refresh;
        public double Refresh
        {
            get { return refresh; }
            set { SetProperty(ref refresh, value); }
        }

        private double startTime;
        public double StartTime
        {
            get { return startTime; }
            set { SetProperty(ref startTime, value); }
        }
        private double endTime;
        public double EndTime
        {
            get { return endTime; }
            set { SetProperty(ref endTime, value); }
        }
        private bool fouc;
        public bool Fouc
        {
            get { return fouc; }
            set { SetProperty(ref fouc, value); }
        }

        private double muteVolumn;
        public double MuteVolumn
        {
            get { return muteVolumn; }
            set { SetProperty(ref muteVolumn, value); }
        }

        private string artistText;
        public string ArtistText
        {
            get { return artistText; }
            set { SetProperty(ref artistText, value); }
        }
        private string albumText;
        public string AlbumText
        {
            get { return albumText; }
            set { SetProperty(ref albumText, value); }
        }
        private object imageAlbumArt;
        public object ImageAlbumArt
        {
            get { return imageAlbumArt; }
            set { SetProperty(ref imageAlbumArt, value); }
        }
        private object chooseItem;
        public object ChooseItem
        {
            get { return chooseItem; }
            set { SetProperty(ref chooseItem, value); }
        }
        private bool repeatEnable;
        public bool RepeatEnable
        {
            get { return repeatEnable; }
            set { SetProperty(ref repeatEnable, value); }
        }

        private bool shuffleEnable;
        public bool ShuffleEnable
        {
            get { return shuffleEnable; }
            set { SetProperty(ref shuffleEnable, value); }
        }
        private string subAlbumText;
        public string SubAlbumText
        {
            get { return subAlbumText; }
            set { SetProperty(ref subAlbumText, value); }
        }

        #endregion


        #region ICommand prop
        public ICommand MusicControlCommand { get; set; }
        public ICommand SliderDownCommand { get; set; }

        public ICommand OpenCommand { get; set; }
        public ICommand SliderUpCommand { get; set; }
        public ICommand BeforeCommand { get; set; }
        public ICommand StopCommand { get; set; }

        public ICommand NextCommand { get; set; }

        public ICommand DragEnterCommand { get; set; }
        public ICommand DropCommand { get; set; }

        public ICommand DoubleClickCommand { get; set; }

        public ICommand ChangedCommand { get; set; }
        public ICommand LocationCommand { get; set; }
        public ICommand ViewLoadedCommand { get; set; }

        public ICommand MuteVolumnCommand { get; set; }
        #endregion

        MusicPlayerClass mp;
        DispatcherTimer Timer;

        public MusicMainViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, INetworkService networkService)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _networkService = networkService;

            MusicList = new ObservableCollection<MusicString>();
            mp = new MusicPlayerClass();
            Timer = new DispatcherTimer();

            RepeatEnable = false;
            ShuffleEnable = false;
            
            MusicName = "음악을 재생해주세요.";
            AlbumText = "";
            TotalTime = 1;
            CurrentTime = 0;
            CurrentSound = 0;
            mp.waveOut.Volume = 0;
            CurrentTimeString = "00:00";
            TotalTimeString = "00:00";
            PlayText = "재생";
            PCheckBox = true;
            PlayEnable = false;

            Timer.Interval = TimeSpan.FromMilliseconds(500);// 0.5초 마다 호출..
            Timer.Tick += new EventHandler(Timer_Tick);
            mp.waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
            BeforeCommand = new DelegateCommand(Before);
            StopCommand = new DelegateCommand(Stop);
            NextCommand = new DelegateCommand(Next);
            OpenCommand = new DelegateCommand(Open);
            ChangedCommand = new DelegateCommand(SliderChanged);
            SliderDownCommand = new DelegateCommand(SliderDown);
            SliderUpCommand = new DelegateCommand(SliderUp);
            LocationCommand = new DelegateCommand(Location);
            MuteVolumnCommand = new DelegateCommand<object>(Mute);
            
            ThreadPool.QueueUserWorkItem(MusicPlayerInit);
            
            ViewLoadedCommand = new DelegateCommand(ViewLoadedCommandImply);
        }

        private void SeekControl(string obj)
        {
            string[] tokenize = obj.Split('_');
            if(tokenize[0].Equals("sound"))
            {
                SoundSeek(tokenize[1]);
                
            }
            else if(tokenize[0].Equals("player"))
            {
                PlayerSeek(tokenize[1]);
            }
                    
        }

        private void PlayerSeek(string token)
        {
            Timer.Stop();
            string strTmp = Regex.Replace(token, @"\D", "");
            
            if(PlayEnable==true)
            {
                mp.pauseMusic();          
                CurrentTime = double.Parse(strTmp);
                mp.setPosition(CurrentTime);
                _networkService.SendMessageToPlayer(_networkService.GetMasterPlayer(), "Music_Timer_" + (int)CurrentTime + "_" + (int)TotalTime + "_" + CurrentTimeString);
                mp.resumeMusic();
            }
            Timer.Start();

        }

        private void SoundSeek(string token)
        {
            string strTmp = Regex.Replace(token, @"\D", "");
            CurrentSound = double.Parse(strTmp);
            mp.waveOut.Volume = float.Parse(strTmp) / 100;
        }

        private void Open()
        {
            SelectMusicIndex = 0;

            if(MusicList.Count==0)
            {
                return;
            }

            ChooseItem = MusicList[SelectMusicIndex];
            ImageAlbumArt = (ChooseItem as MusicString).Picture;
            ArtistText = (ChooseItem as MusicString).Artist;
            AlbumText = (ChooseItem as MusicString).Album;
            SubAlbumText = (ChooseItem as MusicString).Album;

        }

        private void MusicControlKey(KeyEvent.MusicKeyType obj)
        {
            switch (obj)
            {
                case KeyEvent.MusicKeyType.PLAY:
                    Play();
                    break;
                case KeyEvent.MusicKeyType.STOP:
                    Stop();
                    break;
                case KeyEvent.MusicKeyType.NEXT:
                    Next();
                    break;
                case KeyEvent.MusicKeyType.BEFORE:
                    Before();
                    break;
                case KeyEvent.MusicKeyType.LISTUP:
                    SelectIndexMove(-1);
                    break;
                case KeyEvent.MusicKeyType.LISTDOWN:
                    SelectIndexMove(1);
                    break;
                case KeyEvent.MusicKeyType.REPEAT:
                    Repeat();
                    break;
                case KeyEvent.MusicKeyType.SHUFFLE:
                    Shuffle();
                    break;
                default:
                    break;
            }
        }

        private void Shuffle()
        {
            if (ShuffleEnable == false)
            {
                ShuffleEnable = true;
            }
            else
            {
                ShuffleEnable = false;
            }
        }

        private void Repeat()
        {
            if(RepeatEnable == false)
            {
                RepeatEnable = true;
            }
            else
            {
                RepeatEnable = false;
            }
        }

        private void SelectIndexMove(int index)
        {
            
            int selectindex = SelectMusicIndex + index;
            if (selectindex > MusicList.Count - 1)
            {
                selectindex = 0;
            }
            if (selectindex < 0)
            {
                selectindex = MusicList.Count - 1;
            }
            SelectMusicIndex = selectindex;
            ChooseItem = MusicList[SelectMusicIndex];
            SubAlbumText = (ChooseItem as MusicString).Album;
            ArtistText = (ChooseItem as MusicString).Artist;
            ImageAlbumArt = (ChooseItem as MusicString).Picture;
        }

        public void Mute(object obj)
        {
            double tempSound = CurrentSound;
            isMute = true;
            if (isMute == true)
            {
                mp.waveOut.Volume = 0;
                tempSound = 0;
                isMute = false;
            }
        }

        private void ViewLoadedCommandImply()
        {
            if (Timer != null)
            {
                Timer.Start();
            }
        }

        private void MusicPlayerInit(object state)
        {
            string userName = Environment.UserName;
            string dirPath = @"C:\Users\" + @userName + @"\Music";

            if (!Directory.Exists(dirPath))
            {
                return;
            }

            FindFileInDirectory(dirPath);

        }
        public void FindFileInDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                if (fileName.LastIndexOf(".mp3") == fileName.Length - 4)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(()=> {
                        MusicList.Add(new MusicString(fileName));

                        if (MusicList.Count == 1)
                        {
                            ChooseItem = MusicList[0];
                            mp.setMp((ChooseItem as MusicString).FileLocationString);
                        }
                    }), null);
                }
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                FindFileInDirectory(subdirectory);
        }

        public bool isMute = false;
        private void Timer_Tick(object sender, EventArgs e)
        {
            
            CurrentTime = mp.getCurrentTimeSecond();
            CurrentTimeString = mp.getCurrentClockTime();
            TotalTimeString = mp.getToatlClockTime();
            _networkService.SendMessageToPlayer(_networkService.GetMasterPlayer(), "Music_Timer_"+ (int)CurrentTime+"_"+(int)TotalTime+"_"+CurrentTimeString);
        }

        private void Location()
        {
            Refresh += 1;
            Refresh -= 1;
        }
        
        private void WaveOut_PlaybackStopped(object sender, NAudio.Wave.StoppedEventArgs e)
        {
            if (!CurrentTimeString.Equals(TotalTimeString))
            {
                return;
            }
            mp.waveOut.Stop();
            mp.waveStream.Position = 0;
            if (RepeatEnable != true)
            {
                Next();
            }
            else
            {
                mp.waveOut.Play();
            }
        }


        private void SliderUp()
        {
            if (mp.waveStream == null) return;
            mp.setPosition(CurrentTime);
            mp.resumeMusic();
            Timer.Start();
        }
        private void SliderDown()
        {
            if (mp.waveStream == null) return;
            mp.pauseMusic();
            Timer.Stop();
        }
        private void SliderChanged()
        {
            if (mp.waveStream == null) return;
            int minute = (int)CurrentTime / 60;
            int second = (int)CurrentTime - 60 * minute;

            TotalTimeString = string.Format("{0:00}:{1:00}", minute, second) + "/" + mp.getToatlClockTime();

        }

        private void Next()
        {
            
            if (MusicList.Count == 0) return;

            RandomIndexMove(ShuffleEnable);

            SelectIndexMove(1);
           
            CurrentTime = mp.setStopSecond();
            CurrentTimeString = "00:00";

            ChooseItem = MusicList[SelectMusicIndex];
                if (PlayEnable==true)
                {
                    TotalTime = mp.nextPlay((ChooseItem as MusicString).FileLocationString, true);
                    
                }
                else
                {
                    TotalTime = mp.nextPlay((ChooseItem as MusicString).FileLocationString, false);
                }
            beforeMusic = (ChooseItem as MusicString).FileLocationString;
            
            EditMusicInfo();
        }

        void RandomIndexMove(bool isShuffle)
        {
            if (isShuffle == true)
            {
                Random rnd = new Random();
                int randomIndex = rnd.Next(MusicList.Count);
                while (SelectMusicIndex == randomIndex)
                {
                    randomIndex = rnd.Next(MusicList.Count);
                }
                SelectMusicIndex = randomIndex;

            }
        }

        private void Stop()
        {
            if (mp.waveStream == null) return;
           
            PlayText = "재생";
            mp.stopMusic();
            PlayEnable = false;
            CurrentTime = mp.setStopSecond();
            CurrentTimeString = "00:00";
            _networkService.SendMessageToPlayer(_networkService.GetMasterPlayer(), "Music_Timer_" + 0 + "_" + (int)TotalTime + "_" + CurrentTimeString);
            Timer.Stop();
        }

        string beforeMusic;
        private void Play()
        {
            if (PlayEnable == false)
            {
                if (beforeMusic != null && !beforeMusic.Equals((ChooseItem as MusicString).FileLocationString))
                {
                    mp.waveStream.Dispose();
                    mp.isPlaying = false;
                }
                TotalTime = mp.playMusic((ChooseItem as MusicString).FileLocationString);
                EditMusicInfo();

                Timer.Start();
                PlayEnable = true;
                beforeMusic = (ChooseItem as MusicString).FileLocationString;
            }
            else
            {
                Timer.Stop();
                CurrentTime = mp.setPauseSecond();
                TotalTimeString = mp.getToatlClockTime();
                mp.pauseMusic();
                mp.isPlaying = true;
                PlayEnable = false;

            }

        }

        private void Before()
        {

            if (MusicList.Count == 0) return;

            RandomIndexMove(ShuffleEnable);

            SelectIndexMove(-1);

            CurrentTime = mp.setStopSecond();
            CurrentTimeString = "00:00";
            ChooseItem = MusicList[SelectMusicIndex];

            if (PlayEnable == true)
            {
                TotalTime = mp.nextPlay((ChooseItem as MusicString).FileLocationString, true);
            }
            else
            {
                TotalTime = mp.nextPlay((ChooseItem as MusicString).FileLocationString, false);
            }
            EditMusicInfo();
            beforeMusic = (ChooseItem as MusicString).FileLocationString;
           
        }

        private void EditMusicInfo()
        {
            MusicName = (ChooseItem as MusicString).Title;
            ImageAlbumArt = (ChooseItem as MusicString).Picture;
            ArtistText = (ChooseItem as MusicString).Artist;
            AlbumText = (ChooseItem as MusicString).Album;
            SubAlbumText = (ChooseItem as MusicString).Album;
            TotalTimeString = mp.getToatlClockTime();
            _networkService.SendMessageToPlayer(_networkService.GetMasterPlayer(), "Music_Controll_" + MusicName + "_" + ArtistText+"_"+ TotalTimeString);
            _networkService.SendMessageToPlayer(_networkService.GetMasterPlayer(), "Music_Timer_" + (int)CurrentTime + "_" + (int)TotalTime + "_" + CurrentTimeString);
        }

        private void KeyDownEventImply(KeyEvent.KeyType obj)
        {
            switch (obj)
            {
                case KeyEvent.KeyType.CANCEL:
                    mp.isPlaying = false;
                    mp.stopMusic();
                    //CurrentTime = mp.setStopSecond();
                    Timer.Stop();
                    _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("AppList", UriKind.Relative));
                    break;
                default:
                    break;
            }
        }

        #region INavigationAware Implements

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<MasterKeyDownEvent>().Subscribe(KeyDownEventImply);
            _eventAggregator.GetEvent<MusicKeYDownEvent>().Subscribe(MusicControlKey);
            _eventAggregator.GetEvent<MusicControllerKeyEvent>().Subscribe(SeekControl);
            _networkService.SetAppController(_networkService.GetMasterPlayer(), NetworkService.ControllerType.Music);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<MasterKeyDownEvent>().Unsubscribe(KeyDownEventImply);
            _eventAggregator.GetEvent<MusicKeYDownEvent>().Unsubscribe(MusicControlKey);
            _eventAggregator.GetEvent<MusicControllerKeyEvent>().Unsubscribe(SeekControl);
        } 
        #endregion
    }
}
