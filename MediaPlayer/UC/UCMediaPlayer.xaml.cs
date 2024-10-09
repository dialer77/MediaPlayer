using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using MediaControlManager;
using WMPLib;

namespace MediaPlayer.UC
{
    /// <summary>
    /// UCMediaPlayer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UCMediaPlayer : UserControl
    {
        private MediaManager _mediaManager;
        private List<string> _currentPlaylist;
        private int _currentMediaIndex = 0;
        private Window _parentWindow;
        private DispatcherTimer _timer;
        private const double FADE_DURATION = 0.5; // 페이드 애니메이션 지속 시간 (초)
        private const double INACTIVITY_DURATION = 3000; // 비활성 시간 (초)
        private Point _lastMousePosition;
        private const double MOUSE_MOVEMENT_THRESHOLD = 5; // 마우스 움직임 임계값 (픽셀)
        private bool _isControlVisible = true;
        private bool _isUserSeeking = false;
        private DispatcherTimer _progressTimer;

        public UCMediaPlayer()
        {
            InitializeComponent();
            this.Loaded += UCMediaPlayer_Loaded;
            this.Unloaded += UCMediaPlayer_Unloaded;

            // 타이머 초기화
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(INACTIVITY_DURATION);
            _timer.Tick += Timer_Tick;

            // MediaManager 인스턴스 가져오기
            _mediaManager = MediaManager.GetInstance();

            // 재생 진행 타이머 초기화
            _progressTimer = new DispatcherTimer();
            _progressTimer.Interval = TimeSpan.FromMilliseconds(16); // 약 60fps
            _progressTimer.Tick += ProgressTimer_Tick;
            _progressTimer.Start();

        }

        private void UCMediaPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            _parentWindow = Window.GetWindow(this);
            if (_parentWindow != null)
            {
                _parentWindow.StateChanged += ParentWindow_StateChanged;
                _parentWindow.SizeChanged += ParentWindow_SizeChanged;
            }

            // 포커스 설정
            MainGrid.Focus();

            // 초기 상태 설정: 컨트롤을 표시하고 타이머 시작
            ShowControls(animate: false);

        }

        private void UCMediaPlayer_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_parentWindow != null)
            {
                _parentWindow.StateChanged -= ParentWindow_StateChanged;
                _parentWindow.SizeChanged -= ParentWindow_SizeChanged;
            }

            _timer.Tick -= Timer_Tick;
            _timer.Stop();
        }

        private void ParentWindow_StateChanged(object sender, EventArgs e)
        {
            UpdateControlOverlay();
        }

        private void ParentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateControlOverlay();
        }

        private void UCMediaPlayer_InputDetected(object sender, InputEventArgs e)
        {
            if (e is MouseEventArgs mouseEvent)
            {
                Point currentPosition = mouseEvent.GetPosition(this);
                if (_isControlVisible)
                {
                    // 컨트롤이 보이는 상태에서만 마우스 움직임을 체크
                    if (CalculateDistance(_lastMousePosition, currentPosition) > MOUSE_MOVEMENT_THRESHOLD)
                    {
                        _lastMousePosition = currentPosition;
                        ShowControls();
                    }
                }
                else if (mouseEvent.LeftButton == MouseButtonState.Pressed)
                {
                    // 컨트롤이 숨겨진 상태에서는 클릭만 감지
                    ShowControls();
                }
            }
            else if (e is KeyEventArgs)
            {
                // 키보드 입력은 항상 컨트롤을 표시
                ShowControls();
            }
        }

        private double CalculateDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        private void ShowControls(bool animate = true)
        {
            if (!_isControlVisible)
            {
                _isControlVisible = true;
                // 타이머 재시작
                _timer.Stop();
                _timer.Start();

                if (animate)
                {
                    FadeIn();
                }
                else
                {
                    ControlOverlay.Opacity = 1;
                    ControlOverlay.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // 컨트롤이 이미 보이는 상태라면 타이머만 재시작
                _timer.Stop();
                _timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            FadeOut();
        }

        private void FadeIn()
        {
            if (ControlOverlay.Visibility != Visibility.Visible)
            {
                ControlOverlay.Visibility = Visibility.Visible;
                ControlOverlay.Opacity = 0;
            }

            DoubleAnimation fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(FADE_DURATION));
            ControlOverlay.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private void FadeOut()
        {
            _isControlVisible = false;
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(FADE_DURATION));
            fadeOutAnimation.Completed += (s, _) =>
            {
                ControlOverlay.Visibility = Visibility.Collapsed;
            };
            ControlOverlay.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        }

        private void UpdateControlOverlay()
        {
            if (ControlOverlay != null && MainGrid != null)
            {
                ControlOverlay.Width = MainGrid.ActualWidth;
                ControlOverlay.Height = MainGrid.ActualHeight;

                // 오버레이 크기 업데이트 후 타이머 재시작
                ShowControls(animate: false);
            }
        }

        public void SetMedia(IWMPMedia media)
        {
            if (_currentPlaylist != null && _currentPlaylist.Count > 0)
            {
                int index = _currentPlaylist.FindIndex(x => x == media.sourceURL);
                if (index != -1)
                {
                    _currentMediaIndex = index;
                    PlayCurrentMedia();
                }
                else
                {
                    MessageBox.Show("선택한 미디어를 현재 재생 목록에서 찾을 수 없습니다.");
                }
            }
            else
            {
                MessageBox.Show("현재 재생 목록이 비어 있습니다.");
            }
        }
        private PlayListType m_playLIstType = PlayListType.MyPlayList;
        public void SetPlayList(PlayListType playListType)
        {
            m_playLIstType = playListType;
            var playlist = _mediaManager.GetPlayList(m_playLIstType);
            if (playlist != null && playlist.count > 0)
            {
                _currentPlaylist = new List<string>();
                for (int i = 0; i < playlist.count; i++)
                {
                    _currentPlaylist.Add(playlist.Item[i].sourceURL);
                }
                _currentMediaIndex = 0;
                PlayCurrentMedia();
            }
        }

        private void PlayCurrentMedia()
        {
            if (_currentPlaylist != null && _currentMediaIndex < _currentPlaylist.Count)
            {
                string currentMediaUrl = _currentPlaylist[_currentMediaIndex];
                mediaElement.Source = new Uri(currentMediaUrl);
                mediaElement.Play();

                // MediaManager를 통해 현재 미디어의 정보를 가져옵니다.
                IWMPMedia currentMedia = _mediaManager.GetMediaBySourceURL(m_playLIstType, currentMediaUrl);
                if (currentMedia != null)
                {
                    // 미디어 제목을 설정합니다.
                    TitleLabel.Content = currentMedia.getItemInfo("Title");

                    // 작곡가/가수 정보를 설정합니다.
                    string artist = currentMedia.getItemInfo("Artist");
                    string composer = currentMedia.getItemInfo("Composer");
                    string artistInfo = !string.IsNullOrEmpty(artist) ? artist : composer;
                    ArtistLabel.Content = artistInfo;
                }
                else
                {
                    // 미디어 정보를 찾지 못한 경우 기본값을 설정합니다.
                    TitleLabel.Content = "Unknown Title";
                    ArtistLabel.Content = "Unknown Artist";
                }
            }
        }

        public void Play()
        {
            mediaElement.Play();
            UpdatePlayPauseButton(true);
        }

        public void Pause()
        {
            mediaElement.Pause();
            UpdatePlayPauseButton(false);
        }

        private void UpdatePlayPauseButton(bool isPlaying)
        {
            string imagePath = isPlaying ? "pack://application:,,,/Resources/stop.png" : "pack://application:,,,/Resources/play.png";
            try
            {
                var image = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                PlayPauseButton.Background = new ImageBrush(image);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"이미지 로드 오류: {ex.Message}\n경로: {imagePath}");
            }
        }

        public void Next()
        {
            if (_currentPlaylist != null && _currentMediaIndex < _currentPlaylist.Count - 1)
            {
                _currentMediaIndex++;
                PlayCurrentMedia();
            }
        }

        public void Previous()
        {
            if (_currentPlaylist != null && _currentMediaIndex > 0)
            {
                _currentMediaIndex--;
                PlayCurrentMedia();
            }
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if(button == null)
            {
                return;
            }

            if (mediaElement.CanPause)
            {
                if (mediaElement.IsPlaying())
                {
                    Pause();
                }
                else
                {
                    Play();
                }
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            Previous();
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan)
            {
                ProgressSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                TotalTimeTextBlock.Text = mediaElement.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");
                UpdatePlayPauseButton(true);
            }
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (_currentPlaylist != null && _currentMediaIndex < _currentPlaylist.Count - 1)
            {
                // 현재 재생목록에 다음 곡이 있으면 다음 곡을 재생합니다.
                Next();
            }
            else
            {
                // 현재 재생목록의 마지막 곡이었다면 처음부터 다시 재생합니다.
                _currentMediaIndex = 0;
                PlayCurrentMedia();
            }
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan && !_isUserSeeking)
            {
                Dispatcher.InvokeAsync(() =>
                {
                    ProgressSlider.Value = mediaElement.Position.TotalSeconds;
                    UpdateTimeDisplay(mediaElement.Position);
                }, DispatcherPriority.Background);
            }
        }

        private void ProgressSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isUserSeeking = true;
        }

        private void ProgressSlider_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isUserSeeking)
            {
                UpdateTimeDisplay(TimeSpan.FromSeconds(ProgressSlider.Value));
            }
        }

        private void ProgressSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isUserSeeking = false;
            mediaElement.Position = TimeSpan.FromSeconds(ProgressSlider.Value);
            UpdateTimeDisplay(mediaElement.Position);
        }


        private void UpdateTimeDisplay(TimeSpan time)
        {
            CurrentTimeTextBlock.Text = time.ToString(@"hh\:mm\:ss");
        }

        public event RoutedEventHandler MediaOpened
        {
            add { mediaElement.MediaOpened += value; }
            remove { mediaElement.MediaOpened -= value; }
        }

        public event RoutedEventHandler MediaEnded
        {
            add { mediaElement.MediaEnded += value; }
            remove { mediaElement.MediaEnded -= value; }
        }

        public Duration NaturalDuration => mediaElement.NaturalDuration;

        public TimeSpan Position
        {
            get => mediaElement.Position;
            set => mediaElement.Position = value;
        }
    }

    public static class MediaElementExtensions
    {
        public static bool IsPlaying(this MediaElement mediaElement)
        {
            return mediaElement.Position < mediaElement.NaturalDuration.TimeSpan;
        }
    }
}