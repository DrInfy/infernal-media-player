using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Base.Libraries;
using Base.ListLogic;
using ImpControls.SpecialFolder;
using Microsoft.WindowsAPICodePack.Shell;
using SelectionMode = Base.ListLogic.SelectionMode;

namespace ImpControls
{
    public class DirectoryListBox : ImpListBox<ImpFolder>
    {
        private string path = string.Empty;
        

        /// <summary>
        /// Read-only string representing current path
        /// </summary>
        public string Path {
            get { return path; }
        }



        public DirectoryListBox() : base(true, false)
        {
            MouseDoubleClick += OnDoubleClick;
            
        }


        protected override void UpdateItems()
        {
            base.UpdateItems();
            
        }


        protected override void GetTooltip()
        {
            toolTip.Content = controller.GetContent(MouseoverIndex).Value;
        }


        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selected = controller.GetSelected();
            if (selected != null)
                SetPath(selected.Value);
        }


        /// <summary>
        /// Sets the path.
        /// </summary>
        /// <param name="path">
        /// The path. Custom paths are formatted like "$$C:\movies\", special folders should like "$Music" and
        /// normal paths "C:\movies" or "C:\Movies\" </param>
        public void SetPath(string pathData)
        {
            path = pathData;
            ImpFolder select = null;
            // it's rather rare to have smaller structure than 10, so skip start list capacity at 16
            var list = new List<ImpFolder>(16);

            var specialFolder = SpecialFolderLoader.LoadSpecialFolder(pathData);
            if (specialFolder != null)
            {
                controller.Clear();

                // special folder
                select = new ImpFolder(pathData, pathData.Replace("$", ""));
                controller.AddItem(select, true);
                //list.Add(select);

                foreach (string folderPath in specialFolder.FolderPaths)
                    list.Add(new ImpFolder(folderPath, "  " + StringHandler.GetFilename(folderPath)));

                list.Sort(new ComparerFolderSmart());
                
                controller.AddItems(list);
            }
            else
            {
                // normal folders:    
                controller.Clear();

                // add drive
                string firstCharacter = pathData.Substring(0, 1);
                var newPath = firstCharacter.ToUpper() + @":\";
                controller.AddItem(new ImpFolder(newPath, newPath), true);

                int index = 3;
                int lastIndex = 2;
                int depth = 1;
                if (path.Length > 3)
                {
                    do
                    {
                        if (path[index] == '\\')
                        {
                            controller.AddItem(CreatePathItem(path, index, depth, lastIndex), true);
                            //list.Add(CreatePathItem(path, index, depth, lastIndex));
                            lastIndex = index;
                            depth++;
                        }
                        index++;
                    } while (index < path.Length);

                    if (path[index - 1] != '\\')
                    {
                        select = CreatePathItem(path, index, depth, lastIndex);
                        controller.AddItem(select, true);
                        //list.Add(select);
                        depth++;
                    }
                }
                
                try
                {
                    foreach (var directory in Directory.GetDirectories(path))
                    {
                        lastIndex = directory.LastIndexOf('\\');
                        list.Add(CreatePathItem(directory, directory.Length, depth, lastIndex));
                    }
                }
                catch (Exception)
                {

                }

                list.Sort(new ComparerFolderSmart());
                controller.AddItems(list);
            }
            controller.UpdateItems();
            controller.Select(select);
        }


        private static ImpFolder CreatePathItem(string path, int length, int depth, int lastIndex)
        {
            return new ImpFolder(path.Substring(0, length),
                                    new string(' ', depth * 2) + path.Substring(lastIndex + 1, length - lastIndex - 1));
        }


        public void Refresh()
        {
            var last = GetSelected();
            SetPath(path);
            if (last != null)
            {
                controller.Select(last);
            }
            if (GetSelected() == null)
                controller.Select(SelectionMode.One, 0);
        }
    }
}