using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Sticker.PC.Infra.Events;
using Sticker.PC.Infra.StaticResources;
using Sticker.PC.Module.App.Radio.Class;
using Sticker.PC.Module.App.Radio.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Sticker.PC.Module.App.Radio.ViewModels
{
    class RadioMainViewModel : BindableBase
    {
        IRegionManager _regionManager;
        private MediaState mediaState;
        private RadioChannel selectedRadioChannel;
        int tmp = 0;
        int ltmp = 1;
        public ObservableCollection<RadioChannel> RadioChannels { get; set; }
        public string LogoFilePath { get; set; }

        public ObservableCollection<string> LogoFileList { get; set; }
        private List<string> channelTextList;
        public List<string> ChannelTextList
        {
            get { return channelTextList; }
            set { SetProperty(ref channelTextList, value); }
        }

        private string channelText;
        public string ChannelText
        {
            get { return channelText; }
            set { SetProperty(ref channelText, value); }
        }
        private double volume;
        public double Volume
        {
            get { return volume; }
            set { SetProperty(ref volume, value); }
        }
        private double volumeImage;
        public double VolumeImage
        {
            get { return volumeImage; }
            set { SetProperty(ref volumeImage, value); }
        }
        private bool isPlaying;
        public bool IsPlaying
        {
            get { return isPlaying; }
            set { SetProperty(ref isPlaying, value); }
        }
        private bool isWorking;
        public bool IsWorking
        {
            get { return isWorking; }
            set { SetProperty(ref isWorking, value); }
        }
        public ObservableCollection<bool> IsSelectedChannelList { get; set; }
        public MediaState MediaState
        {
            get { return mediaState; }
            set { SetProperty(ref mediaState, value); }
        }
        private string selectedLogoFile;
        public string SelectedLogoFile
        {
            get { return selectedLogoFile; }
            set { SetProperty(ref selectedLogoFile, value); }
        }
        public RadioChannel SelectedRadioChannel {
            get { return selectedRadioChannel; }
            set
            {
                selectedRadioChannel = value;
                OnPropertyChanged();
                StartPlayback();
            }
        }

        private void StartPlayback()
        {
            MediaState = MediaState.Play;
            IsWorking = true;
            IsPlaying = true;
        }
        public void BufferingEnded()
        {
            IsWorking = false;
        }
        private void Init()
        {
            tmp = 0;
            ltmp = 1;
            IsPlaying = false;
            IsWorking = false;
            SelectedRadioChannel = null;
            SelectedLogoFile = LogoFileList[0];
            VolumeImage = 5;
            Volume = 0.5;
            for (int i = 0; i < 10; i++)
            {
                IsSelectedChannelList.Add(true);
            }
        }

        
        private ObservableCollection<RadioChannel> LoadRadioChannels()
        {
            return new ObservableCollection<RadioChannel>
            {
                new RadioChannel
                {
                    ChannelName = "SBS PowerFM\n      (청주)",
                    ChannelUri = "mms://211.224.129.152/joyfm_live"
                },
                //new RadioChannel
                //{
                //    ChannelName = "SBS PowerFM\n      (제주)",
                //    ChannelUri = "mms://121.254.230.3/FMLIVE"
                //},
                //new RadioChannel
                //{
                //    ChannelName = "SBS PowerFM\n      (전주)",
                //    ChannelUri = "mms://114.108.140.39/magicfm_live"
                //},
                //new RadioChannel
                //{
                //    ChannelName = "SBS 청주",
                //    ChannelUri = "mms://211.224.129.152/joyfm_live"
                //},

                new RadioChannel
                {
                    ChannelName = "MBC 전주 AM",
                    ChannelUri = "mms://210.105.237.100/mbcam"
                },

                new RadioChannel
                {
                    ChannelName = "MBC 부산 AM",
                    ChannelUri = "mms://58.231.196.73/busanmbc-am-onair-20120228"
                },
                new RadioChannel
                {
                    ChannelName = "Busan e-FM",
                    ChannelUri = "mms://115.68.15.116/efm"
                },
                new RadioChannel
                {
                    ChannelName = "CBS 제주",
                    ChannelUri = "mms://vod.cbs.co.kr/jjcbs"
                },
                new RadioChannel
                {
                    ChannelName = "CBS 부산",
                    ChannelUri = "mms://media.biointernet.com/fm1021"
                },
                new RadioChannel
                {
                    ChannelName = "FEBC 제주",
                    ChannelUri = "mms://live.febc.net/jeju_febc"
                },
                new RadioChannel
                {
                    ChannelName = "FEBC 서울",
                    ChannelUri = "mms://live.febc.net/LiveFm"
                },
                new RadioChannel
                {
                    ChannelName = "World Radio\n      Ch1",
                    ChannelUri = "mms://live.kbs.gscdn.com/world_rki1"
                },
                //new RadioChannel
                //{
                //    ChannelName = "World Radio Ch2",
                //    ChannelUri = "mms://live.kbs.gscdn.com/world_rki2"
                //},
                //new RadioChannel
                //{
                //    ChannelName = "Wolrd Radio Music",
                //    ChannelUri = "mms://live.kbs.gscdn.com/world_rki3"
                //},
                new RadioChannel
                {
                    ChannelName = "PBC 광주",
                    ChannelUri = "mms://183.105.65.21/onair"
                }
            };
        }

        public RadioMainViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _regionManager = regionManager;
            RadioChannels = LoadRadioChannels();
            LogoFileList = new ObservableCollection<string>();
            for (int i = 0; i < 11; i++)
            {
                LogoFilePath = "/Sticker.PC.Infra;component/Resources/Images/Radio/Logo/"+i+".jpg";
                LogoFileList.Add(LogoFilePath);
            }
            IsSelectedChannelList = new ObservableCollection<bool>();
            ChannelTextList = new List<string>();
            
            Init();
            eventAggregator.GetEvent<MasterKeyDownEvent>().Subscribe(KeyDownEventImply);
            for (int i = 0; i < 10; i++)
            {
                ChannelTextList.Add(RadioChannels[i].ChannelName);
            }
        }

        

        
        

        private void KeyDownEventImply(KeyEvent.KeyType obj)
        {
            if (_regionManager.Regions[RegionName.MainRegion].ActiveViews.Count() != 0)
            {
                if (!(_regionManager.Regions[RegionName.MainRegion].ActiveViews.First() is RadioMain))
                {
                    return;
                }
            }
            
            switch (obj)
            {
                case KeyEvent.KeyType.UP:
                    if(Volume >= 1)
                    {
                        Volume = 1;
                        VolumeImage = 10;
                    }
                    else
                    {
                        Volume = Volume + 0.1;
                        VolumeImage = VolumeImage + 1;
                    }
                    break;
                case KeyEvent.KeyType.DOWN:
                    if (Volume <= 0)
                    {
                        Volume = 0;
                        VolumeImage = 0;
                    }
                    else
                    {
                        Volume = Volume - 0.1;
                        VolumeImage = VolumeImage - 1;
                    }
                    break;
                case KeyEvent.KeyType.RIGHT:
                    if(SelectedRadioChannel == null)
                    {
                        SelectedRadioChannel = RadioChannels[0];
                        SelectedLogoFile = LogoFileList[1];
                        IsSelectedChannelList[0] = false;
                    }
                    else if(SelectedRadioChannel == RadioChannels[RadioChannels.Count-1])
                    {
                        
                        SelectedRadioChannel = RadioChannels[0];
                        SelectedLogoFile = LogoFileList[1];
                        IsSelectedChannelList[0] = false;
                        IsSelectedChannelList[tmp] = true;
                        tmp = 0;
                        ltmp = 1;
                    }
                    else
                    {
                        ltmp = ltmp + 1;
                        tmp = tmp + 1;
                        SelectedRadioChannel = RadioChannels[tmp];
                        SelectedLogoFile = LogoFileList[ltmp];
                        IsSelectedChannelList[tmp - 1] = true;
                        IsSelectedChannelList[tmp] = false;
                        
                    }
                    ChannelText = RadioChannels[tmp].ChannelName;
                    break;
                case KeyEvent.KeyType.LEFT:
                    if (SelectedRadioChannel == null)
                    {
                        SelectedRadioChannel = RadioChannels[0];
                        SelectedLogoFile = LogoFileList[1];
                        IsSelectedChannelList[0] = false;
                    }
                    else if (SelectedRadioChannel == RadioChannels[0])
                    {
                        SelectedRadioChannel = RadioChannels.Last();
                        SelectedLogoFile = LogoFileList.Last();
                        IsSelectedChannelList[0] = true;
                        tmp = RadioChannels.Count - 1;
                        ltmp = LogoFileList.Count - 1;
                        IsSelectedChannelList[tmp] = false;
                    }
                    else
                    {
                        tmp = tmp - 1;
                        ltmp = ltmp - 1;
                        SelectedRadioChannel = RadioChannels[tmp];
                        SelectedLogoFile = LogoFileList[ltmp];
                        IsSelectedChannelList[tmp + 1] = true;
                        IsSelectedChannelList[tmp] = false;
                    }
                    ChannelText = RadioChannels[tmp].ChannelName;
                    break;
                case KeyEvent.KeyType.SELECT:
                    StartPlayback();
                    if (SelectedRadioChannel == null)
                    {
                        SelectedRadioChannel = RadioChannels[0];
                        SelectedLogoFile = LogoFileList[1];
                        IsSelectedChannelList[0] = false;
                    }
                    break;
                case KeyEvent.KeyType.CANCEL:
                    Init();
                    for (int i = 0; i < 10; i++)
                    {
                        IsSelectedChannelList[i] = true;
                    }
                    _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("AppList", UriKind.Relative));
                    break;
                default:
                    break;
            }
        }
    }

}
