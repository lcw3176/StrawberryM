using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using StrawberryM.Model;

namespace StrawberryM.ViewModel
{
    class BaseViewModel : INotifyPropertyChanged
    {
        protected static Queue<string> playListQueue = new Queue<string>();
        protected static ManualResetEvent Controller = new ManualResetEvent(false);
        public static ObservableCollection<PlayList> PlayListCollection { get; set; } =  new ObservableCollection<PlayList>();

        protected static Queue<string> nowPlayQueue = new Queue<string>();
        protected static ManualResetEvent playController = new ManualResetEvent(false);
        
        protected string soundExtension = ".webm";

        /// <summary>
        /// 지금 재생할 곡 등록
        /// </summary>
        /// <param name="title"></param>
        protected void EnqueueNowPlay(string title)
        {
            nowPlayQueue.Enqueue(title);
            playController.Set();
        }

        /// <summary>
        /// 유저 플레이 리스트 등록
        /// </summary>
        /// <param name="music"></param>
        protected void EnqueuePlayList(string music)
        {
            playListQueue.Enqueue(music);
            Controller.Set();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
