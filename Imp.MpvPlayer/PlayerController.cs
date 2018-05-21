#region Usings

using System;
using System.ComponentModel;

#endregion

namespace Imp.MpvPlayer
{
    public class PlayerController : INotifyPropertyChanged
    {
        #region  Public Fields and Properties

        public TimeSpan Duration
        {
            get => this.duration;
            set
            {
                if (value != this.duration)
                {
                    this.duration = value;
                    NotifyPropertyChanged(nameof(this.Duration));
                }
            }
        }

        public TimeSpan Position
        {
            get => this.position;
            set
            {
                if (value != this.position)
                {
                    this.position = value;
                    NotifyPropertyChanged(nameof(this.Position));
                }
            }
        }

        public bool IsMediaLoaded
        {
            get => this.isMediaLoaded;
            set
            {
                if (value != this.isMediaLoaded)
                {
                    this.isMediaLoaded = value;
                    NotifyPropertyChanged(nameof(this.IsMediaLoaded));
                }
            }
        }

        #endregion

        #region Local Fields

        private TimeSpan duration;
        private TimeSpan position;

        private bool isMediaLoaded;

        #endregion

        #region Common

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            var eventArgs = new PropertyChangedEventArgs(propertyName);
            PropertyChanged?.Invoke(this, eventArgs);
        }

        #endregion
    }
}