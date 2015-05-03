#region Usings

using System.Windows.Controls;
using System.Windows.Media;

#endregion

namespace ImpControls.Gui
{
    public interface IStyleLib
    {
        void LoadStyles();
        Brush GetGridBrush(bool bar);
        Brush GetForeground();
        void SetStyle(ImpBaseControl nonButton);
        void SetStyle(ImpButton button, string content);
        void SetStyle(ImpButton button, BtnNumber btnNumber);
        void SetStyle(TextBox textBox);
    }
}