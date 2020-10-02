using Android.App;
using Android.Content;
using StrawberryM.Droid;
using StrawberryM.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioNoisy))]
namespace StrawberryM.Droid
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { Android.Media.AudioManager.ActionAudioBecomingNoisy })]
    class AudioNoisy : BroadcastReceiver
    {
       

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == Android.Media.AudioManager.ActionAudioBecomingNoisy)
            {
                DependencyService.Get<INotificationManager>().ReceiveNotification("stopNow");
            }
        }
    }




}