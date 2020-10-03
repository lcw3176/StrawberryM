using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using StrawberryM.Model;
using StrawberryM.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace StrawberryM.ViewModel
{
    class BrowseViewModel : BaseViewModel
    {
        public ObservableCollection<Youtube> youtubeSearch { get; }

        private string searchText = string.Empty;
        public string SearchText
        {
            get { return searchText; }
            set 
            {
                searchText = value;
                OnPropertyChanged("SearchText");
            }
        }

        public Command searchCommand { get; }
        public Command downloadCommand { get; }

        public BrowseViewModel()
        {
            youtubeSearch = new ObservableCollection<Youtube>();
            searchCommand = new Command(SearchExecuteCommand);
            downloadCommand = new Command(DownloadExecuteCommand);
        }

        /// <summary>
        /// 노래 클릭 시 다운로드
        /// </summary>
        /// <param name="videoTitle"></param>
        private void DownloadExecuteCommand(object videoTitle)
        {
            string id = youtubeSearch.FirstOrDefault(e => e.title == videoTitle.ToString()).id;
            Download(id, videoTitle.ToString());
        }


        /// <summary>
        /// 유튜브에서 노래 검색
        /// </summary>
        private async void SearchExecuteCommand()
        {
            youtubeSearch.Clear();

            string apiKey = Environment.GetEnvironmentVariable("YOUTUBE_API_KEY", EnvironmentVariableTarget.User);
            string appName = Environment.GetEnvironmentVariable("YOUTUBE_Application", EnvironmentVariableTarget.User);

            var youtube = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = appName
            });

            var request = youtube.Search.List("snippet");

            request.Q = SearchText;
            request.MaxResults = 50;

            var result = await request.ExecuteAsync();

            foreach (var item in result.Items)
            {
                if (item.Id.Kind == "youtube#video")
                {

                    youtubeSearch.Add(new Youtube()
                    {
                        id = item.Id.VideoId,
                        title = item.Snippet.Title,
                        downloadCommand = this.downloadCommand,
                    });
                }
            }

            youtube.Dispose();
        }

        private bool isBusy = false;

        /// <summary>
        /// 노래 다운로드 (사운드만 추출해서 다운)
        /// </summary>
        /// <param name="videoId">비디오 고유 아이디</param>
        /// <param name="videoName">비디오 제목</param>
        private async void Download(string videoId, string videoName)
        {
            try
            {
                if(!isBusy)
                {
                    isBusy = true;
                    var youtube = new YoutubeClient();
                    var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);
                    var streamInfo = streamManifest.GetAudioOnly().WithHighestBitrate();
                    Regex pattern = new Regex(@"[\/:*?<>|]");
                    videoName = pattern.Replace(videoName, string.Empty);
                    string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    string musicDirectory = Path.Combine(rootPath, "playList");

                    if (streamInfo != null)
                    {
                        await youtube.Videos.Streams.DownloadAsync(streamInfo, musicDirectory + "/" + videoName + soundExtension);
                        EnqueuePlayList(videoName);
                        isBusy = false;
                    }
                }

                else
                {
                    DependencyService.Get<IMessage>().Alert("현재 다운중");
                }
               

            }

            catch
            {
                DependencyService.Get<IMessage>().Alert("접근 제한 알림");
                isBusy = false;
            }

        }


    }
}
