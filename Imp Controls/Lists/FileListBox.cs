using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Base;
using Base.FileData;
using Base.Libraries;
using Base.ListLogic;
using ImpControls.SpecialFolder;

namespace ImpControls
{
    /// <summary>
    /// Listbox showing files in a folder
    /// </summary>
    public sealed class FileListBox : ImpListBox<FileImpInfo>
    {
        private readonly ComparerSelectableFileName nameComparer;
        private readonly ComparerSelectableFileDate dateComparer;
        private FileSortMode sortMode;
        private string currentPath; 
        private List<string> currentExtensions;
        private IEnumerable<string> filtersVideo;
        private IEnumerable<string> filtersMusic;
        private IEnumerable<string> filtersPictures;
        private IEnumerable<string> filtersPlaylist;


        public FileSortMode SortMode
        {
            get { return sortMode; }
            set
            {
                if (sortMode != value)
                {
                    sortMode = value;
                    Sort();
                }
            }
        }


        protected override void GetTooltip()
        {
            toolTip.Content = controller.GetContent(MouseoverIndex).Name;
        }

        public FileListBox() : base(true, true)
        {
            ItemsDragable = false;
            SortMode = FileSortMode.Name;
            nameComparer = new ComparerSelectableFileName();
            dateComparer = new ComparerSelectableFileDate();
        }

        public void SetPath(string path, List<string> extensions)
        {
            currentPath = path;
            currentExtensions = extensions;

            SetPath();
        }


        private void SetPath()
        {
            FileInfo[] fileInfos;
            try
            {
                var specialFolder = SpecialFolderLoader.LoadSpecialFolder(currentPath);
                if (specialFolder != null)
                {
                    fileInfos = new FileInfo[specialFolder.FilePaths.Count];
                    for (int i = 0; i < specialFolder.FilePaths.Count; i++)
                        fileInfos[i] = new FileInfo(specialFolder.FilePaths[i]);
                }
                else
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(currentPath);
                    fileInfos = directoryInfo.GetFiles();
                }
            }
            catch (Exception e)
            {
                // doesn't matter why path choosing failed, no files available in this folder
                controller.Clear();
                controller.AddItem(new FileImpInfo(ErrorType.FailedToOpenFolder));
                return;
            }
            var list = LibImp.FilterFiles(fileInfos, currentExtensions);


            foreach (var fileImpInfo in list)
                fileImpInfo.FileType = FileTypeFinder.DetermineFileType(fileImpInfo.Path);

            SetList(list);
            Sort();
        }


        public void SetFiles(string path, List<string> pathList, List<string> extensions)
        {
            currentPath = path;
            var fileInfos = new FileInfo[pathList.Count];
            try
            {
                for (int i = 0; i < pathList.Count; i++)
                {
                    fileInfos[i] = new FileInfo(pathList[i]);
                }
            }
            catch(Exception e)
            {
                // doesn't matter why path choosing failed, no files available in this folder
                controller.Clear();
                controller.AddItem(new FileImpInfo(ErrorType.FailedToOpenFolder));
                return;
            }
            var list = LibImp.FilterFiles(fileInfos, currentExtensions);


            foreach (var fileImpInfo in list)
                fileImpInfo.FileType = FileTypeFinder.DetermineFileType(fileImpInfo.Path);

            SetList(list);
            Sort();
        }


        protected override Brush getBrush(int i, DrawingContext drawingContext, Brush brush)
        {
            brush = base.getBrush(i, drawingContext, brush);
            if (IsEnabled && ColorCoding)
            {
                if (i == MouseoverIndex)
                {
                    if (controller.GetContent(i).FileType == FileTypes.Videos)
                        brush = new SolidColorBrush(Color.FromRgb(210, 255, 255));
                    else if (controller.GetContent(i).FileType == FileTypes.Music)
                        brush = new SolidColorBrush(Color.FromRgb(255, 255, 210));
                    else if (controller.GetContent(i).FileType == FileTypes.Pictures)
                        brush = new SolidColorBrush(Color.FromRgb(210, 210, 255));
                }
                else if (!controller.IsSelected(i))
                {
                    if (controller.GetContent(i).FileType == FileTypes.Videos)
                        brush = new SolidColorBrush(Color.FromRgb(190, 220, 220));
                    else if (controller.GetContent(i).FileType == FileTypes.Music)
                        brush = new SolidColorBrush(Color.FromRgb(220, 220, 190));
                    else if (controller.GetContent(i).FileType == FileTypes.Pictures)
                        brush = new SolidColorBrush(Color.FromRgb(190, 190, 220));
                }
            }

            return brush;
        }


        public bool ColorCoding { get; set; }


        protected override void DrawText(int index, DrawingContext drawingContext, Brush brush)
        {
            if (ActualWidth - SCROLLBARWIDTH < 0)
                return;

            

            FormattedText text = FormatText(controller.GetText(index), ref brush);
            
            if (text.MaxTextWidth > 0)
                drawingContext.DrawText(text, new Point(3, (index - LowIndex) * ISize + 3));
        }


        public override FileImpInfo GetSelected()
        {
            var item = base.GetSelected();
            if (item == null || string.IsNullOrEmpty(item.SmartName))
                return null;
            return item;
        }

        private void Sort()
        {
            if (SortMode == FileSortMode.Name)
            {
                controller.Sort(nameComparer);
            }
            else
            {
                controller.Sort(dateComparer);
            }
        }


        public void Refresh()
        {
            SetPath();
        }
    }
}
