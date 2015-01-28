namespace Base
{
    interface IGuiBase
    {
        void SelectPlaylistItem(int index);
        void ShowLoopMode(LoopMode mode);
        void AddToPlaylist(string path);

    }
}
