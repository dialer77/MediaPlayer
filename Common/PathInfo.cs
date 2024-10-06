using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class PathInfo
    {
        private const string MUSIC_FOLDER_NAME = "Music";

        public static string MusicFolder
        {
            get
            {
                string startPath = System.Windows.Forms.Application.StartupPath;
                return System.IO.Path.Combine(System.IO.Directory.GetParent(startPath).FullName, MUSIC_FOLDER_NAME);
            }
        }
    }
}
