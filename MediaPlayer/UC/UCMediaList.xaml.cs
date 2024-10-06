using MediaControlManager;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using static System.Net.Mime.MediaTypeNames;

namespace MediaPlayer.UC
{
    /// <summary>
    /// UCMediaList.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UCMediaList : UserControl
    {
        public UCMediaList()
        {
            InitializeComponent();
        }

        private PlayListType _listType;

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

            //UpdateMusicList(playListType);
            imageMediaIcon.Source = source;
        }
    }
}
