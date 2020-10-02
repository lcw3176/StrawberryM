using Android.Content;
using Android.Media;
using Android.Runtime;
using StrawberryM.Droid;
using StrawberryM.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioFocusManager))]
namespace StrawberryM.Droid
{
    class AudioFocusManager : IFocus
    {
        AudioManager manager;
        AudioManager.IOnAudioFocusChangeListener listener;


        private class FocusListener : Java.Lang.Object, Android.Media.AudioManager.IOnAudioFocusChangeListener
        {

            public void OnAudioFocusChange([GeneratedEnum] AudioFocus focusChange)
            {
                switch (focusChange)
                {
                    case AudioFocus.Loss:
                        DependencyService.Get<INotificationManager>().ReceiveNotification("stopNow");
                        break;
                    default:
                        break;
                }
            }
        }

        public void RequestFocus()
        {
            listener = new FocusListener();

            manager = (AudioManager)Android.App.Application.Context.GetSystemService(Context.AudioService);
            manager.RequestAudioFocus(listener, Stream.Music, AudioFocus.Gain);
        }


        public void ReleaseAudioResources()
        {
            if (listener != null)
            {
                manager.AbandonAudioFocus(listener);
            }

        }
    }
}