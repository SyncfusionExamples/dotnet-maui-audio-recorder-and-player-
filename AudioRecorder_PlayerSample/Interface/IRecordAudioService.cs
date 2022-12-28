using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioRecorder_PlayerSample.Interface
{
    public interface IRecordAudioService
    {
        void StartRecord();
        string StopRecord();
        void PauseRecord();
        void ResetRecord();
    }
}
