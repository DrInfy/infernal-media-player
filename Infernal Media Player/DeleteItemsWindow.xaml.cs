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
using System.Windows.Shapes;
using Base.ListLogic;
using Imp.Controllers;
using ImpControls.Gui;

namespace Imp
{
    /// <summary>
    /// Interaction logic for DeleteItemsWindow.xaml
    /// </summary>
    public partial class DeleteItemsWindow : Window
    {
        private MainController mainC;
        private List<string> paths;

        public DeleteItemsWindow(MainController mainC, List<string> paths)
        {
            this.mainC = mainC;
            this.paths = paths;
            InitializeComponent();
        }


        private void Window_Activated(object sender, EventArgs e)
        {
            DeleteButton.SetContent("Delete");
            CancelButton.SetContent("Cancel");

            ListItemsToDelete.SetList(paths);
        }


        public void SetStyles(StyleLib styling)
        {
            styling.SetStyle(DeleteButton, "Delete");
            styling.SetStyle(CancelButton, "Cancel");
            styling.SetStyle(ListItemsToDelete);            
        }

        private void DeleteButton_Clicked(object sender)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Clicked(object sender)
        {
            DialogResult = false;
            Close();
        }
    }
}
