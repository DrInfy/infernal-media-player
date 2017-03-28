using Imp.Base.Commands;
using Imp.Controls.Gui;
using Imp.Player.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Imp.Player.Panels
{
    /// <summary>
    /// Interaction logic for ImageViewerBottom.xaml
    /// </summary>
    public partial class ImageViewerBottom : UserControl
    {
        private MainController mainC;

        public ImageViewerBottom()
        {
            InitializeComponent();
        }

        public void SetStyles(StyleLib styleLib, MainController mainController)
        {
            mainC = mainController;

            styleLib.SetStyle(ButtonPrev, BtnNumber.Previous);
            styleLib.SetStyle(ButtonLoop, BtnNumber.Loop);
            styleLib.SetStyle(ButtonNext, BtnNumber.Next);
        }

        private void ButtonNext_Clicked(object sender)
        {
            mainC.Exec(ImpCommand.OpenNext);
        }

        private void ButtonLoop_Clicked(object sender)
        {
            mainC.Exec(ImpCommand.LoopChange);
        }

        private void ButtonPrev_Clicked(object sender)
        {
            mainC.Exec(ImpCommand.OpenPrev);
        }
    }
}
