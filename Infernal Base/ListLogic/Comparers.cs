﻿#region Usings

using System;
using System.Collections.Generic;
using Imp.Base.FileData;
using Imp.Base.Libraries;

#endregion

namespace Imp.Base.ListLogic
{
    public class ComparerSelectableFileName : IComparer<Selectable<FileImpInfo>>
    {
        public int Compare(Selectable<FileImpInfo> x, Selectable<FileImpInfo> y)
        {
            return string.Compare(x.Content.SmartName, y.Content.SmartName, StringComparison.Ordinal);
        }
    }

    public class ComparerSelectableViewThenName : IComparer<Selectable<FileImpInfo>>
    {
        public int Compare(Selectable<FileImpInfo> x, Selectable<FileImpInfo> y)
        {
            if (x.Content.LastUsage != null && y.Content.LastUsage != null)
            {
                if (x.Content.LastUsage.Value > y.Content.LastUsage.Value)
                {
                    return -1;
                }

                if (x.Content.LastUsage.Value < y.Content.LastUsage.Value)
                {
                    return 1;
                }

                return 0;
            }

            if (x.Content.LastUsage == null && y.Content.LastUsage != null)
            {
                return -1;
            }

            if (x.Content.LastUsage != null && y.Content.LastUsage == null)
            {
                return 1;
            }

            return string.Compare(x.Content.SmartName, y.Content.SmartName, StringComparison.Ordinal);
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
            return string.Compare(x.SmartName, y.SmartName, StringComparison.Ordinal);
        }
    }

    public class ComparerFileDate : IComparer<FileImpInfo>
    {
        public int Compare(FileImpInfo x, FileImpInfo y)
        {
            return y.DateModified.CompareTo(x.DateModified);
        }
    }

    public class ComparerViewThenName : IComparer<FileImpInfo>
    {
        public int Compare(FileImpInfo x, FileImpInfo y)
        {
            if (x.LastUsage != null && y.LastUsage != null)
            {
                if (x.LastUsage.Value > y.LastUsage.Value)
                {
                    return -1;
                }

                if (x.LastUsage.Value < y.LastUsage.Value)
                {
                    return 1;
                }

                return 0;
            }

            if (x.LastUsage == null && y.LastUsage != null)
            {
                return -1;
            }

            if (x.LastUsage != null && y.LastUsage == null)
            {
                return 1;
            }

            return string.Compare(x.SmartName, y.SmartName, StringComparison.Ordinal);
        }
    }

    public class ComparerFolderSmart : IComparer<ImpFolder>
    {
        public int Compare(ImpFolder x, ImpFolder y)
        {
            return string.Compare(x.SmartPath, y.SmartPath, StringComparison.Ordinal);
        }
    }

    public class ComparerPlayListItemPath : IComparer<Selectable<PlaylistItem>>
    {
        public int Compare(Selectable<PlaylistItem> x, Selectable<PlaylistItem> y)
        {
            return string.Compare(x.Content.FullPath, y.Content.FullPath, StringComparison.Ordinal);
        }
    }

    public class ComparerPlayListItemDate : IComparer<Selectable<PlaylistItem>>
    {
        public int Compare(Selectable<PlaylistItem> x, Selectable<PlaylistItem> y)
        {
            return y.Content.DateModified.CompareTo(x.Content.DateModified);
        }
    }

    public class ComparerPlayListItemRandom : IComparer<Selectable<PlaylistItem>>
    {
        public int Compare(Selectable<PlaylistItem> x, Selectable<PlaylistItem> y)
        {
            return LibImp.Rnd.Next(int.MinValue, int.MaxValue);
        }
    }

    public class ComparerPlayListItemName : IComparer<Selectable<PlaylistItem>>
    {
        public int Compare(Selectable<PlaylistItem> x, Selectable<PlaylistItem> y)
        {
            return string.Compare(x.Content.SmartName, y.Content.SmartName, StringComparison.Ordinal);
        }
    }
}