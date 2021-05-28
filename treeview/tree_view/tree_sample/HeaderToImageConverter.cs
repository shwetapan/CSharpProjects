using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
namespace tree_sample
{
    /// <summary>
    /// convert a full path to specific image type of a drive, folder or file
    /// </summary>
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Get the full path
            var path = (string)value;

            // if the path is null, ignore
            if (path == null)
                return null;

            //get the name of the file/folder
            var name = MainWindow.GetFileFolderName(path);

            // By default we preasume an image
            var image = "Images/file.jpg";

            // if the name is blank, we preassume it is a drive as we cannot have a blank file or folder name
            if (string.IsNullOrEmpty(name))
                image = "Images/drive.png";
            else if (new FileInfo(path).Attributes.HasFlag(FileAttributes.Directory))
                image = "Images/close.png";

            return new BitmapImage(new Uri($"pack://application:,,,/{image}"));
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
