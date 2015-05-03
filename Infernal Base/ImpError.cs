namespace Base
{
    public enum ErrorType
    {
        // file browser error types
        FailedToOpenFolder,
        FailedToOpenDrive,

        // load based errors
        FailedToOpenFile,
        UnknownFileType,
        NotSupportedFile,
        FileNotFound,

        // mediaplayer type errors
        CouldNotRenderAudio,
        CouldNotRenderVideo,
        SeekingNotSupported
    }


    /// <summary>
    ///     Error when something goes wrong, use null when no error occurs
    /// </summary>
    public class ImpError
    {
        #region Static Fields and Constants

        private static int count;

        #endregion

        #region Properties

        public string Text { get; private set; }
        public ErrorType Type { get; set; }

        #endregion

        public ImpError(ErrorType type)
        {
            Type = type;
            GetErrorText();
        }

        public ImpError(string path, ErrorType type)
        {
            Type = type;
            GetErrorText();
            Text += ": " + path;
        }

        public ImpError(ErrorType type, string text)
        {
            Type = type;
            Text = text;
        }

        private void GetErrorText()
        {
            Text = ++count + ", ";

            switch (Type)
            {
                case ErrorType.FailedToOpenFolder:
                    Text += "Failed to open Folder";
                    break;
                case ErrorType.FailedToOpenDrive:
                    Text += "Failed to open drive";
                    break;
                case ErrorType.FailedToOpenFile:
                    Text += "Failed to open file";
                    break;
                case ErrorType.UnknownFileType:
                    Text += "Unknown file type";
                    break;
                case ErrorType.NotSupportedFile:
                    Text += "File type not supported";
                    break;
                case ErrorType.FileNotFound:
                    Text += "File not found";
                    break;
                case ErrorType.CouldNotRenderAudio:
                    Text += "Audio could not be rendered";
                    break;
                case ErrorType.CouldNotRenderVideo:
                    Text += "Video could not be rendered";
                    break;
                case ErrorType.SeekingNotSupported:
                    Text += "This media file does not support seeking";
                    break;
                default:
                    Text += Type.ToString();
                    break;
            }
        }
    }
}