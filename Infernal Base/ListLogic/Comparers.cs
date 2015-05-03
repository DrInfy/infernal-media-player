#region Usings

using System;
using System.Collections.Generic;
using Base.FileData;
using Base.Libraries;

#endregion

namespace Base.ListLogic
{
    public class ComparerSelectableFileName : IComparer<Selectable<FileImpInfo>>
    {
        public int Compare(Selectable<FileImpInfo> x, Selectable<FileImpInfo> y)
        {
            return String.Compare(x.Content.SmartName, y.Content.SmartName, StringComparison.Ordinal);
        }
    }

    public class ComparerSelectableFileDate : IComparer<Selectable<FileImpInfo>>
    {
        public int Compare(Selectable<FileImpInfo> x, Selectable<FileImpInfo> y)
        {
            return y.Content.DateModified.CompareTo(x.Content.DateModified);
        }
    }

    public class ComparerFileName : IComparer<FileImpInfo>
    {
        public int Compare(FileImpInfo x, FileImpInfo y)
        {
            return String.Compare(x.SmartName, y.SmartName, StringComparison.Ordinal);
        }
    }

    public class ComparerFileDate : IComparer<FileImpInfo>
    {
        public int Compare(FileImpInfo x, FileImpInfo y)
        {
            return y.DateModified.CompareTo(x.DateModified);
        }
    }

    public class ComparerFolderSmart : IComparer<ImpFolder>
    {
        public int Compare(ImpFolder x, ImpFolder y)
        {
            return String.Compare(x.SmartPath, y.SmartPath, StringComparison.Ordinal);
        }
    }

    public class ComparerPlayListItemPath : IComparer<Selectable<PlaylistItem>>
    {
        public int Compare(Selectable<PlaylistItem> x, Selectable<PlaylistItem> y)
        {
            return String.Compare(x.Content.FullPath, y.Content.FullPath, StringComparison.Ordinal);
        }
    }


    public class ComparerPlayListItemRandom : IComparer<Selectable<PlaylistItem>>
    {
        public int Compare(Selectable<PlaylistItem> x, Selectable<PlaylistItem> y)
        {
            return LibImp.Rnd.Next(-10000, 10000);
        }
    }

    public class ComparerPlayListItemName : IComparer<Selectable<PlaylistItem>>
    {
        public int Compare(Selectable<PlaylistItem> x, Selectable<PlaylistItem> y)
        {
            return String.Compare(x.Content.SmartName, y.Content.SmartName, StringComparison.Ordinal);
        }
    }
}