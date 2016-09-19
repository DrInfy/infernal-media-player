#region Usings

using System.Windows.Threading;
using Imp.Base;
using Imp.Base.FileLoading;

#endregion

namespace Imp.DirectShow.Player
{
    public class MediaLoader : FileLoader<PlayerController>
    {
        public MediaLoader(Dispatcher dispatcher) : base(dispatcher) {}

        protected override PlayerController Load(string path, out ImpError error)
        {
            var playerController = new PlayerController(path, dispatcher);
            playerController.OpenSource(out error);
            return playerController;
        }
    }
}