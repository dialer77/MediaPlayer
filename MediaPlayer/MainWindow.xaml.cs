using MediaControlManager;
using MediaPlayer.UC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MediaPlayer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private UCMainMenu ucMainMenu = new UCMainMenu();

        private UCMediaList ucMediaList = new UCMediaList();
        
        public MainWindow()
        {
            InitializeComponent();
            UpdateMainPageUI(m_mainPageType);

            ucMainMenu.OnMusicListButtonClicked += UcMainMenu_OnMusicListButtonClicked;
        }

        private void UcMainMenu_OnMusicListButtonClicked(PlayListType type)
        {
            //MediaManager.GetInstance().SetPlayList(type);

            m_mainPageType = MainPageType.MediaList;
            UpdateMainPageUI(m_mainPageType, type);
        }

        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void buttonMaximize_Click(object sender, RoutedEventArgs e)
        {
            if(this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState= WindowState.Maximized;
            }
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ReleaseCapture();
                SendMessage(new WindowInteropHelper(this).Handle, 0x112, 0xf012, 0);
            }
        }

        private MainPageType m_mainPageType = MainPageType.MainMenu;

        private void UpdateMainPageUI(MainPageType mainPageType, PlayListType type = PlayListType.RecentMusicList)
        {
            m_mainPageType = mainPageType;

            panelMainPage.Children.Clear();
            switch (mainPageType)
            {
                case MainPageType.MainMenu:
                    panelMainPage.Children.Add(ucMainMenu);
                    buttonBeforePage.Visibility= Visibility.Hidden;
                    break;
                case MainPageType.MediaList:
                    panelMainPage.Children.Add(ucMediaList);
                    buttonBeforePage.Visibility = Visibility.Visible;
                    ucMediaList.SetPlayListType(type);
                    break;
                case MainPageType.MediaPlayer:
                    buttonBeforePage.Visibility = Visibility.Visible;
                    break;

            }
        }

        private void buttonBeforePage_Click(object sender, RoutedEventArgs e)
        {
            if (m_mainPageType == MainPageType.MediaList)
            {
                m_mainPageType= MainPageType.MainMenu;
                UpdateMainPageUI(m_mainPageType);
            }
        }
    }
}
