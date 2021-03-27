using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plugin.SimpleAudioPlayer;
using StrawberryM.Model;
using StrawberryM.Services;
using Xamarin.Forms;

namespace StrawberryM.ViewModel
{
    class NowPlayingViewModel : BaseViewModel
    {
        private ManualResetEvent rotateController;
        private ManualResetEvent sliderController;

        private double Rotation = 0;
        private double SliderMax = 1;
        private double SliderValue = 0;
        private string PlayButtonImage = "stop.jpg";
        private bool isRotate = false;
        INotificationManager notificationManager;

        private PropertyService service = new PropertyService();
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

            playStateCommand = new Command(PlayStateChangeCommand);
            sliderDragCommand = new Command(SliderDragExecuteCommand);
            changePlayMode = new Command(ChangePlayModecommand);
            beforeSongCommand = new Command(BeforeSongExecuteCommand);
            nextSongCommand = new Command(NextSongExecuteCommand);

            rotateController = new ManualResetEvent(false);
            sliderController = new ManualResetEvent(false);

            Task.Run(() => RotateView());
            Task.Run(() => DequeueSong());
            Task.Run(() => ChangeViewInfo());

            audio.PlaybackEnded += Audio_PlaybackEnded;
        }

        /// <summary>
        /// 오디오 종료 시 다음곡 넘김
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Audio_PlaybackEnded(object sender, EventArgs e)
        {
            isRotate = false;
            ContinueNext();
        }



        /// <summary>
        /// 다음곡으로 넘기기 버튼
        /// </summary>
        public void NextSongExecuteCommand()
        {
            if(audio != null)
            {
                
                int nextIndex = BasePlayListCollection.FirstOrDefault(e => e.name == title).index + 1;

                // 만약 마지막 노래 재생이 끝난 상황이라면
                // 처음 곡으로 돌려보낸다.
                if (nextIndex == BasePlayListCollection.Count)
                {
                    nextIndex = 0;
                }

                EnqueueNowPlay(BasePlayListCollection[nextIndex].name);
            }

        }

        /// <summary>
        /// 이전곡으로 넘기기 버튼
        /// </summary>
        public void BeforeSongExecuteCommand()
        {
            if(audio != null)
            {
                int beforeIndex = BasePlayListCollection.FirstOrDefault(e => e.name == title).index - 1;

                // 만약 현재 재생곡이 처음 노래였다면
                // 마지막 곡으로 보낸다.
                if (beforeIndex < 0)
                {
                    beforeIndex = BasePlayListCollection.Count - 1;
                }

                EnqueueNowPlay(BasePlayListCollection[beforeIndex].name);
            }

        }

        /// <summary>
        /// 재생 모드 변경
        /// </summary>
        private void ChangePlayModecommand(object playMode)
        {
            if(playMode.ToString().Equals("Whole"))
            {
                NowPlay.playMode = PlayMode.Whole;
                DependencyService.Get<IMessageToast>().Alert("전체 반복 모드");
            }

            if (playMode.ToString().Equals("One"))
            {
                NowPlay.playMode = PlayMode.One;
                DependencyService.Get<IMessageToast>().Alert("한곡 반복 모드");
            }

            if (playMode.ToString().Equals("Random"))
            {
                NowPlay.playMode = PlayMode.Random;
                DependencyService.Get<IMessageToast>().Alert("랜덤 재생 모드");
            }

        }

        /// <summary>
        /// 유저가 슬라이더바 조작했을 시
        /// </summary>
        private void SliderDragExecuteCommand()
        {
            audio.Seek(sliderValue);
        }


        /// <summary>
        /// 재생 상태 변경
        /// </summary>
        public void PlayStateChangeCommand()
        {
            if(audio == null)
            {
                return;
            }

            if(audio.IsPlaying)
            {
                isRotate = false;

                DependencyService.Get<IAudioFocus>().ReleaseAudioResources();
                audio.Pause();
                playButtonImage = "play.jpg";
                service.SetLastInfo(NowPlay.playMode.ToString());
            }

            else
            {
                isRotate = true;

                DependencyService.Get<IAudioFocus>().RequestFocus();
                audio.Play();
                playButtonImage = "stop.jpg";

                rotateController.Set();
                sliderController.Set();
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
                DependencyService.Get<IAudioFocus>().ReleaseAudioResources();
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
        /// 노래 플레이시 화면에 lp판 회전 애니메이션
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
            DependencyService.Get<IAudioFocus>().ReleaseAudioResources();
            
            FileService fileService = new FileService();

            audio = CrossSimpleAudioPlayer.Current;
            audio.Load(fileService.GetStreamFromFile(title));

            NowPlay.path = fileService.nowFilePath;
            audio.Play();

            
            DependencyService.Get<IAudioFocus>().RequestFocus();
            playButtonImage = "stop.jpg";

            totalTime = TimeSpan.FromSeconds(audio.Duration);
            sliderMax = totalTime.TotalSeconds;

            isRotate = true;
            rotateController.Set();
            sliderController.Set();

            SongInfoChanged();
        }


        /// <summary>
        /// 노래 진행상황 view와 동기화( 슬라이더 동작 ) 
        /// </summary>
        private void ChangeViewInfo()
        {
            while (true)
            {
                while (audio.IsPlaying)
                {
                    nowTime = TimeSpan.FromSeconds(audio.CurrentPosition);
                    sliderValue = nowTime.TotalSeconds;

                    if (NowAppState.state.Equals(AppState.onClose))
                    {
                        break;
                    }

                    Thread.Sleep(1000);
                }


                sliderController.Reset();
                sliderController.WaitOne(Timeout.Infinite);

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
                NextSongExecuteCommand();
            }

            if (NowPlay.playMode.Equals(PlayMode.Random))
            {
                Random rand = new Random();
                int nextIndex = rand.Next(0, BasePlayListCollection.Count);

                EnqueueNowPlay(BasePlayListCollection[nextIndex].name);
            }
        }


        /// <summary>
        /// notification에서 받아오는 정보 처리
        /// </summary>
        /// <param name="resourceName"></param>
        private void ShowNotification(string resourceName)
        {

            if(string.IsNullOrEmpty(resourceName))
            {
                return;
            }
            
            if (resourceName.Equals("playButton"))
            {
                PlayStateChangeCommand();
            }
            
            if (resourceName.Equals("beforeButton"))
            {
                BeforeSongExecuteCommand();
            }
            
            if (resourceName.Equals("nextButton"))
            {
                NextSongExecuteCommand();
            }
            
            if (resourceName.Equals("closeButton"))
            {
                CloseCommand();
                NowAppState.state = AppState.onClose;
                notificationManager.ScheduleNotification(NowAppState.state, title);

                // 정지 구간 기록
                if (!audio.IsPlaying)
                {
                    service.SetLastInfo(NowPlay.playMode.ToString());
                }

            }
            
            // 포커즈 잃었거나 이어폰 분리 시
            if (resourceName.Equals("stopNow"))
            {
                StopAudio();
            }

        }

        /// <summary>
        /// notification에서 닫기 버튼 눌렀을 때
        /// </summary>
        private void CloseCommand()
        {
            if (audio.IsPlaying)
            {
                audio.Pause();
                DependencyService.Get<IAudioFocus>().ReleaseAudioResources();
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

                // 플레이 중이 아니라면 정지 구간 기록
                if(!audio.IsPlaying)
                {
                    service.SetLastInfo(NowPlay.playMode.ToString());
                }
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
