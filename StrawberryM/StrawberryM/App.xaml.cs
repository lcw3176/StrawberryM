using System;
using StrawberryM.Model;
using StrawberryM.Services;
using StrawberryM.ViewModel;

namespace StrawberryM
{
    public partial class App : Xamarin.Forms.Application
    {
        private PropertyService service = new PropertyService();

        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            NowPlayingViewModel.GetInstance().AppOnResume();

            object mode = service.GetDataFromProperties(FileState.PlayMode.ToString());

            if (mode != null)
            {
                NowPlay.playMode = (PlayMode)Enum.Parse(typeof(PlayMode), mode.ToString());
            }
        }


        protected override void OnSleep()
        {
            NowPlayingViewModel.GetInstance().AppOnSleep();
        }

        protected override void OnResume()
        {
            NowPlayingViewModel.GetInstance().AppOnResume();
        }

    }


}
