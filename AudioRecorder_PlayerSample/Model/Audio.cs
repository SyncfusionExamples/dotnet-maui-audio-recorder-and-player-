using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AudioRecorder_PlayerSample.Model
{
    public class Audio : INotifyPropertyChanged
    {
        #region Private

        private bool isPlayVisible;
        private bool isPauseVisible;
        private string currentAudioPostion;

        #endregion

        #region Constructor

        public Audio()
        {
            IsPlayVisible = true;
        }

        #endregion

        #region Properties

        public string AudioName { get; set; }
        public string AudioURL { get; set; }
        public string Caption { get; set; }
        public bool IsPlayVisible
        {
            get { return isPlayVisible; }
            set
            {
                isPlayVisible = value;
                OnPropertyChanged();
                IsPauseVisble = !value;
            }
        }
        public bool IsPauseVisble
        {
            get { return isPauseVisible; }
            set { isPauseVisible = value; OnPropertyChanged(); }
        }
        public string CurrentAudioPosition
        {
            get { return currentAudioPostion; }
            set
            {
                if (string.IsNullOrEmpty(currentAudioPostion))
                {
                    currentAudioPostion = string.Format("{0:mm\\:ss}", new TimeSpan());
                }
                else
                {
                    currentAudioPostion = value;
                }
                OnPropertyChanged();
            }
        }

        #endregion

        #region Interface

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
