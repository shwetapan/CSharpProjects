
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace tree_sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            #region constructor
            InitializeComponent();
            // Supply the control with the list of sections         
            #endregion
        }      
        #region onLoaded
        /// <summary>
        /// when the application first loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Get logical drive on machine
            foreach (var drive in Directory.GetLogicalDrives())
            {
                // Create new item for it
                var item = new TreeViewItem()
                {
                    // Set the header and path
                    Header = drive,
                    //And the full path
                    Tag = drive
                };


                //Add duumy item
                item.Items.Add(null);

                //Listen out for item being expanded
                item.Expanded += Folder_Expanded;

                // Add it to the main TreeView
                FolderView.Items.Add(item);
            }
        }
        #endregion
        #region FolderExpanded
        /// <summary>
        /// when the folder expanded, find the subfolders/files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            #region Initial Checks
            var item = (TreeViewItem)sender;
            // if the item only contains the dummy data
            if (item.Items.Count != 1 || item.Items[0] != null)
                return;

            // Clear dummy data
            item.Items.Clear();

            //Get full path
            var fullpath = (string)item.Tag;
            #endregion

            #region Get Folders
            // Create blank list for directories
            var directories = new List<string>();

            //try and get any directories from the folder
            //ignoring any issue doing so

            try
            {
                var dirs = Directory.GetDirectories(fullpath);

                if (dirs.Length > 0)
                    directories.AddRange(dirs);
            }
            catch { }

            // for each directory
            directories.ForEach(directoryPath =>
            {
                var subitem = new TreeViewItem()
                {
                    Header = GetFileFolderName(directoryPath),
                    //And tag as full path
                    Tag = directoryPath

                };
                // Add dummy item so we can expand folder
                subitem.Items.Add(null);
                // Handle expanding
                subitem.Expanded += Folder_Expanded;

                //Add this item to the parent
                item.Items.Add(subitem);
            });
            #endregion

            #region Get Files
            // Create blank list for files
            var files = new List<string>();

            //try and get any files from the folder
            //ignoring any issue doing so

            try
            {
                var fs = Directory.GetFiles(fullpath);

                if (fs.Length > 0)
                    files.AddRange(fs);
            }
            catch { }

            // for each file
            files.ForEach(filepath =>
            {
                // Create file item
                var subitem = new TreeViewItem()
                {
                    Header = GetFileFolderName(filepath),
                    //And tag as full path
                    Tag = filepath

                };

                //Add this item to the parent
                item.Items.Add(subitem);
            });
            #endregion
        }
        #endregion
        #region Helpers

        ///<summary> find the file or folder name from full path
        ///</summary>
        ///<param name="path"> The full path</param>
        ///<returns></returns>
        public static string GetFileFolderName(string path)
        {
            // if we have no path return empty
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            // make all slashes to back slashes
            var normalizedPath = path.Replace('/', '\\');

            // find the last backslash in the path
            var lastIndex = normalizedPath.LastIndexOf('\\');

            // if we dont find the backslash, return the path itself
            if (lastIndex <= 0)
                return path;

            // return the name after last back slash
            return path.Substring(lastIndex + 1);

        }
        #endregion

        private void TreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeView tv = sender as TreeView;
            tv.ContextMenu.Visibility = tv.SelectedItem == null ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        private void txtNameToSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }
    }
}
