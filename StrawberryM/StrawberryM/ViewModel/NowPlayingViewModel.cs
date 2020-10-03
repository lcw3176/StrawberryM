using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Org.Apache.Http.Impl.Client;
using Plugin.SimpleAudioPlayer;
using StrawberryM.Model;
using StrawberryM.Services;
using Xamarin.Forms;

namespace StrawberryM.ViewModel
{
    class NowPlayingViewModel : BaseViewModel
    {
        private ManualResetEvent rotateController;

        private double Rotation = 0;
        private double SliderMax = 1;
        private double SliderValue = 0;
        private string PlayButtonImage = "stop.jpg";
        private bool isRotate = false;
        Thread rotationThread;
        Thread changeSongThread;
        INotificationManager notificationManager;


        public Command changePlayMode { get; set; }
        public Command playStateCommand { get; set; }
        public Command sliderDragCommand { get; set; }
        public Command beforeSongCommand { get; set; }
        public Command nextSongCommand { get; set; }

        public ISimpleAudioPlayer audio
        {
            get { return NowPlay.Audio; }
            set 
            {
                NowPlay.Audio = value;
            }
        }

        public string playButtonImage
        {
            get { return PlayButtonImage; }
            set 
            {
                PlayButtonImage = value;
                OnPropertyChanged("playButtonImage");
            }
        }

        public double sliderMax
        {
            get { return SliderMax; }
            set 
            {
                SliderMax = value;
                OnPropertyChanged("sliderMax");
            }
        }
        public double sliderValue
        {
            get { return SliderValue; }
            set {
                SliderValue = value;
                OnPropertyChanged("sliderValue");
            }
        }

        public TimeSpan nowTime
        {
            get { return NowPlay.NowTime; }
            set 
            {
                NowPlay.NowTime = value;
                OnPropertyChanged("nowTime");
            }
        }

        public TimeSpan totalTime
        {
            get { return NowPlay.TotalTime; }
            set 
            {
                NowPlay.TotalTime = value;
                OnPropertyChanged("totalTime");
            }
        }

        public string title
        {
            get { return NowPlay.Title; }
            set 
            {
                NowPlay.Title = value;
                OnPropertyChanged("title");
            }
        }

        public double rotation 
        {
            get { return Rotation; }
            set 
            {
                Rotation = value;
                OnPropertyChanged("rotation");
            }
        }

        /// <summary>
        /// 싱글톤 패턴
        /// </summary>
        /// <returns></returns>
        public static NowPlayingViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new NowPlayingViewModel();
            }

            return instance;
        }

        private static NowPlayingViewModel instance;


        private NowPlayingViewModel()
        {
            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) => {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.ResourceName);
            };

            playStateCommand = new Command(playStateChangeCommand);
            sliderDragCommand = new Command(sliderDragExecuteCommand);
            changePlayMode = new Command(changePlayModecommand);
            beforeSongCommand = new Command(beforeSongExecuteCommand);
            nextSongCommand = new Command(nextSongExecuteCommand);

            rotateController = new ManualResetEvent(false);

            rotationThread = new Thread(RotateView);
            rotationThread.Start();

            changeSongThread = new Thread(DequeueSong);
            changeSongThread.Start();
        }



        /// <summary>
        /// 다음곡으로 넘기기 버튼
        /// </summary>
        public void nextSongExecuteCommand()
        {
            if(audio != null)
            {
                
                int nextIndex = PlayListCollection.FirstOrDefault(e => e.name == title).index + 1;

                // 만약 마지막 노래 재생이 끝난 상황이라면
                // 처음 곡으로 돌려보낸다.
                if (nextIndex == PlayListCollection.Count)
                {
                    nextIndex = 0;
                }

                EnqueueNowPlay(PlayListCollection[nextIndex].name);
            }

        }

        /// <summary>
        /// 이전곡으로 넘기기 버튼
        /// </summary>
        public void beforeSongExecuteCommand()
        {
            if(audio != null)
            {
                int beforeIndex = PlayListCollection.FirstOrDefault(e => e.name == title).index - 1;

                // 만약 현재 재생곡이 처음 노래였다면
                // 마지막 곡으로 보낸다.
                if (beforeIndex < 0)
                {
                    beforeIndex = PlayListCollection.Count - 1;
                }

                EnqueueNowPlay(PlayListCollection[beforeIndex].name);
            }

        }

        /// <summary>
        /// 재생 모드 변경
        /// </summary>
        private void changePlayModecommand(object playMode)
        {
            if(playMode.ToString().Equals("Whole"))
            {
                NowPlay.playMode = PlayMode.Whole;
                DependencyService.Get<IMessage>().Alert("전체 반복 모드");
            }

            if (playMode.ToString().Equals("One"))
            {
                NowPlay.playMode = PlayMode.One;
                DependencyService.Get<IMessage>().Alert("한곡 반복 모드");
            }

            if (playMode.ToString().Equals("Random"))
            {
                NowPlay.playMode = PlayMode.Random;
                DependencyService.Get<IMessage>().Alert("랜덤 재생 모드");
            }

        }

        /// <summary>
        /// 유저가 슬라이더바 조작했을 시
        /// </summary>
        private void sliderDragExecuteCommand()
        {
            audio.Seek(sliderValue);
        }


        /// <summary>
        /// 재생 상태 변경
        /// </summary>
        public void playStateChangeCommand()
        {
            if(audio == null)
            {
                return;
            }

            if(audio.IsPlaying)
            {
                isRotate = false;

                DependencyService.Get<IFocus>().ReleaseAudioResources();
                audio.Pause();
                playButtonImage = "play.jpg";
            }

            else
            {
                isRotate = true;
                rotateController.Set();

                DependencyService.Get<IFocus>().RequestFocus();
                audio.Play();
                playButtonImage = "stop.jpg";
            }

            SongInfoChanged();
        }


        /// <summary>
        /// 이어폰 분리 (Noisy) 혹은 다른 앱 실행 ( AudioFocus.Loss ) 시 노래 정지
        /// </summary>
        private void StopAudio()
        {
            if (audio.IsPlaying)
            {
                DependencyService.Get<IFocus>().ReleaseAudioResources();
                audio.Pause();
                playButtonImage = "play.jpg";
            }

            SongInfoChanged();
        }

        /// <summary>
        /// 노래 재생, 변경 시 정보 등록
        /// </summary>
        private void DequeueSong()
        {
            try
            {
                while (true)
                {
                    while (nowPlayQueue.Count > 0)
                    {
                        title = nowPlayQueue.Dequeue();
                        PlayAudio(title);
                    }

                    playController.Reset();
                    playController.WaitOne(Timeout.Infinite);
                }
            }

            catch { }
        }

        /// <summary>
        /// 노래 플레이시 화면에 lp판 회전 애니메이션, label 움직이는 효과
        /// </summary>
        private void RotateView()
        {
            while (true)
            {
                while (isRotate)
                {
                    if (rotation >= 360f)
                    {
                        rotation = 0;
                    }

                    if(NowAppState.state.Equals(AppState.onClose))
                    {
                        break;
                    }

                    rotation += 1;
                    Thread.Sleep(10);

                }

                rotateController.Reset();
                rotateController.WaitOne(Timeout.Infinite);
            }
        }


        /// <summary>
        /// 노래 재생
        /// </summary>
        /// <param name="title"></param>
        private void PlayAudio(string title)
        {
            DependencyService.Get<IFocus>().ReleaseAudioResources();

            audio = CrossSimpleAudioPlayer.Current;
            audio.Load(GetStreamFromFile(title));
            audio.Play();

            DependencyService.Get<IFocus>().RequestFocus();
            playButtonImage = "stop.jpg";

            totalTime = TimeSpan.FromSeconds(audio.Duration);
            sliderMax = totalTime.TotalSeconds;

            Task.Run(() => 
            {
                ChangeViewInfo();
            });

            isRotate = true;
            rotateController.Set();
            
            SongInfoChanged();
        }

        private System.IO.Stream GetStreamFromFile(string filename)
        {
            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string musicDirectory = Path.Combine(rootPath, "playList");
            string filePath = Path.Combine(musicDirectory, filename + soundExtension);

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            return stream;
        }


        /// <summary>
        /// 노래 진행상황 view와 동기화, 노래 재생 끝나면 다음 곡으로 체인지
        /// </summary>
        private void ChangeViewInfo()
        {
            while (true)
            {
                if (audio == null)
                {
                    break;
                }

                if (audio.IsPlaying)
                {
                    nowTime = TimeSpan.FromSeconds(audio.CurrentPosition);
                    sliderValue = nowTime.TotalSeconds;

                    if (NowAppState.state.Equals(AppState.onClose))
                    {
                        break;
                    }

                }

                else
                {
                    if (sliderValue >= sliderMax - 1)
                    {
                        ContinueNext();
                        break;
                    }
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 다음곡으로 자동 재생
        /// </summary>
        private void ContinueNext()
        {
            if(NowPlay.playMode.Equals(PlayMode.One))
            {
                EnqueueNowPlay(title);
            }

            if (NowPlay.playMode.Equals(PlayMode.Whole))
            {
                nextSongExecuteCommand();
            }

            if (NowPlay.playMode.Equals(PlayMode.Random))
            {
                Random rand = new Random();
                int nextIndex = rand.Next(0, PlayListCollection.Count);

                EnqueueNowPlay(PlayListCollection[nextIndex].name);
            }
        }


        /// <summary>
        /// notification에서 받아오는 정보 처리
        /// </summary>
        /// <param name="resourceName"></param>
        private void ShowNotification(string resourceName)
        {

            Device.BeginInvokeOnMainThread(() => {

                if(string.IsNullOrEmpty(resourceName))
                {
                    return;
                }

                if (resourceName.Equals("playButton"))
                {
                    playStateChangeCommand();
                }

                if (resourceName.Equals("beforeButton"))
                {
                    beforeSongExecuteCommand();
                }

                if (resourceName.Equals("nextButton"))
                {
                    nextSongExecuteCommand();
                }

                if (resourceName.Equals("closeButton"))
                {
                    CloseCommand();
                    NowAppState.state = AppState.onClose;
                    notificationManager.ScheduleNotification(NowAppState.state, title);
                }

                // 포커즈 잃었거나 이어폰 분리 시
                if (resourceName.Equals("stopNow"))
                {
                    StopAudio();
                }

            });
        }

        /// <summary>
        /// notification에서 닫기 버튼 눌렀을 때
        /// </summary>
        private void CloseCommand()
        {
            if (audio.IsPlaying)
            {
                audio.Pause();
                DependencyService.Get<IFocus>().ReleaseAudioResources();
                playButtonImage = "play.jpg";
            }
        }

        /// <summary>
        /// notification에게 새로운 곡 제목 알려주기
        /// </summary>
        private void SongInfoChanged()
        {
            notificationManager.ScheduleNotification(NowAppState.state, title);
        }

        /// <summary>
        /// 앱 시작할때 notification 제거
        /// </summary>
        public void AppOnResume()
        {
            if(audio == null)
            {
                return;
            }

            sliderValue = nowTime.TotalSeconds;

            if (audio.IsPlaying)
            {
                isRotate = true;
                rotateController.Set();
            }

            NowAppState.state = AppState.onResume;
            notificationManager.ScheduleNotification(NowAppState.state, title);
        }

        /// <summary>
        /// 앱 sleep 모드일때 notification 띄우기
        /// </summary>
        public void AppOnSleep()
        {
            if (audio == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(title))
            {
                isRotate = false;

                NowAppState.state = AppState.onSleep;
                notificationManager.ScheduleNotification(NowAppState.state, title);
            }

            // 재생할 노래가 정해지지 않았을땐 notification 띄우지 않음
            else
            {
                NowAppState.state = AppState.onClose;
                notificationManager.ScheduleNotification(NowAppState.state, title);
            }

        }

    }
}
