#region Usings

using System.Collections.Generic;
using System.Windows.Input;
using Imp.Base;
using Imp.Base.Commands;

#endregion

namespace Imp.Player.Controllers
{
    public partial class MainController
    {
        public override List<KeyCommand<ImpCommand>> GenerateDefaultKeyCommands()
        {
            var cmds = new List<KeyCommand<ImpCommand>>();
            // play / pause keycommands
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.Playpause, Key = Key.Space, NeedRelease = true});
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.GlobalPlaypause, Key = Key.MediaPlayPause, NeedRelease = true, Anywhere = true, AllowedStyle = PlayerStyle.MediaPlayer});
            cmds.Add(new KeyCommand<ImpCommand>()
            {
                Command = ImpCommand.GlobalPlaypause,
                Key = Key.Pause,
                NeedRelease = true,
                Anywhere = true,
                ModifierKeys = ModifierKeys.Shift,
                AllowedStyle = PlayerStyle.MediaPlayer
            });
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.Pause, Key = Key.MediaStop, NeedRelease = true, Anywhere = true, AllowedStyle = PlayerStyle.All});

            // rewind / fast forward
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.Fastforward, Key = Key.Right, NeedRelease = false, AllowedStyle = PlayerStyle.MediaPlayer});
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.Rewind, Key = Key.Left, NeedRelease = false, AllowedStyle = PlayerStyle.MediaPlayer});
            // in picture mode
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.OpenNext, Key = Key.Right, NeedRelease = true, AllowedStyle = PlayerStyle.PictureViewer});
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.OpenPrev, Key = Key.Left, NeedRelease = true, AllowedStyle = PlayerStyle.PictureViewer});

            // volume / mute keycommands
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.VolumeChange, Key = Key.Add, NeedRelease = false, Argument = 0.01});
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.VolumeChange, Key = Key.Subtract, NeedRelease = false, Argument = -0.01});
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.VolumeChange, Key = Key.Up, NeedRelease = false, ModifierKeys = ModifierKeys.Control, Argument = 0.01});
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.VolumeChange, Key = Key.Down, NeedRelease = false, ModifierKeys = ModifierKeys.Control, Argument = -0.01});
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.VolumeMute, Key = Key.M, NeedRelease = true});


            // Open previous / next keycommands
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.OpenNext, Key = Key.PageDown, NeedRelease = true});
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.OpenPrev, Key = Key.PageUp, NeedRelease = true});
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.OpenNext, Key = Key.Right, NeedRelease = true, ModifierKeys = ModifierKeys.Control});
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.OpenPrev, Key = Key.Left, NeedRelease = true, ModifierKeys = ModifierKeys.Control});
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.OpenNext, Key = Key.MediaNextTrack, NeedRelease = true, Anywhere = true, AllowedStyle = PlayerStyle.MusicPlayer});
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.OpenPrev, Key = Key.MediaPreviousTrack, NeedRelease = true, Anywhere = true, AllowedStyle = PlayerStyle.MusicPlayer});
            cmds.Add(new KeyCommand<ImpCommand>()
            {
                Command = ImpCommand.OpenNext,
                Key = Key.Oem7,
                NeedRelease = true,
                Anywhere = true,
                ModifierKeys = ModifierKeys.Control,
                AllowedStyle = PlayerStyle.MediaPlayer
            });
            cmds.Add(new KeyCommand<ImpCommand>()
            {
                Command = ImpCommand.OpenPrev,
                Key = Key.Oem3,
                NeedRelease = true,
                Anywhere = true,
                ModifierKeys = ModifierKeys.Control,
                AllowedStyle = PlayerStyle.MediaPlayer
            });


            // panel and window keycommands
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.Exit, Key = Key.Escape, NeedRelease = true});
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.Exit, Key = Key.F12, NeedRelease = true});

            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.ToggleFullscreen, Key = Key.F11, NeedRelease = true});
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.PlayerMinimize, Key = Key.F10, NeedRelease = true});

            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.PanelOpen, Key = Key.F6, NeedRelease = true});
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.PanelSettings, Key = Key.F7, NeedRelease = true});
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.PanelPlaylist, Key = Key.F8, NeedRelease = true});

            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.TogglePanels, Key = Key.Space, ModifierKeys = ModifierKeys.Control, NeedRelease = true});

            // copy name text
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.CopyName, Key = Key.C, ModifierKeys = ModifierKeys.Control});


            // play random
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.OpenRandom, Key = Key.R, ModifierKeys = ModifierKeys.Control, NeedRelease = true});
            cmds.Add(new KeyCommand<ImpCommand>() {Command = ImpCommand.Shuffle, Key = Key.S, ModifierKeys = ModifierKeys.Control, NeedRelease = true});

            return cmds;
        }
    }
}