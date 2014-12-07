using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    interface IGuiBase
    {
        void SelectPlaylistItem(int index);
        void ShowLoopMode(LoopMode mode);
        void AddToPlaylist(string path);

    }
}
