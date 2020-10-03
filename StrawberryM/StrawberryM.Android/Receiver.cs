using Android.Content;
using StrawberryM.Services;
using Xamarin.Forms;


namespace StrawberryM.Droid
{
    [BroadcastReceiver]
    class Receiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent?.Extras != null)
            {
                string resourceName = intent.Extras.GetString(ForegroundService.ResourceName);
                DependencyService.Get<INotificationManager>().ReceiveNotification(resourceName);

            }
        }
    }
}