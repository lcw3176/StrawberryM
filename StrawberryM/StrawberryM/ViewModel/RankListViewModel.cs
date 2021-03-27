using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using StrawberryM.Model;
using StrawberryM.Services;
using Xamarin.Forms;

namespace StrawberryM.ViewModel
{
    class RankListViewModel : BaseViewModel
    {
        public ObservableCollection<RankList> RankCollection { get; set; } = new ObservableCollection<RankList>();

        private RankService serivce = new RankService();
        public ICommand ChangeGerne { get; set; }
        public ICommand SearchCommand { get; set; }

        public RankListViewModel()
        {
            ChangeGerne = new Command(ChangeGerneCommandExecute);
            SearchCommand = new Command(SearchCommandExecute);
            Task.Run(() => ChangeGerneCommandExecute("total"));
        }

        private async void SearchCommandExecute(object obj)
        {
            int rank = int.Parse(obj.ToString());

            string singer = RankCollection[rank - 1].singer;
            string song = RankCollection[rank - 1].song;

            object search = singer + " " + song;

            BrowseViewModel.GetInstance().SearchExecuteCommand(search);
            await Shell.Current.GoToAsync("//search");
        }

        private async void ChangeGerneCommandExecute(object obj)
        {
            RankCollection.Clear();

            RankCollection.Add(new RankList()
            {
                singer = "로딩중",
            });

            string type = obj.ToString();
            var result = await serivce.GetRankFromUrl(type);

            

            if(result != null)
            {
                RankCollection.Clear();

                foreach (var i in result)
                {
                    string[] values = i.Value.Split(new string[] { "<divided>" }, StringSplitOptions.None);

                    RankCollection.Add(new RankList()
                    {
                        rank = i.Key + 1,
                        singer = values[0],
                        song = values[1],
                        SearchCommand = this.SearchCommand
                    });
                }
            }

            else
            {
                DependencyService.Get<IMessageToast>().Alert("연결 오류");
            }

            
        }


    }
}
