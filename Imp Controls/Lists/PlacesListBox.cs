#region Usings

using System.Collections.Generic;
using System.IO;
using Imp.Base.ListLogic;
using Imp.Controls.SpecialFolder;

#endregion

namespace Imp.Controls.Lists
{
    /// <summary>
    /// Listbox showing drives and additional places like "My Pictures" folder or
    /// "My Music" or any additional user directories
    /// </summary>
    public class PlacesListBox : ImpListBox<DoubleString>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlacesListBox"/> class.
        /// </summary>
        public PlacesListBox()
            : base(false, false) {}

        protected override void GetTooltip()
        {
            toolTip.Content = controller.GetContent(MouseoverIndex).Value.Replace("$", "");
        }

        /// <summary>
        /// Loads the places.
        /// </summary>
        /// <param name="additionalPlaces">The additional places.</param>
        public void LoadPlaces(DoubleString[] additionalPlaces)
        {
            var last = GetSelected();

            var paths = new List<DoubleString>(8);
            foreach (var driveInfo in DriveInfo.GetDrives())
            {
                if (driveInfo.IsReady)
                {
                    if (string.IsNullOrEmpty(driveInfo.VolumeLabel))
                    {
                        paths.Add(new DoubleString(driveInfo.RootDirectory.Name, driveInfo.RootDirectory.Name));
                    }
                    else
                    {
                        paths.Add(new DoubleString(driveInfo.RootDirectory.Name,
                            driveInfo.RootDirectory.Name + " (" + driveInfo.VolumeLabel + ")"));
                    }
                }
            }

            controller.Clear();
            controller.AddItems(additionalPlaces);

            controller.AddItems(paths);
            controller.AddItem(new DoubleString("$" + SpecialFolderLoader.VideoFolderName, SpecialFolderLoader.VideoFolderName));
            controller.AddItem(new DoubleString("$" + SpecialFolderLoader.MusicFolderName, SpecialFolderLoader.MusicFolderName));
            controller.AddItem(new DoubleString("$" + SpecialFolderLoader.DownloadFolderName, SpecialFolderLoader.DownloadFolderName));


            if (last != null)
            {
                controller.Select(last);
            }
            if (GetSelected() == null)
                controller.Select(SelectionMode.One, 0);
        }
    }
}