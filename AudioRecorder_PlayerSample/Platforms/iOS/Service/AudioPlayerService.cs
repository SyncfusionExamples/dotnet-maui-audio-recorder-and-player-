using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioRecorder_PlayerSample.Interface;
using AVFoundation;
using Foundation;

namespace AudioRecorder_PlayerSample.Platforms.Service
{
    public partial class AudioPlayerService : IAudioPlayerService
    {
        #region Fields
        AVPlayer _player;
        NSObject notificationHandle;
        NSUrl url;
        private bool isFinishedPlaying;
        private bool isPlaying;
        #endregion

        #region Properties
        public bool IsPlaying
        {
            get { return isPlaying; }
            set
            {
                if (_player.Rate == 1 && _player.Error == null)
                    isPlaying = true;
                else
                    isPlaying = false;
            }
        }
        #endregion

        #region Constructor
        public AudioPlayerService()
        {
            RegisterNotification();
        }
        #endregion

        #region Destrcutor
        ~AudioPlayerService()
        {
            UnregisterNotification();
        }
        #endregion

        #region Methods

        /// <summary>
        /// This method is used to play the recorder audio.
        /// </summary>
        public void PlayAudio(string filePath)
        {
            isFinishedPlaying = false;
            if (_player == null)
            {
                AVAsset asset = AVAsset.FromUrl(NSUrl.CreateFileUrl(new[] { filePath }));
                AVPlayerItem avPlayerItem = new AVPlayerItem(asset);
                _player = new AVPlayer(avPlayerItem);
                _player.AutomaticallyWaitsToMinimizeStalling = false;
                _player.Volume = 1;

                _player.Play();
                IsPlaying = true;
                isFinishedPlaying = false;
            }
            else if (_player != null && !IsPlaying)
            {
                _player.Play();
                IsPlaying = true;
                isFinishedPlaying = false;
            }
        }

        /// <summary>
        /// Using this metod to pause the audio.
        /// </summary>
        public void Pause()
        {
            if (_player != null && IsPlaying)
            {
                _player.Pause();
                IsPlaying = false;
            }
        }

        /// <summary>
        /// This method is used to stop the audio.
        /// </summary>
        public void Stop()
        {
            if (_player != null)
            {
                _player.Dispose();
                IsPlaying = false;
                _player = null;
            }
        }

        /// <summary>
        /// This method is used to get the current time of the audio.
        /// </summary>
        public string GetCurrentPlayTime()
        {
            if (_player != null)
            {
                var positionTimeSeconds = _player.CurrentTime.Seconds;
                TimeSpan currentTime = TimeSpan.FromSeconds(positionTimeSeconds);
                string currentPlayTime = string.Format("{0:mm\\:ss}", currentTime);
                return currentPlayTime;
            }
            return null;
        }

        public bool CheckFinishedPlayingAudio()
        {
            return isFinishedPlaying;
        }

        /// <summary>
        /// This method used to register notification for recieve the alert when complete the audio playing.
        /// </summary>
        private void RegisterNotification()
        {
            notificationHandle = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, HandleNotification);
        }

        private void UnregisterNotification()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(notificationHandle);
        }

        /// <summary>
        /// Invoked when audio playing completed.
        /// </summary>
        private void HandleNotification(NSNotification notification)
        {
            isFinishedPlaying = true;
            Stop();
        }

        #endregion
    }
}
