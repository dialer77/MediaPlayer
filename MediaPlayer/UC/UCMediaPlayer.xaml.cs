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
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MediaPlayer.UC
{
    /// <summary>
    /// UCMediaPlayer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UCMediaPlayer : UserControl
    {
        public UCMediaPlayer()
        {
            InitializeComponent();
            
            // WindowsFormsHost에 Windows Media Player 컨트롤 추가
            //var host = new WindowsFormsHost();
            //var mediaPlayerControl = new System.Windows.Forms.Panel();
            //host.Child = mediaPlayerControl;
            //windowsFormsHost.Child = mediaPlayerControl;
        }
    }
}
