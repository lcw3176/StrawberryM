using System.IO;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using StrawberryM.Model;

namespace StrawberryM.Services
{
    class FileService
    {

        public string nowFilePath;

        /// <summary>
        /// 재생목록 불러오기
        /// </summary>
        public FileInfo[] GetSongs()
        {
            string rootPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).ToString();
            string musicDirectory = Path.Combine(rootPath, "strawberryPlayList");


            if (!Directory.Exists(musicDirectory))
            {
                Java.IO.File dir = new Java.IO.File(musicDirectory);
                dir.Mkdir();
                return null;
            }


            DirectoryInfo directoryInfo = new DirectoryInfo(musicDirectory);

            return directoryInfo.GetFiles();
        }


        /// <summary>
        /// 노래 삭제
        /// </summary>
        public bool DeleteSong(string songName, string soundExtension)
        {
            string rootPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).ToString();
            string musicDirectory = Path.Combine(rootPath, "strawberryPlayList");
            string deletePath = Path.Combine(musicDirectory, songName.ToString() + soundExtension);

            FileInfo file = new FileInfo(deletePath);

            if (file.Exists)
            {
                file.Delete();
            }

            return true;
        }

        /// <summary>
        /// 파일 스트림 가져오기
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public Stream GetStreamFromFile(string filename)
        {
            string rootPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).ToString();
            string musicDirectory = Path.Combine(rootPath, "strawberryPlayList");
            nowFilePath = Path.Combine(musicDirectory, filename + NowPlay.soundExtension);

            var stream = new FileStream(nowFilePath, FileMode.Open, FileAccess.Read);

            return stream;
        }


        /// <summary>
        /// 저장소 권한 얻기
        /// </summary>
        /// <returns></returns>
        public void RequestPermission()
        {
            var status = CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();

            if (status.Result != PermissionStatus.Granted)
            {
                CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();
            }
        }


        public bool CheckPermission()
        {
            var status = CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();

            if (status.Result != PermissionStatus.Granted)
            {
                return false;
            }

            return true;
        }


    }
}
