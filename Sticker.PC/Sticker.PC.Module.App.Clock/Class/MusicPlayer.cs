using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Sticker.PC.Module.App.Clock.Class
{
    public class MusicPlayer
    {
        public List<string> MP3FileList { get; set; }
        public int NowPlayPosition;

        public bool IsPlaying;
        public bool IsPlayable;

        WaveStream mainOutputStream;
        WaveChannel32 volumnStream;
        WaveOut player;

        Dispatcher originalDispatcher;

        public MusicPlayer()
        {
            MusicPlayerInit();
            originalDispatcher = Dispatcher.CurrentDispatcher;
        }

        ~MusicPlayer()
        {
            DisposePlayer();
        }

        private void MusicPlayerInit()
        {
            string userName = Environment.UserName;
            string dirPath = @"C:\Users\" + @userName + @"\Music";

            if (!Directory.Exists(dirPath))
            {
                return;
            }

            MP3FileList = new List<string>();
            FindFileInDirectory(dirPath);

            NowPlayPosition = 0;
            IsPlayable = true;
            if (MP3FileList.Count==0)
            {
                IsPlayable = false;
            }
            IsPlaying = false;
        }

        public void FindFileInDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                if (fileName.LastIndexOf(".mp3") == fileName.Length - 4)
                {
                    MP3FileList.Add(fileName);
                }
            }
            
            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                FindFileInDirectory(subdirectory);
        }

        public void DisposePlayer()
        {
            IsPlaying = false;
            try
            {
                mainOutputStream.Dispose();
                volumnStream.Dispose();
                player.Dispose();

                if (_playThread != null)
                {
                    _playThread.Abort();
                }
            }
            catch
            {
                return;
            }
        }

        Thread _playThread;

        Action<string> playInfoChange;

        public void AttachPlayInfoChangeHandler(Action<string> handler)
        {
            playInfoChange = handler;
        }

        public void Play()
        {
            if(IsPlayable)
            {
                if(IsPlaying)
                {
                    player.Play();
                    return;
                }

                mainOutputStream = new Mp3FileReader(MP3FileList[NowPlayPosition]);

                if(playInfoChange!=null)
                {
                    string userName = Environment.UserName;
                    string dirPath = @"C:\Users\" + @userName + @"\Music\";
                    playInfoChange(MP3FileList[NowPlayPosition].Replace(dirPath, ""));
                }

                volumnStream = new WaveChannel32(mainOutputStream);
                player = new WaveOut();

                _playThread = new Thread(() =>
                {
                    for (;;)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(3));
                        if (mainOutputStream.CurrentTime == mainOutputStream.TotalTime)
                        {
                            originalDispatcher.BeginInvoke(new Action(() =>
                            {
                                Next();
                            }));

                            break;
                        }
                    }
                });
                _playThread.Start();

                player.Init(volumnStream);
                IsPlaying = true;
                player.Play();
            }
        }

        public void Stop()
        {
            if(player!=null)
            {
                player.Stop();
            }
        }

        public void Next()
        {
            if (IsPlayable)
            {
                if (player != null)
                {
                    DisposePlayer();
                }

                NowPlayPosition++;
                if (MP3FileList.Count <= NowPlayPosition)
                {
                    NowPlayPosition = 0;
                }

                Play();
            }
        }

        public void Previous()
        {
            if (IsPlayable)
            {
                if (player != null)
                {
                    DisposePlayer();
                }

                NowPlayPosition--;
                if (NowPlayPosition < 0)
                {
                    NowPlayPosition = MP3FileList.Count-1;
                }

                Play();
            }
        }
    }
}
