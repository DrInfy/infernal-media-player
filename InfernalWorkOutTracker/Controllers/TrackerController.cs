using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Base;
using Base.Commands;
using Base.Controllers;
using Base.Interfaces;
using Base.ListLogic;

namespace InfernalWorkOutTracker.Controllers
{
    public class TrackerController :  KeyboardController<TrackerCommand>, IControllerContextMenu<TrackerCommand, ContextMenuEnum>
    {
        public WotSettings Settings { get; private set; }

        public override bool Focused
        {
            get { return true; }
        }

        public override bool Selected
        {
            get { return true; }
        }

        public IEventController EventC { get; private set; }

        public TrackerController()
        {
            Settings = new WotSettings();
        }

        public void ContextMenu(Point cursorPositionInDesktop, ContextMenuEnum menuPosition)
        {
            throw new NotImplementedException();
        }


        public override void Exec(TrackerCommand cmd, object arg = null)
        {
            switch (cmd)
            {
                case TrackerCommand.None:
                    break;
                case TrackerCommand.Open:
                    break;
                case TrackerCommand.Play:
                    break;
                case TrackerCommand.Pause:
                    break;
                case TrackerCommand.GotoPrevious:
                    break;
                case TrackerCommand.GotoNext:
                    break;
                case TrackerCommand.SaveResults:
                    break;
                case TrackerCommand.StoreScore:
                    break;
                case TrackerCommand.PanelOpen:
                    break;
                case TrackerCommand.AddFile:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("cmd");
            }
        }


        public void Update()
        {
            throw new NotImplementedException();
        }

        public override List<KeyCommand<TrackerCommand>> GenerateDefaultKeyCommands()
        {
            throw new NotImplementedException();
        }

        public void CreateContextMenu(Point cursorPositionInDesktop, List<TextAndCommand<TrackerCommand>> cmdList)
        {
            throw new NotImplementedException();
        }
    }
}
