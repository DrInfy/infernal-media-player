#region Usings

using System;
using System.Windows.Input;

#endregion

namespace Base.Commands
{
    public class KeyboardState
    {
        #region Fields

        private readonly bool[] keyStates;
        private readonly Key[] keys;

        #endregion

        public KeyboardState()
        {
            var l = (Key[]) Enum.GetValues(typeof (Key));

            keyStates = new bool[l.Length];
            keys = new Key[l.Length];

            var realCount = 0;
            bool duplicateFound;
            for (var i = 0; i < l.Length; i++)
            {
                duplicateFound = false;
                keyStates[i] = false;
                for (var j = i + 1; j < l.Length; j++)
                {
                    if (l[i] == l[j])
                    {
                        duplicateFound = true;
                    }
                }
                if (!duplicateFound)
                {
                    keys[realCount] = l[i];
                    realCount++;
                }
            }

            Array.Resize(ref keys, realCount);
        }

        /// <summary>
        /// Sets the specified key value.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        internal void Set(int index, bool value)
        {
            keyStates[index] = value;
        }

        internal bool Get(Key key)
        {
            #region jumptable

            switch (key)
            {
                case Key.None:
                    return keyStates[0];
                case Key.Cancel:
                    return keyStates[1];
                case Key.Back:
                    return keyStates[2];
                case Key.Tab:
                    return keyStates[3];
                case Key.LineFeed:
                    return keyStates[4];
                case Key.Clear:
                    return keyStates[5];
                case Key.Return:
                    return keyStates[6];
                case Key.Pause:
                    return keyStates[7];
                case Key.Capital:
                    return keyStates[8];
                case Key.KanaMode:
                    return keyStates[9];
                case Key.JunjaMode:
                    return keyStates[10];
                case Key.FinalMode:
                    return keyStates[11];
                case Key.HanjaMode:
                    return keyStates[12];
                case Key.Escape:
                    return keyStates[13];
                case Key.ImeConvert:
                    return keyStates[14];
                case Key.ImeNonConvert:
                    return keyStates[15];
                case Key.ImeAccept:
                    return keyStates[16];
                case Key.ImeModeChange:
                    return keyStates[17];
                case Key.Space:
                    return keyStates[18];
                case Key.Prior:
                    return keyStates[19];
                case Key.Next:
                    return keyStates[20];
                case Key.End:
                    return keyStates[21];
                case Key.Home:
                    return keyStates[22];
                case Key.Left:
                    return keyStates[23];
                case Key.Up:
                    return keyStates[24];
                case Key.Right:
                    return keyStates[25];
                case Key.Down:
                    return keyStates[26];
                case Key.Select:
                    return keyStates[27];
                case Key.Print:
                    return keyStates[28];
                case Key.Execute:
                    return keyStates[29];
                case Key.Snapshot:
                    return keyStates[30];
                case Key.Insert:
                    return keyStates[31];
                case Key.Delete:
                    return keyStates[32];
                case Key.Help:
                    return keyStates[33];
                case Key.D0:
                    return keyStates[34];
                case Key.D1:
                    return keyStates[35];
                case Key.D2:
                    return keyStates[36];
                case Key.D3:
                    return keyStates[37];
                case Key.D4:
                    return keyStates[38];
                case Key.D5:
                    return keyStates[39];
                case Key.D6:
                    return keyStates[40];
                case Key.D7:
                    return keyStates[41];
                case Key.D8:
                    return keyStates[42];
                case Key.D9:
                    return keyStates[43];
                case Key.A:
                    return keyStates[44];
                case Key.B:
                    return keyStates[45];
                case Key.C:
                    return keyStates[46];
                case Key.D:
                    return keyStates[47];
                case Key.E:
                    return keyStates[48];
                case Key.F:
                    return keyStates[49];
                case Key.G:
                    return keyStates[50];
                case Key.H:
                    return keyStates[51];
                case Key.I:
                    return keyStates[52];
                case Key.J:
                    return keyStates[53];
                case Key.K:
                    return keyStates[54];
                case Key.L:
                    return keyStates[55];
                case Key.M:
                    return keyStates[56];
                case Key.N:
                    return keyStates[57];
                case Key.O:
                    return keyStates[58];
                case Key.P:
                    return keyStates[59];
                case Key.Q:
                    return keyStates[60];
                case Key.R:
                    return keyStates[61];
                case Key.S:
                    return keyStates[62];
                case Key.T:
                    return keyStates[63];
                case Key.U:
                    return keyStates[64];
                case Key.V:
                    return keyStates[65];
                case Key.W:
                    return keyStates[66];
                case Key.X:
                    return keyStates[67];
                case Key.Y:
                    return keyStates[68];
                case Key.Z:
                    return keyStates[69];
                case Key.LWin:
                    return keyStates[70];
                case Key.RWin:
                    return keyStates[71];
                case Key.Apps:
                    return keyStates[72];
                case Key.Sleep:
                    return keyStates[73];
                case Key.NumPad0:
                    return keyStates[74];
                case Key.NumPad1:
                    return keyStates[75];
                case Key.NumPad2:
                    return keyStates[76];
                case Key.NumPad3:
                    return keyStates[77];
                case Key.NumPad4:
                    return keyStates[78];
                case Key.NumPad5:
                    return keyStates[79];
                case Key.NumPad6:
                    return keyStates[80];
                case Key.NumPad7:
                    return keyStates[81];
                case Key.NumPad8:
                    return keyStates[82];
                case Key.NumPad9:
                    return keyStates[83];
                case Key.Multiply:
                    return keyStates[84];
                case Key.Add:
                    return keyStates[85];
                case Key.Separator:
                    return keyStates[86];
                case Key.Subtract:
                    return keyStates[87];
                case Key.Decimal:
                    return keyStates[88];
                case Key.Divide:
                    return keyStates[89];
                case Key.F1:
                    return keyStates[90];
                case Key.F2:
                    return keyStates[91];
                case Key.F3:
                    return keyStates[92];
                case Key.F4:
                    return keyStates[93];
                case Key.F5:
                    return keyStates[94];
                case Key.F6:
                    return keyStates[95];
                case Key.F7:
                    return keyStates[96];
                case Key.F8:
                    return keyStates[97];
                case Key.F9:
                    return keyStates[98];
                case Key.F10:
                    return keyStates[99];
                case Key.F11:
                    return keyStates[100];
                case Key.F12:
                    return keyStates[101];
                case Key.F13:
                    return keyStates[102];
                case Key.F14:
                    return keyStates[103];
                case Key.F15:
                    return keyStates[104];
                case Key.F16:
                    return keyStates[105];
                case Key.F17:
                    return keyStates[106];
                case Key.F18:
                    return keyStates[107];
                case Key.F19:
                    return keyStates[108];
                case Key.F20:
                    return keyStates[109];
                case Key.F21:
                    return keyStates[110];
                case Key.F22:
                    return keyStates[111];
                case Key.F23:
                    return keyStates[112];
                case Key.F24:
                    return keyStates[113];
                case Key.NumLock:
                    return keyStates[114];
                case Key.Scroll:
                    return keyStates[115];
                case Key.LeftShift:
                    return keyStates[116];
                case Key.RightShift:
                    return keyStates[117];
                case Key.LeftCtrl:
                    return keyStates[118];
                case Key.RightCtrl:
                    return keyStates[119];
                case Key.LeftAlt:
                    return keyStates[120];
                case Key.RightAlt:
                    return keyStates[121];
                case Key.BrowserBack:
                    return keyStates[122];
                case Key.BrowserForward:
                    return keyStates[123];
                case Key.BrowserRefresh:
                    return keyStates[124];
                case Key.BrowserStop:
                    return keyStates[125];
                case Key.BrowserSearch:
                    return keyStates[126];
                case Key.BrowserFavorites:
                    return keyStates[127];
                case Key.BrowserHome:
                    return keyStates[128];
                case Key.VolumeMute:
                    return keyStates[129];
                case Key.VolumeDown:
                    return keyStates[130];
                case Key.VolumeUp:
                    return keyStates[131];
                case Key.MediaNextTrack:
                    return keyStates[132];
                case Key.MediaPreviousTrack:
                    return keyStates[133];
                case Key.MediaStop:
                    return keyStates[134];
                case Key.MediaPlayPause:
                    return keyStates[135];
                case Key.LaunchMail:
                    return keyStates[136];
                case Key.SelectMedia:
                    return keyStates[137];
                case Key.LaunchApplication1:
                    return keyStates[138];
                case Key.LaunchApplication2:
                    return keyStates[139];
                case Key.Oem1:
                    return keyStates[140];
                case Key.OemPlus:
                    return keyStates[141];
                case Key.OemComma:
                    return keyStates[142];
                case Key.OemMinus:
                    return keyStates[143];
                case Key.OemPeriod:
                    return keyStates[144];
                case Key.Oem2:
                    return keyStates[145];
                case Key.Oem3:
                    return keyStates[146];
                case Key.AbntC1:
                    return keyStates[147];
                case Key.AbntC2:
                    return keyStates[148];
                case Key.Oem4:
                    return keyStates[149];
                case Key.Oem5:
                    return keyStates[150];
                case Key.Oem6:
                    return keyStates[151];
                case Key.Oem7:
                    return keyStates[152];
                case Key.Oem8:
                    return keyStates[153];
                case Key.Oem102:
                    return keyStates[154];
                case Key.ImeProcessed:
                    return keyStates[155];
                case Key.System:
                    return keyStates[156];
                case Key.OemAttn:
                    return keyStates[157];
                case Key.OemFinish:
                    return keyStates[158];
                case Key.OemCopy:
                    return keyStates[159];
                case Key.OemAuto:
                    return keyStates[160];
                case Key.OemEnlw:
                    return keyStates[161];
                case Key.OemBackTab:
                    return keyStates[162];
                case Key.Attn:
                    return keyStates[163];
                case Key.CrSel:
                    return keyStates[164];
                case Key.ExSel:
                    return keyStates[165];
                case Key.EraseEof:
                    return keyStates[166];
                case Key.Play:
                    return keyStates[167];
                case Key.Zoom:
                    return keyStates[168];
                case Key.NoName:
                    return keyStates[169];
                case Key.Pa1:
                    return keyStates[170];
                case Key.OemClear:
                    return keyStates[171];
                case Key.DeadCharProcessed:
                    return keyStates[172];
                default:
                    throw new ArgumentOutOfRangeException("key");
            }

            #endregion
        }

        public void Update()
        {
            var i = 0;
            foreach (var key in keys)
            {
                if (key != Key.None && Keyboard.IsKeyDown(key))
                {
                    keyStates[i] = true;
                }
                else
                {
                    keyStates[i] = false;
                }
                i++;
            }
        }

        /// <summary>
        /// Copies values to another keyboard state.
        /// </summary>
        /// <param name="state">The state.</param>
        public void CopyTo(KeyboardState state)
        {
            for (var i = 0; i < keys.Length; i++)
            {
                state.Set(i, keyStates[i]);
            }
        }
    }
}