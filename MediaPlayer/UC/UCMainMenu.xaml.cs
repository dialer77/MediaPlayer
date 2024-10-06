using MediaControlManager;
using System;
using System.Collections.Generic;
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
    /// UCMainMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UCMainMenu : UserControl
    {
        public delegate void OnMusicListButtonClick(PlayListType type);
        public event OnMusicListButtonClick OnMusicListButtonClicked;

        public UCMainMenu()
        {
            InitializeComponent();

            buttonRecentList.Tag = PlayListType.RecentMusicList;
            buttonMyPlayList.Tag = PlayListType.MyPlayList;
            buttonLastPlayList.Tag = PlayListType.LastPlayList;
            buttonTopPopularList.Tag = PlayListType.TopPopularMusicList;
        }

        private void buttonPlayList_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
            {
                return;
            }

            OnMusicListButtonClicked((PlayListType)button.Tag);
        }
    }
}
