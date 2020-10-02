using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using StrawberryM.Model;

namespace StrawberryM.Droid
{
    [Service]
    class MediaPlayerService : Service
    {
        MediaPlayer player;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            player = NowPlay.Audio;

            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            if(player.IsPlaying)
            {
                return;
            }

            else
            {
                player.Stop();
                player.Release();
                player = null;
            }
        }
    }
}