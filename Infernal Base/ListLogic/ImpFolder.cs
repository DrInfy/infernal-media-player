#region Usings

using Imp.Base.Libraries;

#endregion

namespace Imp.Base.ListLogic
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