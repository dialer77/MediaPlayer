using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WMPLib;

namespace MediaControlManager
{
    public class MediaManager
    {


        private const string FOLDER_NAME_MUSIC = "Music";
        private const string FOLDER_NAME_VIDEO = "Video";
        
        private static Lazy<MediaManager> _instance = new Lazy<MediaManager>(() => new MediaManager(), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        public static MediaManager GetInstance() => _instance.Value;

        public string LastErrorMessage { get; private set; } = string.Empty;

        private WindowsMediaPlayer m_musicPlayer = new WindowsMediaPlayer();

        private Dictionary<PlayListType, IWMPPlaylist> m_dicPlayList = new Dictionary<PlayListType, IWMPPlaylist>();

        private MediaManager()
        {
        }

        public IWMPPlaylist GetPlayList(PlayListType playListType)
        {
            return m_dicPlayList[playListType];
        }

        public bool LoadMusicList()
        {
            bool bResult = false;
            try
            {

                foreach(PlayListType playListType in Enum.GetValues(typeof( PlayListType)))
                {
                    LoadMusicFiles(playListType);
                }

                bResult = true;
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
            }
            return bResult;
        }

        private void LoadMusicFiles(PlayListType playListType)
        {
            try
            {
                string musicFolderPath = System.IO.Path.Combine(PathInfo.MusicFolder, playListType.ToString(), FOLDER_NAME_MUSIC);
                string videoFolderPath = System.IO.Path.Combine(PathInfo.MusicFolder, playListType.ToString(), FOLDER_NAME_VIDEO); 
                string[] musicFileNames = System.IO.Directory.GetFiles(musicFolderPath);
                string[] videoFileNames = System.IO.Directory.GetFiles(videoFolderPath);

                IWMPPlaylist playlist = m_musicPlayer.newPlaylist(playListType.ToString(), musicFolderPath);
                List<string> videoFiles = new List<string>();
                foreach (string musicFileName in musicFileNames)
                {
                    string musicName = System.IO.Path.GetFileNameWithoutExtension(musicFileName).Trim();
                    string videoFileName = videoFileNames.Where(name => System.IO.Path.GetFileNameWithoutExtension(name).Trim() == musicName).FirstOrDefault();

                    IWMPMedia media = m_musicPlayer.newMedia(musicFileName);
                    media.setItemInfo("VideoURL", videoFileName);
                    playlist.appendItem(media);
                }
                m_dicPlayList.Add(playListType, playlist);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public WindowsMediaPlayer MusicPlayer 
        { 
            get { return m_musicPlayer; }
        }
        


        public void SetPlayList(PlayListType playListType)
        {
            m_musicPlayer.currentPlaylist = m_dicPlayList[playListType];
            m_musicPlayer.controls.stop();
        }
    }
}
