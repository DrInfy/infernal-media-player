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
    public class MpvLoader : FileLoader<PlayerController>
    {
        private Player player;
        private volatile bool isLoading;
        private string errorMessage;

        public MpvLoader(Dispatcher dispatcher, Player player) : base(dispatcher)
        {
            this.player = player;
        }

        protected override PlayerController Load(string path, out ImpError error)
        {
            var controller = new PlayerController();
            this.player.player.MediaError += Player_MediaError;
            this.player.player.MediaLoaded += Player_MediaLoaded;
            this.isLoading = true;
            this.player.player.Load(path, true);

            while (this.isLoading)
            {
                Thread.Sleep(5);
            }

            error = this.errorMessage != null 
                ? new ImpError(ErrorType.FailedToOpenFile, this.errorMessage) 
                : null;

            return controller;
        }

        private void Player_MediaLoaded(object sender, EventArgs e)
        {
            this.isLoading = false;
        }

        private void Player_MediaError(object sender, EventArgs e)
        {
            this.isLoading = false;
            this.errorMessage = e.ToString();
        }
    }
}
