#region Usings

using System;
using Base.Libraries;

#endregion

namespace Base.Controllers
{
    public enum FadeType
    {
        None,
        Pausing,
        Startup,
        Playing,
        MoveFalling,
        MovePaused,
        MoveRising,
        VolumeChange,
    }

    /// <summary>
    /// Implementation independent volume fader that handles volumes as percentages
    /// and media position as ticks
    /// </summary>
    public class ImpFader
    {
        #region Static Fields and Constants

        private const long fadeDownTime = 30 * LibImp.NanoToMilli;
        private const long fadeUpTime = 50 * LibImp.NanoToMilli;
        private const long moveIntervalTime = 200 * LibImp.NanoToMilli;
        private const double upMinimumFade = 0.005;
        private const double downMinimumFade = 0.005;
        private const double upMaxFade = 0.15;
        private const double downMaxFade = 0.15;

        #endregion

        #region Fields

        private FadeType fade = FadeType.None;
        private long posTarget = 0;
        private double volumeTarget = 0;
        private long lastSetTime = 0;
        private long lastPositionSetTime = 0;
        private long startuptotalwait;

        #endregion

        #region Properties

        public bool Active { get; private set; }

        #endregion

        public void Play()
        {
            if (fade == FadeType.None | fade == FadeType.Pausing | fade == FadeType.VolumeChange)
                fade = FadeType.Playing;
            StartFade();
        }

        public void Startup()
        {
            startuptotalwait = 0;
            if (fade == FadeType.None | fade == FadeType.Pausing | fade == FadeType.VolumeChange)
                fade = FadeType.Startup;
            StartFade();
        }

        public void Pause()
        {
            if (fade != FadeType.MoveFalling) { posTarget = -1; }
            fade = FadeType.Pausing;
            StartFade();
        }

        public void SetVolume(double value)
        {
            volumeTarget = value;
            if (fade == FadeType.None)
                fade = FadeType.VolumeChange;
            StartFade();
        }

        /// <summary>
        /// Player was just killed, we do not wish to continue fading anymore
        /// </summary>
        public void PlayerKilled()
        {
            posTarget = 0;
            EndFade();
        }

        /// <summary>
        /// Starts fading towards desired position
        /// </summary>
        /// <param name="value">Pass the desired position</param>
        public void SetPosition(long value)
        {
            if (value < 0) { posTarget = 0; }
            posTarget = value;

            if (fade != FadeType.Pausing)
            {
                fade = FadeType.MoveFalling;
                StartFade();
            }
        }

        /// <summary>
        /// Returns the desired or current position based on fade type
        /// </summary>
        /// <param name="value">Pass the current player position here</param>
        /// <returns> The desired position or current position if no move is active.</returns>
        public long GetPosition(long value)
        {
            return (posTarget == -1) ? value : posTarget;
        }

        /// <summary>
        /// Starts fading towards desired volume and/or position
        /// </summary>
        private void StartFade()
        {
            if (Active) { return; }

            lastSetTime = DateTime.Now.Ticks;
            Active = true;
        }

        /// <summary>
        /// Puts an end to all fading 
        /// </summary>
        private void EndFade()
        {
            fade = FadeType.None;
            Active = false;
            posTarget = -1;
        }

        /// <summary>
        /// Update from the main thread to update smooth position and volume changes.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="volume"></param>
        /// <param name="paused"></param>
        /// <returns>True if requesting pause.</returns>
        public bool Update(ref long position, ref double volume, bool paused)
        {
            var pause = false;

            if (double.IsNaN(volume) | volume <= 0)
            {
                volume = 0;
            }
            else
            {
                volume = (Math.Pow(10, volume * 2) - 1) / 99;
            }

            switch (fade)
            {
                case FadeType.VolumeChange:
                    FadeVolume(ref volume, volumeTarget, paused);
                    if (volume == volumeTarget) { EndFade(); }

                    break;
                case FadeType.MoveFalling:
                    FadeVolume(ref volume, 0, paused);

                    if (volume == 0)
                    {
                        if (lastPositionSetTime + moveIntervalTime < DateTime.Now.Ticks)
                        {
                            lastPositionSetTime = DateTime.Now.Ticks;
                            position = posTarget;

                            fade = (paused) ? FadeType.MoveRising : FadeType.MovePaused;
                        }
                    }

                    break;
                case FadeType.MovePaused:
                    if (paused | (position > posTarget + LibImp.SecondToTicks / 3))
                    {
                        fade = FadeType.MoveRising;
                    }

                    break;

                case FadeType.MoveRising:
                    FadeVolume(ref volume, volumeTarget, paused);
                    posTarget = -1;
                    if (volume == volumeTarget) { EndFade(); }
                    break;

                case FadeType.Pausing:
                    FadeVolume(ref volume, 0, paused);

                    if (volume == 0)
                    {
                        if (posTarget != -1)
                            position = posTarget;

                        EndFade();
                        pause = true;
                    }
                    break;

                case FadeType.Startup:
                    startuptotalwait += DateTime.Now.Ticks - lastSetTime;
                    if (startuptotalwait > 0.2 * LibImp.SecondToTicks)
                    {
                        fade = FadeType.Playing;
                    }
                    break;

                case FadeType.Playing:
                    FadeVolume(ref volume, volumeTarget, paused);
                    if (volume == volumeTarget) { EndFade(); }
                    break;
            }
            lastSetTime = DateTime.Now.Ticks;

            volume = Math.Log10(volume * 99 + 1) / 2;
            return pause;
        }

        /// <summary>
        /// Smoothly fades volume towards the desired value
        /// </summary>
        private void FadeVolume(ref double volume, double value, bool paused)
        {
            // No need for audiofading when the system is on pause
            if (((paused & fade != FadeType.Pausing) | (volume == 0 & value == 0)))
            {
                volume = PutVolume(0);
                return;
            }

            // set temp as valuechange based on time passed
            double temp = DateTime.Now.Ticks - lastSetTime;

            if ((volume < value))
            {
                temp /= fadeUpTime;
            }
            else
            {
                temp /= fadeDownTime;
            }

            if ((temp > 1))
                temp = 1;

            // In most cases this sets the new value for volume
            temp = volume + (value - volume) * temp;

            // check that the fade isn't too big or too small
            if ((volume < value))
            {
                if ((temp - volume > upMaxFade))
                    temp = volume + upMaxFade;
                if ((temp - volume < upMinimumFade))
                    temp = volume + upMinimumFade;
                if ((temp > value))
                    temp = value;
            }
            else
            {
                if ((volume - temp > downMaxFade))
                    temp = volume - downMaxFade;
                if ((volume - temp < downMinimumFade))
                    temp = volume - downMinimumFade;
                if ((temp < value))
                    temp = value;
            }


            volume = PutVolume(temp);
        }

        /// <summary>
        /// Ensures no out of bounds volumevalues, takes 0-1
        /// </summary>
        private static double PutVolume(double value)
        {
            if ((value < 0))
            {
                value = 0;
            }
            else if ((value > 1))
            {
                value = 1;
            }

            return value;
        }
    }
}