namespace Imp.Base.ListLogic
{
    public class Selectable<T>
    {
        #region Properties

        public bool Selected { get; set; }
        public bool SelectedIndex { get; set; }
        public bool NeverFilter { get; set; }
        public T Content { get; set; }

        public string PresentText
        {
            get
            {
                // return reference to our string if T is string
                // string.ToString would create a new instance, which could cause an
                // impact in performance
                var str = Content as string;
                return str ?? Content.ToString() ?? "";
            }
        }

        #endregion

        public Selectable(T content)
        {
            Content = content;
        }
    }
}