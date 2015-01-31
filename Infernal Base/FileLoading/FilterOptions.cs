using System;

namespace Base.FileLoading
{
    [Flags]
    public enum FilterOptions
    {
        None = 0,
        Files = 1,
        ChildFolders = 2
    }
}