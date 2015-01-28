using System;
using Base.Libraries;

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

    public class ImpFader
    {
        const long FADEDOWNTIME = 30 * LibImp.NANOTOMILLI;
        const long FADEUPTIME = 50 * LibImp.NANOTOMILLI;
        const long MOVEINTERVALTIME = 200 * LibImp.NANOTOMILLI;
        const double UPMINIMUMFADE = 0.005;
        const double DOWNMINIMUMFADE = 0.005;
        private const double UPMAXFADE = 0.15;

        const double DOWNMAXFADE = 0.15;

        FadeType fade = FadeType.None;
        long posTarget = 0;

        double volumeTarget = 0;
        long LastSetTime = 0;

        long LastPositionSetTime = 0;
        
        long startuptotalwait;

        public bool Active { get; private set; }

        

        public void Play()
        {
            if ((fade == FadeType.None | fade == FadeType.Pausing | fade == FadeType.VolumeChange))
                fade = FadeType.Playing;
            StartFade();
        }

        public void Startup()
        {
            startuptotalwait = 0;
            if ((fade == FadeType.None | fade == FadeType.Pausing | fade == FadeType.VolumeChange))
                fade = FadeType.Startup;
            StartFade();
        }

        public void Pause()
        {
            if ((fade != FadeType.MoveFalling))
                posTarget = -1;
            fade = FadeType.Pausing;
            StartFade();
        }



        public void SetVolume(double value)
        {
            volumeTarget = value;
            if ((fade == FadeType.None))
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
            if (value < 0)
                posTarget = 0;
            posTarget = value;
            if ((fade != FadeType.Pausing))
            {
                fade = FadeType.MoveFalling;
                StartFade();
            }
        }

        /// <summary>
        /// Returns the desired or current position based on fade type
        /// </summary>
        /// <param name="value">Pass the current player position here</param>
        /// <returns></returns>
        public long GetPosition(long value)
        {
            if ((posTarget == -1))
            {
                return value;
            }
            else
            {
                return posTarget;
            }
        }

        /// <summary>
        /// Starts fading towards desired volume and/or position
        /// </summary>
        private void StartFade()
        {
            if (Active)
                return;

            LastSetTime = DateTime.Now.Ticks;
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
            dynamic pause_ = false;

            if ((double.IsNaN(volume) | volume <= 0))
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
                    fadevolume(ref volume, volumeTarget, paused);
                    if ((volume == volumeTarget))
                        EndFade();

                    break;
                case FadeType.MoveFalling:
                    fadevolume(ref volume, 0, paused);

                    if ((volume == 0))
                    {

                        if ((LastPositionSetTime + MOVEINTERVALTIME < DateTime.Now.Ticks))
                        {
                            LastPositionSetTime = DateTime.Now.Ticks;
                            position = posTarget;

                            fade = (paused) ? FadeType.MoveRising : FadeType.MovePaused;
                        }
                    }

                    break;
                case FadeType.MovePaused:
                    if ((paused | (position > posTarget + LibImp.SecondToTicks / 3)))
                    {
                        fade = FadeType.MoveRising;
                    }

                    break;
                case FadeType.MoveRising:
                    fadevolume(ref volume, volumeTarget, paused);
                    posTarget = -1;
                    if ((volume == volumeTarget))
                        EndFade();

                    break;
                case FadeType.Pausing:
                    fadevolume(ref volume, 0, paused);

                    if ((volume == 0))
                    {
                        if ((posTarget != -1))
                            position = posTarget;

                        EndFade();
                        pause_ = true;
                    }
                    break;
                case FadeType.Startup:
                    startuptotalwait += DateTime.Now.Ticks - LastSetTime;
                    if (startuptotalwait > 0.2 * LibImp.SecondToTicks)
                    {
                        fade = FadeType.Playing;
                    }
                    break;
                case FadeType.Playing:
                    fadevolume(ref volume, volumeTarget, paused);
                    if ((volume == volumeTarget))
                        EndFade();

                    break;
            }
            LastSetTime = DateTime.Now.Ticks;

            volume = Math.Log10(volume * 99 + 1) / 2;
            return pause_;
        }

        /// <summary>
        /// Smoothly fades volume towards the desired value
        /// </summary>
        private void fadevolume(ref double volume, double value, bool paused)
        {
            // No need for audiofading when the system is on pause
            if (((paused & fade != FadeType.Pausing) | (volume == 0 & value == 0)))
            {
                put_volume(ref volume, 0);
                return;
            }

            // set temp as valuechange based on time passed
            double temp = DateTime.Now.Ticks - LastSetTime;

            if ((volume < value))
            {
                temp /= FADEUPTIME;
            }
            else
            {
                temp /= FADEDOWNTIME;
            }

            if ((temp > 1))
                temp = 1;

            // In most cases this sets the new value for volume
            temp = volume + (value - volume) * temp;

            // check that the fade isn't too big or too small
            if ((volume < value))
            {
                if ((temp - volume > UPMAXFADE))
                    temp = volume + UPMAXFADE;
                if ((temp - volume < UPMINIMUMFADE))
                    temp = volume + UPMINIMUMFADE;
                if ((temp > value))
                    temp = value;
            }
            else
            {
                if ((volume - temp > DOWNMAXFADE))
                    temp = volume - DOWNMAXFADE;
                if ((volume - temp < DOWNMINIMUMFADE))
                    temp = volume - DOWNMINIMUMFADE;
                if ((temp < value))
                    temp = value;
            }


            put_volume(ref volume, temp);
        }

        /// <summary>
        /// Ensures no out of bounds volumevalues, takes 0-1
        /// </summary>
        private void put_volume(ref double volume, double value)
        {
            if ((value < 0))
            {
                value = 0;
            }
            else if ((value > 1))
            {
                value = 1;
            }
            volume = value;
        }
    }


}
