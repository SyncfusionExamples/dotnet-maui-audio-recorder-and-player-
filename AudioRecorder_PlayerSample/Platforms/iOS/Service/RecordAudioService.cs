using AudioRecorder_PlayerSample.Interface;
using AVFoundation;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioRecorder_PlayerSample.Platforms.Service
{
    public partial class RecordAudioService : IRecordAudioService
    {
        #region Fields
        AVAudioRecorder recorder;
        NSUrl url;
        NSError error;
        NSDictionary settings;
        string audioFilePath;
        #endregion

        #region Constructor

        public RecordAudioService()
        {
            InitializeAudioSession();
        }
        #endregion

        #region Methods
        private bool InitializeAudioSession()
        {
            var audioSession = AVAudioSession.SharedInstance();
            var err = audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);
            if (err != null)
                return false;
            err = audioSession.SetActive(true);
            if (err != null)
                return false;
            return false;
        }
        public void StartRecord()
        {
            if (recorder == null)
            {
                string fileName = "/Record_" + DateTime.UtcNow.ToString("ddMMM_hhmmss") + ".wav";
                var docuFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                audioFilePath = docuFolder + fileName;

                url = NSUrl.FromFilename(audioFilePath);
                NSObject[] values = new NSObject[]
                {
                NSNumber.FromFloat(44100.0f), //Sample rate
                NSNumber.FromInt32((int)AudioToolbox.AudioFormatType.LinearPCM), //AVFormat
                NSNumber.FromInt32(2), //Channel
                NSNumber.FromInt32(16), //PCMBitDept
                NSNumber.FromBoolean(false), //IsBigEndianKey
                NSNumber.FromBoolean(false) //IsFloatKey
                };

                NSObject[] key = new NSObject[]
                {
                AVAudioSettings.AVSampleRateKey,
                AVAudioSettings.AVFormatIDKey,
                AVAudioSettings.AVNumberOfChannelsKey,
                AVAudioSettings.AVLinearPCMBitDepthKey,
                AVAudioSettings.AVLinearPCMIsBigEndianKey,
                AVAudioSettings.AVLinearPCMIsFloatKey
                };

                settings = NSDictionary.FromObjectsAndKeys(values, key);
                recorder = AVAudioRecorder.Create(url, new AudioSettings(settings), out error);
                recorder.PrepareToRecord();
                recorder.Record();
            }
            else
            {
                recorder.Record();
            }
        }
        public void PauseRecord()
        {
            recorder.Pause();
        }
        public void ResetRecord()
        {
            recorder.Dispose();
            recorder = null;
        }
        public string StopRecord()
        {
            if (recorder == null)
            {
                return string.Empty;
            }

            recorder.Stop();
            recorder = null;
            return audioFilePath;
        }

        #endregion
    }
}
