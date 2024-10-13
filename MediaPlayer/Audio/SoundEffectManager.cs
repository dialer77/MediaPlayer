using System;
using System.Windows;
using System.Media;
using System.Diagnostics;
using System.Windows.Resources;

namespace MediaPlayer.Audio
{
    public class SoundEffectManager
    {
        private static SoundEffectManager _instance;
        private SoundPlayer _clickSoundPlayer;

        private SoundEffectManager()
        {
            InitializeClickSound();
        }

        public static SoundEffectManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SoundEffectManager();
                }
                return _instance;
            }
        }

        private void InitializeClickSound()
        {
            try
            {
                Uri resourceUri = new Uri("pack://application:,,,/Resources/clickSound.wav", UriKind.Absolute);
                StreamResourceInfo info = Application.GetResourceStream(resourceUri);

                if (info == null)
                {
                    // 디버그 정보 추가
                    Debug.WriteLine($"Resource not found: {resourceUri}");
                    
                    // 파일 시스템에서 직접 로드 시도
                    string localPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "clickSound.wav");
                    if (System.IO.File.Exists(localPath))
                    {
                        _clickSoundPlayer = new SoundPlayer(localPath);
                        _clickSoundPlayer.Load();
                        Debug.WriteLine($"Sound loaded from file system: {localPath}");
                        return;
                    }
                    
                    throw new InvalidOperationException("Could not load clickSound.wav from resources or file system");
                }

                _clickSoundPlayer = new SoundPlayer(info.Stream);
                _clickSoundPlayer.Load(); // 동기적으로 로드
                Debug.WriteLine($"Sound loaded successfully from: {resourceUri}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"효과음 로드 오류: {ex.Message}");
                MessageBox.Show($"효과음 로드 오류: {ex.Message}");
            }
        }

        public void PlayClickSound()
        {
            try
            {
                Debug.WriteLine("Attempting to play click sound");
                _clickSoundPlayer?.Play();
                Debug.WriteLine("Click sound played");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"효과음 재생 오류: {ex.Message}");
                MessageBox.Show($"효과음 재생 오류: {ex.Message}");
            }
        }
    }
}
