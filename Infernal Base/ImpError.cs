using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    /// Error when something goes wrong, use null when no error occurs
    /// </summary>
    public class ImpError
    {
        private ErrorType type;
        private string text;
        static private int count;

        public string Text
        {
            get { return text; }
        }

        public ErrorType Type
        {
            get { return type; }
            set { type = value; }
        }


        public ImpError(ErrorType type)
        {
            this.type = type;
            GetErrorText(type);
        }


        private void GetErrorText(ErrorType type)
        {
            text = ++count + ", ";

            switch (type)
            {
                case ErrorType.FailedToOpenFolder:
                    text += "Failed to open Folder";
                    break;
                case ErrorType.FailedToOpenDrive:
                    text += "Failed to open drive";
                    break;
                case ErrorType.FailedToOpenFile:
                    text += "Failed to open file";
                    break;
                case ErrorType.UnknownFileType:
                    text += "Unknown file type";
                    break;
                case ErrorType.NotSupportedFile:
                    text += "File type not supported";
                    break;
                case ErrorType.FileNotFound:
                    text += "File not found";
                    break;
                case ErrorType.CouldNotRenderAudio:
                    text += "Audio could not be rendered";
                    break;
                case ErrorType.CouldNotRenderVideo:
                    text += "Video could not be rendered";
                    break;
                case ErrorType.SeekingNotSupported:
                    text += "This media file does not support seeking";
                    break;
                default:
                    text += type.ToString();
                    break;
            }
        }


        public ImpError(string path, ErrorType type)
        {
            this.type = type;
            GetErrorText(type);
            text += ": " + path;
        }

        public ImpError(ErrorType type, string text)
        {
            this.type = type;
            this.text = text;
        }
    }
}
