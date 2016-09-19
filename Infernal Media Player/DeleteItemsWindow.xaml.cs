#region Usings

using System;
using System.Collections.Generic;
using System.Windows;
using Imp.Controls.Gui;
using Imp.Player.Controllers;

#endregion

namespace Imp.Player
{
    /// <summary>
    /// Interaction logic for DeleteItemsWindow.xaml
    /// </summary>
    public partial class DeleteItemsWindow : Window
    {
        #region Fields

        private readonly List<string> paths;
        private MainController mainC;

        #endregion

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