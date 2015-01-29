namespace Base.ListLogic
{
    public class Selectable<T>
    {
        public bool Selected { get; set; }
        public bool SelectedIndex { get; set; }

        public bool NeverFilter { get; set; }

        public T Content { get; set; }


        public Selectable(T content)
        {
            Content = content;
        }

        public string PresentText
        {
            get
            {
                // return reference to our string if T is string
                // string.ToString would create a new instance, which could cause an
                // impact in performance
                var str = Content as string; 
                return str ?? Content.ToString();
            }
        }
    }
}