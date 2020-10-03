using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using StrawberryM.Droid;
using StrawberryM.Model;
using Xamarin.Essentials;
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


            RemoteViews view = new RemoteViews(AppInfo.PackageName, Resource.Layout.notification);
            view.SetTextViewText(Resource.Id.title, NowPlay.Title);
            view.SetOnClickPendingIntent(Resource.Id.playButton, playPendingIntent);
            view.SetOnClickPendingIntent(Resource.Id.beforeButton, beforePendingIntent);
            view.SetOnClickPendingIntent(Resource.Id.nextButton, nextPendingIntent);
            view.SetOnClickPendingIntent(Resource.Id.closeButton, closePendingIntent);
            view.SetOnClickPendingIntent(Resource.Id.title, titlePendingIntent);

            if (NowPlay.Audio.IsPlaying)
            {
                view.SetImageViewResource(Resource.Id.playButton, Resource.Drawable.stop);
            }

            else
            {
                view.SetImageViewResource(Resource.Id.playButton, Resource.Drawable.play);
            }


            NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                .SetContent(view)
                .SetLargeIcon(BitmapFactory.DecodeResource(AndroidApp.Context.Resources, Resource.Drawable.icon))
                .SetSmallIcon(Resource.Drawable.icon);

            var notification = builder.Build();
            notification.Flags = NotificationFlags.NoClear;

            var manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);

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