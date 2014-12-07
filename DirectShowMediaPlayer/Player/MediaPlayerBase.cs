using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Base;
using MediaPlayer.Player;
using WPFMediaKit.Threading;

namespace MediaPlayer
{
    public class MediaLoader : FileLoader<PlayerController>
    {
        public MediaLoader(Dispatcher dispatcher) : base(dispatcher)
        {
        }


        protected override PlayerController Load(string path, out ImpError result)
        {
            result = null;
            throw new NotImplementedException();
        }
    }
}
