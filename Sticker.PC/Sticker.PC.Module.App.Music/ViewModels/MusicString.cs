using Id3;
using Id3.Frames;
using NAudio.Wave;
using System;

namespace Sticker.PC.Module.App.Music.ViewModels
{
    public class MusicString
    {
        WaveStream waveStream;
        
        public string FileLocationString { get; set; }

        public string TimeString { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public byte[] Picture { get; set; }
        public string Title { get; set; }
        public string ListTitle{ get; set; }

        public MusicString(string fileLocationString)
        {
            FileLocationString = fileLocationString;

            waveStream = new Mp3FileReader(FileLocationString);
            TimeString = string.Format("{0:00}:{1:00}", waveStream.TotalTime.Minutes, waveStream.TotalTime.Seconds);

            try
            {
                var mp3 = new Mp3File(FileLocationString);
                Id3Tag tag = mp3.GetTag(Id3TagFamily.FileStartTag);
                Artist = tag.Artists.Value;
                Album = tag.Album.Value;
                Title = tag.Title.Value;
                PictureFrame p = tag.Pictures[0];
                Picture = p.PictureData;
                ListTitle = Artist + " - " + Title;
            }catch(Exception)
            {
                Artist = "";
                Album = "";
                Title = "내용 없음";
                Picture = null;
                ListTitle = Artist + " - " + Title;
            }   
            
        }
        
    }
}