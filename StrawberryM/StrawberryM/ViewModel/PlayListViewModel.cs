using StrawberryM.Model;
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
        private Thread QueueThread;

        public string searchPlayList { get; set; }
        public Command LoadItemsCommand { get; }
        public Command playCommand { get; }
        public Command deleteCommand { get; }
        public ObservableCollection<PlayList> playListCollection { get; set; }
        public static int idx;

        public PlayListViewModel()
        {
            playListCollection = PlayListCollection;
            playCommand = new Command(playExecuteCommand);
            deleteCommand = new Command(deleteExecuteCommand);

            LoadSong();

            QueueThread = new Thread(ExecuteMessage);
            QueueThread.Start();
        }

        /// <summary>
        /// 노래 삭제
        /// </summary>
        /// <param name="songName"></param>
        private void deleteExecuteCommand(object songName)
        {
            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string musicDirectory = Path.Combine(rootPath, "playList");
            string deletePath = Path.Combine(musicDirectory, songName.ToString() + soundExtension);

            FileInfo file = new FileInfo(deletePath);

            if(file.Exists)
            {
                file.Delete();
            }

            LoadSong();
        }

        /// <summary>
        /// 재생목록 불러오기
        /// </summary>
        private void LoadSong()
        {
            idx = 0;
            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string musicDirectory = Path.Combine(rootPath, "playList");

            if (!Directory.Exists(musicDirectory))
            {
                Directory.CreateDirectory(musicDirectory);

                return;
            }

            PlayListCollection.Clear();

            DirectoryInfo directoryInfo = new DirectoryInfo(musicDirectory);

            FileInfo[] files = directoryInfo.GetFiles();
            
            foreach(var i in files)
            {
                PlayListCollection.Add(new PlayList()
                {
                    index = idx,
                    name = i.Name.Replace(soundExtension, string.Empty),
                    playCommand = this.playCommand,
                    deleteCommand = this.deleteCommand
                });
                idx++;

            }
        }

        /// <summary>
        /// 노래 재생큐에 등록
        /// </summary>
        /// <param name="fileName"></param>
        private void playExecuteCommand(object fileName)
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
                        PlayListCollection.Add(new PlayList()
                        {
                            index = idx,
                            name = songName,
                            playCommand = this.playCommand,
                            deleteCommand = this.deleteCommand
                        });
                        idx++;


                    }

                    Controller.Reset();
                    Controller.WaitOne(Timeout.Infinite);
                }
            }

            catch { }
        }

    }
}
