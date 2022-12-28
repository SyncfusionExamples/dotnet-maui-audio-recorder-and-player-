using AudioRecorder_PlayerSample.Interface;
using AudioRecorder_PlayerSample.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AudioRecorder_PlayerSample.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        #region private variable

        private string recentAudioFilePath;
        private bool isRecordingAudio;
        private TimeSpan timerValue;
        private string timerLabel;
        private bool isRecord;
        private Audio audioFile;
        private bool isRecordButtonVisible;
        private bool isPauseButtonVisible;
        private bool isResumeButtonVisible;
        IRecordAudioService recordAudioService;
        IAudioPlayerService audioPlayerService;
        IDispatcherTimer recordTimer;
        IDispatcherTimer playTimer;
        private ObservableCollection<Audio> audios;

        #endregion

        #region Constructor
        public MainPageViewModel(IAudioPlayerService audioPlayerService, IRecordAudioService recordAudioService)
        {
            this.audioPlayerService = audioPlayerService;
            this.recordAudioService = recordAudioService;
            Audios = new ObservableCollection<Audio>();
            IsRecordButtonVisible = true;
            IsRecordingAudio = false;
            IsResumeButtonVisible = false;

            DeleteCommand = new Command<Audio>(DeleteAudio);
            ResetCommand = new Command(ResetRecording);
            RecordCommand = new Command(StartRecording);
            StopCommand = new Command(StopRecording);
            SendCommand = new Command(SendRecording);
            PauseAudioCommand = new Command(PauseAudio);
            PlayAudioCommand = new Command(StartPlayingAudio);
            PauseCommand = new Command(PauseRecording);          
        }


        #endregion

        #region Commands
        public ICommand RecordCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand SendCommand { get; set; }
        public ICommand PauseAudioCommand { get; set; }
        public ICommand PlayAudioCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        #endregion

        #region Properties
        public string RecentAudioFilePath
        {
            get { return recentAudioFilePath; }
            set { recentAudioFilePath = value; OnPropertyChanged(); }
        }
        public bool IsRecordingAudio
        {
            get { return isRecordingAudio; }
            set
            {
                isRecordingAudio = value;
                OnPropertyChanged();
            }
        }
        public string TimerLabel
        {
            get { return timerLabel; }
            set { timerLabel = value; OnPropertyChanged(); }
        }

        public Audio AudioFile
        {
            get { return audioFile; }
            set { audioFile = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Audio> Audios
        {
            get { return audios; }
            set { audios = value; OnPropertyChanged(); }
        }

        public bool IsRecordButtonVisible
        {
            get { return isRecordButtonVisible; }
            set
            {
                isRecordButtonVisible = value;
                OnPropertyChanged("IsRecordButtonVisible");
            }
        }
        public bool IsPauseButtonVisible
        {
            get { return isPauseButtonVisible; }
            set { isPauseButtonVisible = value; OnPropertyChanged(); }
        }
        public bool IsResumeButtonVisible
        {
            get { return isResumeButtonVisible; }
            set { isResumeButtonVisible = value; OnPropertyChanged(); }
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Methods

        public void CreateTimer()
        {
            recordTimer = Application.Current.Dispatcher.CreateTimer();
            
            //timer start
            recordTimer.Interval = new TimeSpan(0, 0, 1);
            recordTimer.Tick += (s, e) =>
            {
                if (isRecord)
                {
                    timerValue += new TimeSpan(0, 0, 1);
                    TimerLabel = string.Format("{0:mm\\:ss}", timerValue);
                }
            };
        }
        private async void StartRecording()
        {
            if (!IsRecordingAudio)
            {
                var permissionStatus = await RequestandCheckPermission();
                if (permissionStatus == PermissionStatus.Granted)
                {
                    IsRecordingAudio = true;
                    IsPauseButtonVisible = true;
                    recordAudioService.StartRecord();
                    IsRecordButtonVisible = false;
                    isRecord = true;
                    timerValue = new TimeSpan(0, 0, -1);
                    if (recordTimer == null)
                        CreateTimer();
                    recordTimer.Start();
                }
                else
                {
                    IsRecordingAudio = false;
                    IsPauseButtonVisible = false;
                }
            }
            else
            {
                ResumeRecording();
            }
        }
        private void PauseRecording()
        {
            isRecord = false;
            IsPauseButtonVisible = false;
            IsResumeButtonVisible = true;
            recordAudioService.PauseRecord();
        }
        private void ResumeRecording()
        {
            recordAudioService.StartRecord();
            IsResumeButtonVisible = false;
            IsPauseButtonVisible = true;
            isRecord = true;
        }
        private void ResetRecording()
        {
            recordAudioService.ResetRecord();
            timerValue = new TimeSpan();
            TimerLabel = string.Format("{0:mm\\:ss}", timerValue);
            IsRecordingAudio = false;
            IsPauseButtonVisible = false;
            IsResumeButtonVisible = false;
            StartRecording();
        }
        private async void StopRecording()
        {
            IsPauseButtonVisible = false;
            IsResumeButtonVisible = false;
            IsRecordingAudio = false;
            IsRecordButtonVisible = true;
            timerValue = new TimeSpan();
            recordTimer.Stop();
            RecentAudioFilePath = recordAudioService.StopRecord();
            await App.Current.MainPage.DisplayAlert("Alert", "Audio has been recorded", "OK");
            TimerLabel = string.Format("{0:mm\\:ss}", timerValue);
            SendRecording();
        }
        private void SendRecording()
        {
            Audio recordedFile = new Audio() { AudioURL = RecentAudioFilePath };
            if (recordedFile != null)
            {
                recordedFile.AudioName = Path.GetFileName(RecentAudioFilePath);
                Audios.Insert(0, recordedFile);
            }
        }
        private void StartPlayingAudio(object obj)
        {
            if (audioFile != null && audioFile != (Audio)obj)
            {
                AudioFile.IsPlayVisible = true;
                StopAudio();
            }
            if (obj is Audio)
            {
                audioFile = (Audio)obj;
                audioFile.IsPlayVisible = false;
                string audioFilePath = AudioFile.AudioURL;
                audioPlayerService.PlayAudio(audioFilePath);
                SetCurrentAudioPosition();
            }
        }
        private void PauseAudio(object obj)
        {
            if (obj is Audio)
            {
                var audioFile = (Audio)obj;
                audioFile.IsPlayVisible = true;
                audioPlayerService.Pause();
            }
        }
        public void StopAudio()
        {
            if (AudioFile != null)
            {
                audioPlayerService.Stop();
                playTimer.Stop();
            }
        }
        private void SetCurrentAudioPosition()
        {
            if(playTimer == null)
                playTimer = Application.Current.Dispatcher.CreateTimer();
            playTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            playTimer.Tick += (s, e) => 
            {
                if (AudioFile != null)
                {
                    AudioFile.CurrentAudioPosition = audioPlayerService.GetCurrentPlayTime();
                    bool isAudioCompleted = audioPlayerService.CheckFinishedPlayingAudio();
                    if (isAudioCompleted)
                    {
                        AudioFile.IsPlayVisible = true;
                        audioPlayerService.Stop();
                        playTimer.Stop();
                    }
                }
            };
            playTimer.Start();
        }
        private async void DeleteAudio(Audio obj)
        {
            var alert = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to delete the audio?", "Yes", "No");
            if (alert)
                Audios.Remove(obj);
        }
        public async Task<PermissionStatus> RequestandCheckPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
                await Permissions.RequestAsync<Permissions.StorageWrite>();

            status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
                await Permissions.RequestAsync<Permissions.Microphone>();

            PermissionStatus storagePermission = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            PermissionStatus microPhonePermission = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            if (storagePermission == PermissionStatus.Granted && microPhonePermission == PermissionStatus.Granted) 
            {
                return PermissionStatus.Granted;
            }
            return PermissionStatus.Denied;
        }
        #endregion
    }
}
