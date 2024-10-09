using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaControlManager.Models
{
    public class Media
    {
        public string Title { get; private set; }

        private string m_artist = "";
        public string Artist 
        {
            get
            {
                if(m_artist == "")
                {
                    return "알수없음";
                }
                return m_artist;
            }
        }

        public string URL { get; private set; }

        public Media(string title, string artist, string url) 
        {
            Title = title;
            if(artist != null)
            {
                m_artist = artist; 
            }
            URL = url;
        }

    }
}
