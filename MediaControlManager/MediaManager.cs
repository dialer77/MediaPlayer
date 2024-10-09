using Common;
using MediaControlManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TagLib; // 추가

namespace MediaControlManager
{
    public class MediaManager
    {
        
        private static Lazy<MediaManager> _instance = new Lazy<MediaManager>(() => new MediaManager(), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        public static MediaManager GetInstance() => _instance.Value;

        public string LastErrorMessage { get; private set; } = string.Empty;

        private Dictionary<PlayListType, List<Media>> m_dicPlayList = new Dictionary<PlayListType, List<Media>>();

        private MediaManager()
        {


        }

        public List<Media> GetPlayList(PlayListType playListType)
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
                List<Media> playlist = new List<Media>();

                foreach (string mediaFileName in mediaFileNames)
                {
                    // TagLib를 사용하여 파일의 태그 정보를 읽습니다
                    var file = TagLib.File.Create(mediaFileName);
                    
                    string artist = file.Tag.FirstPerformer ?? "알수없음";
                    string title = file.Tag.Title ?? System.IO.Path.GetFileNameWithoutExtension(mediaFileName);

                    // Media 객체를 생성할 때 artist와 title 정보를 전달합니다
                    Media media = new Media(title, artist, mediaFileName);
                    playlist.Add(media);
                }
                m_dicPlayList.Add(playListType, playlist);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Media GetMediaBySourceURL(PlayListType playListType, string sourceURL)
        {
            var playList = m_dicPlayList[playListType];
            for (int i = 0; i < playList.Count; i++)
            {
                Media media = playList[i];
                if (media.URL == sourceURL)
                {
                    return media;
                }
            }
            return null;
        }
    }
}
