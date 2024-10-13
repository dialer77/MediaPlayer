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
using MediaControlManager.Models;
using System.Diagnostics; // Stopwatch를 사용하기 위해 추가

namespace MediaPlayer.UC
{
    /// <summary>
    /// UCMediaPlayer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UCMediaPlayer : UserControl
    {
        private const double INACTIVITY_DURATION = 3000; // 비활성 시간 (초)
        private const double INACTIVITY_CHECK_INTERVAL = 500; // 0.5초마다 체크
        private const double MOUSE_MOVEMENT_THRESHOLD = 5; // 마우스 움직임 임계값 (픽셀)
        private const double FADE_DURATION = 0.5; // 페이드 애니메이션 지속 시간 (초)

        private Window _parentWindow;

        private List<string> _currentPlaylist = null;
        private int _currentMediaIndex = 0;
        
        private Point _lastMousePosition;
        private bool _isControlVisible = true;
        private bool _isUserSeeking = false;
        private bool _isPlaying = false;
        private bool _isMediaLoaded = false;

        private DispatcherTimer m_timerUpdateUI = null;
        private DateTime _lastActivityTime = DateTime.UtcNow;

        public UCMediaPlayer()
        {
            InitializeComponent();

            // UI 관련 업데이트 타이머
            m_timerUpdateUI = new DispatcherTimer();
            m_timerUpdateUI.Interval = TimeSpan.FromMilliseconds(16);
            m_timerUpdateUI.Tick += TimerUpdateUI_Tick;
        }

        private void UCMediaPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            _parentWindow = Window.GetWindow(this);
        }

        private void TimerUpdateUI_Tick(object sender, EventArgs e)
        {
            try
            {
                if((DateTime.UtcNow - _lastActivityTime).TotalMilliseconds > INACTIVITY_DURATION)
                {
                    if (_isControlVisible)
                    {
                        FadeOut();
                    }
                }

                if (mediaElement.NaturalDuration.HasTimeSpan && !_isUserSeeking)
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        ProgressSlider.Value = mediaElement.Position.TotalSeconds;
                        UpdateTimeDisplay(mediaElement.Position);
                    }, DispatcherPriority.Normal);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
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
                    double mouseDistance = Math.Sqrt(Math.Pow(currentPosition.X - _lastMousePosition.X, 2) + Math.Pow(currentPosition.Y - _lastMousePosition.Y, 2));

                    if (mouseDistance > MOUSE_MOVEMENT_THRESHOLD)
                    {
                        _lastMousePosition = currentPosition;
                        ResetInactivityTimer();
                    }

                    if (mouseEvent.LeftButton == MouseButtonState.Pressed)
                    {
                        ResetInactivityTimer();
                        
                        // Check if the click is on any of the control elements
                        if (!(mouseEvent.Source is Button) && !(mouseEvent.Source is Slider))
                        {
                            if (_isPlaying)
                            {
                                Pause();
                            }
                            else
                            {
                                Play();
                            }
                        }
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
       
        private void ResetInactivityTimer()
        {
            _lastActivityTime = DateTime.UtcNow;
        }

        public void ShowControls(bool animate = true)
        {
            if (!_isControlVisible)
            {
                _isControlVisible = true;
                ResetInactivityTimer();

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
                ResetInactivityTimer();
            }
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

        public void SetMedia(Media media)
        {
            if (_currentPlaylist != null && _currentPlaylist.Count > 0)
            {
                int index = _currentPlaylist.FindIndex(x => x == media.URL);
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
            var playlist = MediaManager.GetInstance().GetPlayList(m_playLIstType);
            if (playlist != null && playlist.Count > 0)
            {
                _currentPlaylist = new List<string>();
                for (int i = 0; i < playlist.Count; i++)
                {
                    _currentPlaylist.Add(playlist[i].URL);
                }
                _currentMediaIndex = 0;
                PlayCurrentMedia();
            }
        }

        private void PlayCurrentMedia()
        {
            if (_currentPlaylist != null && _currentMediaIndex < _currentPlaylist.Count)
            {
                _isMediaLoaded = false; // 새 미디어를 로드할 때 초기화
                string currentMediaUrl = _currentPlaylist[_currentMediaIndex];
                mediaElement.Source = new Uri(currentMediaUrl);
                mediaElement.Play();

                // MediaManager를 통해 현재 미디어의 정보를 가져옵니다.
                Media currentMedia = MediaManager.GetInstance().GetMediaBySourceURL(m_playLIstType, currentMediaUrl);
                if (currentMedia != null)
                {
                    TitleLabel.Content = currentMedia.Title;
                    ArtistLabel.Content = currentMedia.Artist;
                }
                else
                {
                    TitleLabel.Content = "Unknown Title";
                    ArtistLabel.Content = "Unknown Artist";
                }
            }
        }

        public void Play()
        {
            mediaElement.Play();
            _isPlaying = true;
            UpdatePlayPauseButton(true);
        }

        public void Pause()
        {
            mediaElement.Pause();
            _isPlaying = false;
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
            if (_isPlaying)
            {
                Pause();
            }
            else
            {
                Play();
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
                _isMediaLoaded = true;
                ProgressSlider.Minimum = 0;
                ProgressSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                TotalTimeTextBlock.Text = mediaElement.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");
                _isPlaying = true;
                UpdatePlayPauseButton(true);

                // Slider 활성화
                ProgressSlider.IsEnabled = true;
            }
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            _isPlaying = false;
            UpdatePlayPauseButton(false);
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

            // Slider 비활성화
            ProgressSlider.IsEnabled = false;
        }


        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isMediaLoaded && !_isUserSeeking)
            {
                mediaElement.Position = TimeSpan.FromSeconds(e.NewValue);
                UpdateTimeDisplay(mediaElement.Position);
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
            if (_isMediaLoaded)
            {
                _isUserSeeking = false;
                mediaElement.Position = TimeSpan.FromSeconds(ProgressSlider.Value);
                UpdateTimeDisplay(mediaElement.Position);
            }
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

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue == false)
            {
                if (_parentWindow != null)
                {
                    _parentWindow.StateChanged -= ParentWindow_StateChanged;
                    _parentWindow.SizeChanged -= ParentWindow_SizeChanged;
                }

                mediaElement.Stop();
                _isControlVisible = false;
                m_timerUpdateUI.Stop();
            }
            else
            {
                if (_parentWindow != null)
                {
                    _parentWindow.StateChanged += ParentWindow_StateChanged;
                    _parentWindow.SizeChanged += ParentWindow_SizeChanged;
                }

                ShowControls(animate: false);
                m_timerUpdateUI.Start();
            }
        }

        // IsPlaying 메서드 수정
        public bool IsPlaying()
        {
            return _isPlaying;
        }
    }
}