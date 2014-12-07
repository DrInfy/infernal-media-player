using System.Windows.Controls;
using System.Windows.Input;
using Base;
using Base.Commands;
using Base.ListLogic;
using Imp.Controllers;
using ImpControls.Gui;
using Ipv;

namespace Imp.Panels
{
    /// <summary>
    /// Interaction logic for PlaylistPanel.xaml
    /// </summary>
    public partial class PlaylistPanel : UserControl
    {
        private MainController mainC;

        public PlaylistPanel()
        {
            InitializeComponent();
            ListPlaylist.LoadPlaylistItem += LoadPlayListItem;
            ListPlaylist.SetDispatcher(Dispatcher);
        }


       


        public void SetStyles(StyleLib styleLib, MainController mainController)
        {
            mainC = mainController;

            styleLib.SetStyle(ButtonClearPlaylist, BtnNumber.ClearList);

            styleLib.SetStyle(ButtonClosePanel, BtnNumber.Exit_);
            styleLib.SetStyle(ButtonMaximizePanel, BtnNumber.Maximize);

            styleLib.SetStyle(ButtonSavePlaylist, BtnNumber.Save);
            styleLib.SetStyle(ButtonRemoveFile, BtnNumber.Remove);
            styleLib.SetStyle(ButtonSort, "Sort");

            styleLib.SetStyle(TextBoxPlaylistName);
            styleLib.SetStyle(TextBoxFind);
            styleLib.SetStyle(ListPlaylist);
        }

        private void ButtonClosePanel_Clicked(object sender)
        {
            mainC.Exec(Base.Commands.ImpCommand.PanelPlaylist);
        }

        private void ListPlaylist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var playlistItem = ListPlaylist.GetSelected();
            if (playlistItem != null)
                mainC.Exec(ImpCommand.Open, playlistItem);
        }

        private void LoadPlayListItem(PlaylistItem item)
        {
            if (item != null)
                mainC.Exec(ImpCommand.Open, item);
        }

        private void ButtonClearPlaylist_Clicked(object sender)
        {
            mainC.Exec(ImpCommand.ClearPlaylist);
        }

        private void TextBoxFind_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListPlaylist.FindText = TextBoxFind.Text;
        }

        private void ButtonRemoveFile_Clicked(object sender)
        {
            mainC.Exec(ImpCommand.RemoveSelected);
            ListPlaylist.RemoveSelected();
        }

        private void ButtonSort_Clicked(object sender)
        {
            mainC.Exec(ImpCommand.Sort);
        }

        private void ButtonMaximizePanel_Clicked(object sender)
        {
            mainC.Exec(ImpCommand.PanelPlaylist, PanelCommand.MaxToggle);
        }

        
    }
}
