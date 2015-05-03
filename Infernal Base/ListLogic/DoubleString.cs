namespace Base.ListLogic
{
    public class DoubleString
    {
        #region Fields

        public readonly string Value;
        public readonly string DisplayName;

        #endregion

        public DoubleString(string value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        protected bool Equals(DoubleString other)
        {
            return string.Equals(Value, other.Value) && string.Equals(DisplayName, other.DisplayName);
        }

        public override int GetHashCode()
        {
            unchecked { return ((Value?.GetHashCode() ?? 0) * 397) ^ (DisplayName?.GetHashCode() ?? 0); }
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DoubleString) obj);
        }
    }
}