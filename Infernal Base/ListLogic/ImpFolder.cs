#region Usings

using Base.Libraries;

#endregion

namespace Base.ListLogic
{
    public class ImpFolder : DoubleString
    {
        #region Fields

        public string SmartPath;

        #endregion

        public ImpFolder(string fullPath, string displayName)
            : base(fullPath, displayName)
        {
            SmartPath = StringHandler.GetSmartName(fullPath);
        }
    }
}