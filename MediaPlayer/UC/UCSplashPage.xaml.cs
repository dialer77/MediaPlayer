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
    /// UCSplashPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UCSplashPage : UserControl
    {
        public delegate void OnFinishMediaLoad();
        public event OnFinishMediaLoad OnFinishMediaLoaded;

        public UCSplashPage()
        {
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality); 
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime startTime = DateTime.UtcNow;

            await Task.Run(() =>
            {
                MediaManager.GetInstance().LoadMediaList();
            });

            while((DateTime.UtcNow - startTime).TotalMilliseconds < 2000)
            {
                await Task.Delay(100);
            }

            OnFinishMediaLoaded();
        }
    }
}
