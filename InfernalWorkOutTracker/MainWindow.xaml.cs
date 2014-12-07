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
using ImpControls.Gui;
using InfernalWorkOutTracker.Controllers;

namespace InfernalWorkOutTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TrackerController mainC = new TrackerController();
        public MainWindow()
        {
            InitializeComponent();
            var style = new StyleLib();
            style.LoadStyles();
            OpenPanel.SetStyles(style, mainC);
        }
    }
}
