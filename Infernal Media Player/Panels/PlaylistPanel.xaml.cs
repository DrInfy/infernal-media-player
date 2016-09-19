#region Usings

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Imp.Base;
using Imp.Base.Commands;
using Imp.Base.ListLogic;
using Imp.Controls.Gui;
using Imp.Player.Controllers;

#endregion

namespace Imp.Player.Panels
{
    /// <summary>
    /// Interaction logic for PlaylistPanel.xaml
    /// </summary>
    public partial class PlaylistPanel : UserControl
    {
        #region Fields

        private MainController mainC;

        #endregion

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

            styleLib.SetStyle(ButtonClosePanel, BtnNumber.Close);
            styleLib.SetStyle(ButtonMaximizePanel, BtnNumber.Maximize);

            styleLib.SetStyle(ButtonRemoveFile, BtnNumber.Remove);
            styleLib.SetStyle(ButtonSort, BtnNumber.SortByName);

            styleLib.SetStyle(ButtonClearFind, BtnNumber.Close);

            //styleLib.SetStyle(LabelTopic);
            styleLib.SetStyle(TextBoxFind);
            styleLib.SetStyle(ListPlaylist);
        }

        private void ButtonClosePanel_Clicked(object sender)
        {
            mainC.Exec(ImpCommand.PanelPlaylist);
        }

        private void ListPlaylist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var playlistItem = ListPlaylist.GetSelected();
            if (playlistItem != null)
                mainC.Exec(ImpCommand.Open, playlistItem);
        }

        private void ListPlaylist_OnDoubleTouchDown(object sender, TouchEventArgs e)
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
            ButtonClearFind.Visibility = string.IsNullOrWhiteSpace(TextBoxFind.Text) ? Visibility.Hidden : Visibility.Visible;
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

        private void ButtonClearFind_OnClicked(object sender)
        {
            TextBoxFind.Clear();
        }

        private void ListPlaylist_OnOpenRightClick_Menu(object sender, MouseButtonEventArgs e)
        {
            mainC.ContextMenu(mainC.CursorPositionInDesktop(e), ContextMenuEnum.Playlist);
        }
    }
}