using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Content;
using Xamarin.Forms;
using StrawberryM.Services;

namespace StrawberryM.Droid
{
    [Activity(
        Label = "StrawberryM", 
        LaunchMode = LaunchMode.SingleTop, 
        Icon = "@drawable/icon", 
        Theme = "@style/MainTheme", 
        MainLauncher = true, 
        ExcludeFromRecents = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            CreateNotificationFromIntent(Intent);

            Intent intent = new Intent(this, typeof(MediaPlayerService));
            StartService(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            CreateNotificationFromIntent(intent);
        }

        void CreateNotificationFromIntent(Intent intent)
        {
            if (intent?.Extras != null)
            {
                string resoureName = intent.Extras.GetString(Notification.ResourceName);
                DependencyService.Get<INotificationManager>().ReceiveNotification(resoureName);
            }
        }

    }

}