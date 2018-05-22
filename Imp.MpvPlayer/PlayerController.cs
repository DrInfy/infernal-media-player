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
            get => this.targetPosition ?? this.currentPosition;
            set
            {
                if (value != this.currentPosition)
                {
                    this.currentPosition = value;
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

        public double Volume
        {
            get => this.targetVolume;
            set => this.targetVolume = (int)value;
        }

        #endregion

        #region Local Fields

        private TimeSpan duration;
        private TimeSpan? targetPosition;
        private TimeSpan currentPosition;
        
        private int targetVolume;
        private bool isMediaLoaded;

        #endregion

        #region Common

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            var eventArgs = new PropertyChangedEventArgs(propertyName);
            PropertyChanged?.Invoke(this, eventArgs);
        }

        public void SetTargetPosition(Mpv.WPF.MpvPlayer player, TimeSpan position)
        {
            this.targetPosition = position;
            player.Position = position;
        }

        public void ClearTarget()
        {
            this.targetPosition = null;
        }

        #endregion
    }
}