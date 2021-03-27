using StrawberryM.Model;
using StrawberryM.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StrawberryM.ViewModel
{
    class PlayListViewModel : BaseViewModel
    {
        public Command LoadItemsCommand { get; }
        public Command playCommand { get; set; }
        public Command deleteCommand { get; set; }
        public ObservableCollection<PlayList> playListCollection 
        {
            get { return BasePlayListCollection; }
            set 
            {
                BasePlayListCollection = value;
                OnPropertyChanged("playListCollection");
            }
        }

        private FileService service = new FileService();

        public PlayListViewModel()
        {

            playListCollection = BasePlayListCollection;
            playCommand = new Command(PlayExecuteCommand);
            deleteCommand = new Command(DeleteExecuteCommand);

            Task.Run(() => ExecuteMessage());
            service.RequestPermission();

            Device.StartTimer(new TimeSpan(0, 0, 1), () => {

                if(service.CheckPermission())
                {
                    InitDisplay();
                    return false;   
                }

                return true;
            });

        }

        /// <summary>
        /// 재생목록 로딩 후 디스플레이
        /// </summary>
        private void InitDisplay()
        {
            int idx = 0;
            playListCollection.Clear();
            FileInfo[] result = service.GetSongs();
            
            if(result != null)
            {
                foreach (FileInfo i in result)
                {
                    playListCollection.Add(new PlayList()
                    {
                        index = idx,
                        name = i.Name.Replace(NowPlay.soundExtension, string.Empty),
                        playCommand = this.playCommand,
                        deleteCommand = this.deleteCommand
                    });
                    idx++;

                }
            }

        }

        /// <summary>
        /// 노래 삭제
        /// </summary>
        /// <param name="songName"></param>
        private void DeleteExecuteCommand(object songName)
        {
            if(service.DeleteSong(songName.ToString(), NowPlay.soundExtension))
            {
                InitDisplay();
            }   
        }

       

        /// <summary>
        /// 노래 재생큐에 등록
        /// </summary>
        /// <param name="fileName"></param>
        private void PlayExecuteCommand(object fileName)
        {
            EnqueueNowPlay(fileName.ToString());
        }


        /// <summary>
        /// 메세지 큐, 노래 추가되면 목록에 추가
        /// </summary>
        private void ExecuteMessage()
        {
            try
            {
                while (true)
                {
                    while (playListQueue.Count > 0)
                    {
                        string songName = playListQueue.Dequeue();
                        BasePlayListCollection.Add(new PlayList()
                        {
                            index = BasePlayListCollection.Count,
                            name = songName,
                            playCommand = this.playCommand,
                            deleteCommand = this.deleteCommand
                        });

                    }

                    Controller.Reset();
                    Controller.WaitOne(Timeout.Infinite);
                }
            }

            catch { }
        }

    }
}
