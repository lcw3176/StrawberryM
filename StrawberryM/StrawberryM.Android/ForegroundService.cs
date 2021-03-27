
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using AndroidX.Core.App;
using StrawberryM.Model;
using static AndroidX.Media.App.NotificationCompat;
using AndroidApp = Android.App.Application;

namespace StrawberryM.Droid
{
    [Service]
    class ForegroundService : Service
    {

        public const string ResourceName = "resourceName";
        const string channelId = "default";
        const string channelName = "Default";
        const string channelDescription = "The default channel for notifications.";
        int serviceNotifID = 1234;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            StartNotification();

            return StartCommandResult.Sticky;
        }
        
        private void StartNotification()
        {
            Intent playintent = new Intent(AndroidApp.Context, typeof(Receiver));
            playintent.PutExtra(ResourceName, "playButton");

            Intent beforeintent = new Intent(AndroidApp.Context, typeof(Receiver));
            beforeintent.PutExtra(ResourceName, "beforeButton");

            Intent nextIntent = new Intent(AndroidApp.Context, typeof(Receiver));
            nextIntent.PutExtra(ResourceName, "nextButton");

            Intent closeIntent = new Intent(AndroidApp.Context, typeof(Receiver));
            closeIntent.PutExtra(ResourceName, "closeButton");

            Intent titleIntent = new Intent(AndroidApp.Context, typeof(MainActivity));
            titleIntent.PutExtra(ResourceName, "contentTitle");

            PendingIntent playPendingIntent = PendingIntent.GetBroadcast(AndroidApp.Context, 1, playintent, PendingIntentFlags.UpdateCurrent);
            PendingIntent beforePendingIntent = PendingIntent.GetBroadcast(AndroidApp.Context, 2, beforeintent, PendingIntentFlags.UpdateCurrent);
            PendingIntent nextPendingIntent = PendingIntent.GetBroadcast(AndroidApp.Context, 3, nextIntent, PendingIntentFlags.UpdateCurrent);
            PendingIntent closePendingIntent = PendingIntent.GetBroadcast(AndroidApp.Context, 4, closeIntent, PendingIntentFlags.UpdateCurrent);
            PendingIntent titlePendingIntent = PendingIntent.GetActivity(AndroidApp.Context, 5, titleIntent, PendingIntentFlags.UpdateCurrent);


            NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                .SetContentText(NowPlay.Title)
                .SetLargeIcon(ThumbnailUtils.CreateVideoThumbnail(NowPlay.path, ThumbnailKind.MiniKind))
                .SetSmallIcon(Resource.Drawable.icon)
                .SetContentIntent(titlePendingIntent)
                .SetShowWhen(false)
                .SetAutoCancel(true)
                .AddAction(Resource.Drawable.prev_mini, "", beforePendingIntent);

            if (NowPlay.Audio.IsPlaying)
            {
                builder.AddAction(Resource.Drawable.pause_mini, "", playPendingIntent);
            }

            else
            {
                builder.AddAction(Resource.Drawable.play_mini, "", playPendingIntent);
            }

            builder.AddAction(Resource.Drawable.next_mini, "", nextPendingIntent)
                   .AddAction(Resource.Drawable.close_mini, "", closePendingIntent)
                   .SetStyle(new MediaStyle()
                   .SetShowActionsInCompactView(0, 1, 2));

            var notification = builder.Build();
            notification.Flags = NotificationFlags.NoClear;

            var manager = (Android.App.NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
                {
                    Description = channelDescription
                };

                channel.SetSound(null, null);
                channel.SetVibrationPattern(new long[] { 0 });
                channel.EnableVibration(true);
                manager.CreateNotificationChannel(channel);
            }

            StartForeground(serviceNotifID, notification);
        }


    }
}