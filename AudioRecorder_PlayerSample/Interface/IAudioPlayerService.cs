using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioRecorder_PlayerSample.Interface
{
    public interface IAudioPlayerService
    {
        void PlayAudio(string filePath);
        void Pause();
        void Stop();
        string GetCurrentPlayTime();
        bool CheckFinishedPlayingAudio();
    }
}
