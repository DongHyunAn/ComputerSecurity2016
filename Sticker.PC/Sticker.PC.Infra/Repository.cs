using Prism.Mvvm;
using Sticker.PC.Infra.Class.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Sticker.PC.Infra
{
    public class Repository : BindableBase
    {
        #region Singleton
        private static Repository _instance = new Repository();

        public static Repository Instance
        {
            get
            {
                return _instance;
            }
        }

        private Repository()
        {

        }
        #endregion

        private ObservableCollection<AppInfo> _appInfoList;
        public ObservableCollection<AppInfo> AppInfoList
        {
            get {
                if (_appInfoList == null)
                {
                    MakeAppInfoList();
                }
                return _appInfoList;
            }
            set { SetProperty(ref _appInfoList, value); }
        }

        private void MakeAppInfoList()
        {
            _appInfoList = new ObservableCollection<AppInfo>();

            _appInfoList.Add(new AppInfo() { ModuleUri = new Uri("MusicMain", UriKind.Relative), Title = "Music", Thumbnail = "/Sticker.PC.Infra;component/Resources/Images/Main/img_music.png"});
            _appInfoList.Add(new AppInfo() { ModuleUri = new Uri("GalleryMain", UriKind.Relative), Title = "Photos", Thumbnail = "/Sticker.PC.Infra;component/Resources/Images/Main/img_photos.png" });
            _appInfoList.Add(new AppInfo() { ModuleUri = null, Title = "Web Browser", Thumbnail = "/Sticker.PC.Infra;component/Resources/Images/Main/img_web browser.png" });
            _appInfoList.Add(new AppInfo() { ModuleUri = null, Title = "낱말맞추기", Thumbnail = "/Sticker.PC.Infra;component/Resources/Images/Main/img_game3.png" });
            _appInfoList.Add(new AppInfo() { ModuleUri = new Uri("RPSLogo", UriKind.Relative), Title = "가위바위보", Thumbnail = "/Sticker.PC.Infra;component/Resources/Images/Main/img_rps.png"});
            _appInfoList.Add(new AppInfo() { ModuleUri = new Uri("OneCardLogo", UriKind.Relative), Title = "원카드", Thumbnail = "/Sticker.PC.Infra;component/Resources/Images/Main/img_onecard.png" });
            _appInfoList.Add(new AppInfo() { ModuleUri = new Uri("ClockMusic", UriKind.Relative), Title = "Clock", Thumbnail = "/Sticker.PC.Infra;component/Resources/Images/Main/img_time.png"});
            _appInfoList.Add(new AppInfo() { ModuleUri = null, Title = "Youtube", Thumbnail = "/Sticker.PC.Infra;component/Resources/Images/Main/img_youtube.png" });
            _appInfoList.Add(new AppInfo() { ModuleUri = new Uri("RadioMain", UriKind.Relative), Title = "Radio", Thumbnail = "/Sticker.PC.Infra;component/Resources/Images/Main/img_radio.png"});
        }

        private ObservableCollection<GalleryInfoWrapper> _galleryImagePageList;
        public ObservableCollection<GalleryInfoWrapper> GalleryImagePageList
        {
            get {
                if(_galleryImagePageList == null)
                {
                    MakeGalleryImageList();
                }
                return _galleryImagePageList; }
            set { SetProperty(ref _galleryImagePageList, value); }
        }

        private void MakeGalleryImageList()
        {
            _galleryImagePageList = new ObservableCollection<GalleryInfoWrapper>();

            for (int i = 0; i < 2 ; i++)
            {
                List<GalleryFileInfo> list = new List<GalleryFileInfo>();

                if(i%2==0)
                {
                    for (int j = 0; j < 21; j++)
                    {
                        list.Add(new GalleryFileInfo() { ImageFilePath = "/Sticker.PC.Infra;component/Resources/Images/Gallery/Sample/GalleryItem_" + (j + 1).ToString() + ".png" });
                    }
                }
                else
                {
                    for (int j = 21; j >= 3; j--)
                    {
                        list.Add(new GalleryFileInfo() { ImageFilePath = "/Sticker.PC.Infra;component/Resources/Images/Gallery/Sample/GalleryItem_" + (j + 1).ToString() + ".png" });
                    }
                }

                _galleryImagePageList.Add(new GalleryInfoWrapper()
                {
                    GalleryFileList = new ObservableCollection<GalleryFileInfo>(list)
                });
            }
        }

        private ObservableCollection<ProfileInfo> _profileImageList;
        public ObservableCollection<ProfileInfo> ProfileImageList
        {
            get {
                if(_profileImageList==null)
                {
                    MakeProfileImageList();
                }
                return _profileImageList;
            }
            set { SetProperty(ref _profileImageList, value); }
        }

        private void MakeProfileImageList()
        {
            _profileImageList = new ObservableCollection<ProfileInfo>();

            for(int i=0;i<9;i++)
            {
                _profileImageList.Add(new ProfileInfo(i + 1));
            }
        }
        private ObservableCollection<MusicFileInfo> _musicList;
        public ObservableCollection<MusicFileInfo> MusicList
        {
            get {
                if(_musicList == null)
                {
                    MakeMusicList();
                }
                return _musicList;
            }
            set { SetProperty(ref _musicList, value); }
        }

        private void MakeMusicList()
        {
            _musicList = new ObservableCollection<MusicFileInfo>();
        }



    }
}
