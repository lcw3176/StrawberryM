using StrawberryM.Model;
using StrawberryM.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using Xamarin.Forms;

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

        private YoutubeService service = new YoutubeService();

        public Command searchCommand { get; }
        public Command downloadCommand { get; }

        private static BrowseViewModel instance;
        public static BrowseViewModel GetInstance()
        {
            if(instance == null)
            {
                instance = new BrowseViewModel();
            }

            return instance;

        }

        private BrowseViewModel()
        {
            youtubeSearch = new ObservableCollection<Youtube>();
            searchCommand = new Command(SearchExecuteCommand);
            downloadCommand = new Command(DownloadExecuteCommand);
        }

        /// <summary>
        /// 노래 클릭 시 다운로드
        /// </summary>
        /// <param name="videoTitle"></param>
        private async void DownloadExecuteCommand(object videoTitle)
        {
            string id = youtubeSearch.FirstOrDefault(e => e.title == videoTitle.ToString()).id;
            string reshapedVideoName = await service.Download(id, videoTitle.ToString());

            if(!string.IsNullOrEmpty(reshapedVideoName))
            {
                EnqueuePlayList(reshapedVideoName);
            }
        }


        /// <summary>
        /// 유튜브에서 노래 검색
        /// </summary>
        public async void SearchExecuteCommand(object obj)
        {
            youtubeSearch.Clear();
            SearchText = obj.ToString();

            var result = await service.Search(obj.ToString());

            foreach (var item in result.Items)
            {
                if (item.Id.Kind == "youtube#video")
                {

                    youtubeSearch.Add(new Youtube()
                    {
                        id = item.Id.VideoId,
                        title = HttpUtility.HtmlDecode(item.Snippet.Title),
                        downloadCommand = this.downloadCommand,
                    });
                }
            }

            
        }
    }
}
