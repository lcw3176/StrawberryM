using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using StrawberryM.Model;
using Xamarin.Forms;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace StrawberryM.Services
{
    class YoutubeService
    {
        private bool isBusy = false;

        /// <summary>
        /// 유튜브 동영상 검색
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public async Task<SearchListResponse> Search(string searchText)
        {
            const string apiKey = "apikey";
            const string appName = "appName";

            
            var youtube = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = appName
            });

            var request = youtube.Search.List("snippet");

            request.Q = searchText;
            request.MaxResults = 50;

            return await request.ExecuteAsync();
        }

        /// <summary>
        /// 노래 다운로드 (사운드만 추출해서 다운)
        /// </summary>
        /// <param name="videoId">비디오 고유 아이디</param>
        /// <param name="videoName">비디오 제목</param>
        public async Task<string> Download(string videoId, string videoName)
        {
            try
            {
                if (!isBusy)
                {
                    isBusy = true;
                    var youtube = new YoutubeClient();
                    var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);
                    var streamInfo = streamManifest.GetMuxed().WithHighestVideoQuality();
                    Regex pattern = new Regex(@"[\/:*?<>|]");
                    videoName = pattern.Replace(videoName, string.Empty);
                    string rootPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).ToString();
                    string musicDirectory = Path.Combine(rootPath, "strawberryPlayList");
                    string saveFilePath = Path.Combine(musicDirectory, videoName + NowPlay.soundExtension);

                    if (streamInfo != null)
                    {
                        await youtube.Videos.Streams.DownloadAsync(streamInfo, saveFilePath);
                        isBusy = false;

                        return videoName;
                    }

                    return null;
                }

                else
                {
                    DependencyService.Get<IMessageToast>().Alert("현재 다운중");

                    return null;
                }


            }

            catch
            {
                DependencyService.Get<IMessageToast>().Alert("접근 제한 알림");
                isBusy = false;

                return null;
            }

        }
    
    }
}
