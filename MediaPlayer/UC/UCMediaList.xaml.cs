using MediaControlManager;
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
using WMPLib;
using static System.Net.Mime.MediaTypeNames;

namespace MediaPlayer.UC
{
    /// <summary>
    /// UCMediaList.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UCMediaList : UserControl
    {
        public delegate void OnMediaItemSelect(PlayListType type, IWMPMedia media);
        public event OnMediaItemSelect OnMediaItemSelected;

        private PlayListType _listType;
        public UCMediaList()
        {
            InitializeComponent();
        }

        public void SetPlayListType(PlayListType playListType)
        {
            _listType = playListType;

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
            IWMPPlaylist mediaList = MediaManager.GetInstance().GetPlayList(playListType);

            ObservableCollection<MediaItem> mediaItems = new ObservableCollection<MediaItem>();

            for (int i = 0; i < mediaList.count; i++)
            {
                IWMPMedia music = mediaList.Item[i];

                MediaItem item = new MediaItem();

                string artist = music.getItemInfo("Artist");
                if (artist == "")
                {
                    artist = "알수없음";
                }
                mediaItems.Add(new MediaItem() { 
                    Title = music.name,
                    Singer = artist,
                    More = "..."});
            }

            listViewMediaList.ItemsSource = mediaItems;
        }
    }

    public class MediaItem
    {
        public string Title { get; set; }
        public string Singer { get; set; }
        public string More { get; set; } = "···";
    }

    public class SubtractValueConverter : IValueConverter
    {
        public double Subtract { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                return Math.Max(0, doubleValue - Subtract);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
