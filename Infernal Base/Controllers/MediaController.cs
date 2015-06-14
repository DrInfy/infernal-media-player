#region Usings

using System;
using Base.Interfaces;
using Base.Libraries;
using Imp.Controllers;

#endregion

namespace Base.Controllers
{
    public class MediaController
    {
        #region Static Fields and Constants

        private const double startingMoveValue = 20.00;
        private const int MoveValueMax = 1000;
        private const int MoveValueIncrease = 100;

        #endregion

        #region Fields

        // variables for handling keyboard based movement
        public bool LastMoved = false;
        public bool MoveTemp = false;
        private readonly IMediaUriPlayer player;
        private readonly IStateButton playButton;
        private readonly IStateButton muteButton;
        private readonly IStateButton loopButton;
        private readonly IEventController eventC;
        private double volume = 1;
        private double moveValue;
        private long lastMovedtime;
        private LoopMode loopMode = LoopMode.NoLoop;

        #endregion

        #region Properties

        public bool Paused
        {
            get { return playButton.CurrentState == 0; }
        }

        public double Volume
        {
            get { return volume; }
        }

        public LoopMode LoopMode
        {
            get { return loopMode; }
            set { loopMode = value; }
        }

        #endregion

        public MediaController(IMediaUriPlayer player, IStateButton playButton, IStateButton muteButton, IStateButton loopButton, IEventController eventC)
        {
            this.muteButton = muteButton;
            this.eventC = eventC;
            this.loopButton = loopButton;
            this.playButton = playButton;
            this.player = player;
        }

        public void Play()
        {
            if (player.IsPlaying)
            {
                player.Play();
                playButton.CurrentState = 1;
                eventC.SetEvent(new EventText("Playing", 1, EventType.Delayed));
                ImpNativeMethods.PreventSleep();
            }
        }

        public void Pause()
        {
            player.Pause();
            playButton.CurrentState = 0;
            eventC.SetEvent(new EventText("Paused", 1, EventType.Delayed));
            ImpNativeMethods.AllowSleep();
        }

        public void PlayPause()
        {
            if (playButton.CurrentState == 0)
                Play();
            else
                Pause();
        }

        public void MediaClosed()
        {
            playButton.CurrentState = 0;
            playButton.IsEnabled = false;
            ImpNativeMethods.AllowSleep();
        }

        public void MediaOpened()
        {
            playButton.IsEnabled = true;
            if (playButton.CurrentState != 0)
                player.Volume = Volume;

            Play();
        }

        public void Stop()
        {
            player.Stop();
            playButton.CurrentState = 0;
            ImpNativeMethods.AllowSleep();
        }

        public void Rewind()
        {
            FastMove(-1);
        }

        public void Fastforward()
        {
            FastMove(1);
        }

        private void FastMove(int sign)
        {
            if (LastMoved)
                moveValue = Math.Abs(moveValue) + ((DateTime.Now.Ticks - lastMovedtime) * MoveValueIncrease * LibImp.TicksToSecond);
            else
            {
                moveValue = startingMoveValue;
                lastMovedtime = DateTime.Now.Ticks - (long) (0.015 * LibImp.TicksToSecond);
            }

            moveValue = Math.Min(MoveValueMax, moveValue) * sign;

            SetPosition(player.Position + (DateTime.Now.Ticks - lastMovedtime) * LibImp.TicksToSecond * moveValue);

            lastMovedtime = DateTime.Now.Ticks;
            MoveTemp = true;
        }

        public void Mute()
        {
            player.Volume = 0;
            muteButton.CurrentState = 0;
            eventC.SetEvent(new EventText("Muted"));
        }

        public void Unmute()
        {
            player.Volume = Volume;
            muteButton.CurrentState = 1;
            eventC.SetEvent(new EventText(String.Format("Volume: {0}%", ((int) (Volume * 100)))));
        }

        public void ToggleMute()
        {
            if (muteButton.CurrentState == 0)
                Unmute();
            else
                Mute();
        }

        public void FreePlayer()
        {
            player.Clear();
            ImpNativeMethods.AllowSleep();
        }

        public void ToggleLoop()
        {
            switch (LoopMode)
            {
                case LoopMode.NoLoop:
                    loopMode = LoopMode.LoopAll;
                    loopButton.CurrentState = 1;
                    break;
                case LoopMode.LoopAll:
                    loopMode = LoopMode.LoopOne;
                    loopButton.CurrentState = 2;
                    break;
                case LoopMode.LoopOne:
                    loopMode = LoopMode.NoLoop;
                    loopButton.CurrentState = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetPosition(double positionInSeconds)
        {
            var rewind = (player.Position > positionInSeconds);
            positionInSeconds = Math.Max(Math.Min(positionInSeconds, player.Duration), 0);


            eventC.SetEvent(
                new EventText(String.Format("{0} {1}", rewind ? "<< " : ">> ",
                    StringHandler.SecondsToTimeText((int) Math.Round(positionInSeconds)))));
            player.Position = positionInSeconds;
        }

        public void SetVolume(double volumePercentage)
        {
            var volumeBy100 = Math.Max(Math.Min(Math.Round(volumePercentage * 100), 100), 0);

            eventC.SetEvent(new EventText(String.Format("Volume: {0}%", ((int) volumeBy100))));

            volume = volumeBy100 / 100;

            // apply volume only when not muted
            if (muteButton.CurrentState != 0)
                player.Volume = Volume;
        }
    }
}