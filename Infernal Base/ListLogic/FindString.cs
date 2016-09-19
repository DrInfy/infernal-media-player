namespace Imp.Base.ListLogic
{
    /// <summary>
    /// Search string structure for taking account for how many times a piece of string
    /// must appear in list content string to be accepted in finds
    /// </summary>
    public class FindString
    {
        #region Fields

        /// <summary>
        /// The piece of text that is searched for in the content string 
        /// </summary>
        public string Text;

        /// <summary>
        /// The number of times Text must appear in the name for the search to accept it
        /// </summary>
        public int Count;

        #endregion

        public FindString(string text, int count)
        {
            Text = text;
            Count = count;
        }
    }
}