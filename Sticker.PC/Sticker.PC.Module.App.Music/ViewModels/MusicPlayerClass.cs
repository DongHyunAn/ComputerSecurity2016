using NAudio.Wave;
using System;

namespace Sticker.PC.Module.App.Music.ViewModels
{
    public class MusicPlayerClass
    {
        public WaveStream waveStream;
        public WaveOut waveOut;
        
        public double StartPoint { get; set; }
        public double EndPoint { get; set; }

        public bool isRepeat;
        public bool isPlaying;

        public MusicPlayerClass()
        {
            waveOut = new WaveOut();
        }
        public double getCurrentTimeSecond()
        {
            return waveStream.CurrentTime.TotalSeconds;
        }
        public string remainString;
        public double playMusic(string musicString)
        {

            if (isPlaying == false)
            {

                stopMusic();
                waveStream = new Mp3FileReader(musicString);
                //remainString = musicString;
                waveOut.Init(waveStream);
                waveOut.Play();
                isPlaying = true;
            }
            else
            {                
                waveOut.Resume();
            }
            return waveStream.TotalTime.TotalSeconds;
        }

        public void stopMusic()
        {
            if (waveOut != null)
            {

                isPlaying = false;
                waveOut.Stop();
            }
        }

        public void pauseMusic()
        {
            if (waveOut != null)
            {
                isPlaying = true;
                waveOut.Pause();
            }
        }

        public void resumeMusic()
        {
            isPlaying = false;
            waveOut.Play();
        }
        public double setPauseSecond()
        {
            if (waveOut != null)
            {
                return waveStream.CurrentTime.TotalSeconds;
            }
            return 0;
        }
        public double setStopSecond()
        {
            
            waveStream.Position = 0;
            return 0;
        }

        public void setPosition(double currentTime)
        {
            waveStream.CurrentTime = TimeSpan.FromSeconds(currentTime);

        }

        public string getCurrentClockTime()
        {
            return String.Format("{0:00}:{1:00}", waveStream.CurrentTime.Minutes, waveStream.CurrentTime.Seconds);
        }

        public string getToatlClockTime()
        {
            return String.Format("{0:00}:{1:00}", waveStream.TotalTime.Minutes, waveStream.TotalTime.Seconds);
        }

        public double nextPlay(string musicString, bool isPlaying)
        {
            if (waveOut != null)
            {
                stopMusic();
                waveStream.Dispose();
                waveStream = new Mp3FileReader(musicString);
                waveOut.Init(waveStream);
                //if (isPlaying == false)
                //{
                //    waveOut.Stop();
                //}
                //else
                //{
                //    waveOut.Play();
                //    isPlaying = true;
                //}   
                this.isPlaying = isPlaying;
            }
            if (isPlaying == true)
            {
                waveOut.Play();
            }
            
            return waveStream.TotalTime.TotalSeconds;
        }

        public void setMp(string musicString)
        {
            waveStream = new Mp3FileReader(musicString);
            waveOut.Init(waveStream);
        }
    }
}