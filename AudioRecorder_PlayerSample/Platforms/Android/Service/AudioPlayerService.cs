using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Media;
using AudioRecorder_PlayerSample.Interface;
using Stream = Android.Media.Stream;

namespace AudioRecorder_PlayerSample.Platforms.Service
{
    public partial class AudioPlayerService : IAudioPlayerService
    {
        #region Fields

        private MediaPlayer _mediaPlayer;
        private int currentPositionLength = 0;
        private bool isPrepared;
        private bool isCompleted;

        #endregion

        #region Methods
        public void PlayAudio(string filePath)
        {
            if (_mediaPlayer != null && !_mediaPlayer.IsPlaying)
            {
                _mediaPlayer.SeekTo(currentPositionLength);
                currentPositionLength = 0;
                _mediaPlayer.Start();
            }

            else if (_mediaPlayer == null || !_mediaPlayer.IsPlaying)
            {
                try
                {
                    isCompleted = false;
                    _mediaPlayer = new MediaPlayer();
                    _mediaPlayer.SetDataSource(filePath);
                    _mediaPlayer.SetAudioStreamType(Stream.Music);
                    _mediaPlayer.PrepareAsync();
                    _mediaPlayer.Prepared += (sender, args) =>
                    {
                        isPrepared = true;
                        _mediaPlayer.Start();
                    };
                    _mediaPlayer.Completion += (sender, args) =>
                    {
                        isCompleted = true;
                    };
                }
                catch (Exception e)
                {
                    _mediaPlayer = null;
                }
            }
        }
        public void Pause()
        {
            if (_mediaPlayer != null && _mediaPlayer.IsPlaying)
            {
                _mediaPlayer.Pause();
                currentPositionLength = _mediaPlayer.CurrentPosition;
            }
        }
        public void Stop()
        {
            if (_mediaPlayer != null)
            {
                if (isPrepared)
                {
                    _mediaPlayer.Stop();
                    _mediaPlayer.Release();
                    isPrepared = false;
                }
                isCompleted = false;
                _mediaPlayer = null;
            }
        }
        public string GetCurrentPlayTime()
        {
            if (_mediaPlayer != null)
            {
                var positionTimeSeconds = double.Parse(_mediaPlayer.CurrentPosition.ToString());
                positionTimeSeconds = positionTimeSeconds / 1000;
                TimeSpan currentTime = TimeSpan.FromSeconds(positionTimeSeconds);
                string currentPlayTime = string.Format("{0:mm\\:ss}", currentTime);
                return currentPlayTime;
            }
            return null;
        }
        public bool CheckFinishedPlayingAudio()
        {
            return isCompleted;
        }
        
        #endregion
    }
}
