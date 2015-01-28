using Base.Libraries;

namespace Base.ListLogic
{
    public class ImpFolder : DoubleString
    {
        public string SmartPath;
        public ImpFolder(string fullPath, string displayName)
            : base(fullPath, displayName)
        {
            SmartPath = StringHandler.GetSmartName(fullPath);
        }
    }
}
