using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WMPLib;

namespace MediaControlManager
{
    public class MediaManager
    {
        
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

        public bool LoadMediaList()
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
                string mediaFolderPath = System.IO.Path.Combine(PathInfo.MusicFolder, playListType.ToString());
                if(System.IO.Directory.Exists(mediaFolderPath) == false)
                {
                    System.IO.Directory.CreateDirectory(mediaFolderPath);
                }

                string[] mediaFileNames = System.IO.Directory.GetFiles(mediaFolderPath);

                IWMPPlaylist playlist = m_musicPlayer.newPlaylist(playListType.ToString(), mediaFolderPath);
                foreach (string mediaFileName in mediaFileNames)
                {
                    IWMPMedia media = m_musicPlayer.newMedia(mediaFileName);
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
        

        private object m_csCurrentPlayList = new object();

        public void SetPlayList(PlayListType playListType)
        {
            lock (m_csCurrentPlayList)
            {
                
                m_musicPlayer.currentPlaylist = m_dicPlayList[playListType];
                Thread.Sleep(100);
                m_musicPlayer.controls.stop();
            }
        }

        public IWMPMedia GetMediaBySourceURL(PlayListType playListType, string sourceURL)
        {
            var playList = m_dicPlayList[playListType];
            for (int i = 0; i < playList.count; i++)
            {
                IWMPMedia media = playList.get_Item(i);
                if (media.sourceURL == sourceURL)
                {
                    return media;
                }
            }
            return null;
        }
    }
}
