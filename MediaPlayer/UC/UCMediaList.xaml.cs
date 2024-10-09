using MediaControlManager;
using MediaControlManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MediaPlayer.UC
{
    /// <summary>
    /// UCMediaList.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UCMediaList : UserControl
    {
        public delegate void OnMediaItemSelect(PlayListType type, Media media);
        public event OnMediaItemSelect OnMediaItemSelected;

        private PlayListType _listType;
        public UCMediaList()
        {
            InitializeComponent();

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
        }

        public void SetPlayListType(PlayListType playListType)
        {
            if(playListType != PlayListType.None)
            {
                _listType = playListType;
            }
            UpdateUI(_listType);
        }

        private void UpdateUI(PlayListType playListType)
        {
            ImageSource source = null;
            switch (playListType)
            {
                case PlayListType.RecentMusicList:
                    source = new BitmapImage(new Uri("pack://application:,,,/Resources/RecentListIcon.png"));
                    break;
                case PlayListType.MyPlayList:
                    source = new BitmapImage(new Uri("pack://application:,,,/Resources/MyPlayListIcon.png"));
                    break;
                case PlayListType.LastPlayList:
                    source = new BitmapImage(new Uri("pack://application:,,,/Resources/LastPlayListIcon.png"));
                    break;
                case PlayListType.TopPopularMusicList:
                    source = new BitmapImage(new Uri("pack://application:,,,/Resources/TopPopularListIcon.png"));
                    break;
            }

            UpdateMusicList(playListType);
            imageMediaIcon.Source = source;
        }

        private void UpdateMusicList(PlayListType playListType)
        {
            List<Media> mediaList = MediaManager.GetInstance().GetPlayList(playListType);

            listViewMediaList.ItemsSource = mediaList;
        }

        private void listViewMediaList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            Media selectedMedia = (Media)listViewMediaList.SelectedItem;
            if(selectedMedia == null)
            {
                return;
            }
            OnMediaItemSelected(_listType, selectedMedia);
        }
    }

    public class SubtractValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is double totalWidth && values[1] is double subtractWidth)
            {
                return Math.Max(0, totalWidth - subtractWidth);
            }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MultiplyAndSubtractConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3 &&
                values[0] is double totalWidth &&
                values[1] is double multiplier &&
                values[2] is double subtractValue)
            {
                return Math.Max(0, (totalWidth * multiplier) - subtractValue);
            }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MultiplyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width && parameter is string multiplierString)
            {
                if (double.TryParse(multiplierString, out double multiplier))
                {
                    return width * multiplier;
                }
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
