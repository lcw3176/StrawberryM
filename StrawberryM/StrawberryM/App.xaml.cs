using Android.OS;
using StrawberryM.ViewModel;

namespace StrawberryM
{
    public partial class App : Xamarin.Forms.Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
        }


        protected override void OnStart()
        {
            NowPlayingViewModel.GetInstance().AppOnResume();
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
