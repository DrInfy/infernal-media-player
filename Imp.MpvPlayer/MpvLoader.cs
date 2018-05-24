using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Imp.Base;
using Imp.Base.FileLoading;

namespace Imp.MpvPlayer
{
    public class MpvLoader : FileLoader<bool>
    {
        private Player player;

        public MpvLoader(Dispatcher dispatcher, Player player) : base(dispatcher)
        {
            this.player = player;
        }

        protected override bool Load(string path, out ImpError error)
        {
            var result = this.player.StartLoad(path);

            while (result.IsLoading)
            {
                Thread.Sleep(5);
            }

            error = result.Error;

            this.player.FinalizeLoad(result);

            return result.Error != null;
        }
    }
}
