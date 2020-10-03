using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Xamarin.Forms;
using Android.Widget;

using AndroidApp = Android.App.Application;
using Android.Graphics;
using Xamarin.Essentials;
using StrawberryM.Model;
using StrawberryM.Services;

[assembly: Dependency(typeof(StrawberryM.Droid.Notification))]
namespace StrawberryM.Droid
{
    class Notification : INotificationManager 
    {

        public event EventHandler NotificationReceived;


        public void ScheduleNotification(AppState state, string title)
        {

            if (state.Equals(AppState.onSleep))
            {
                DependencyService.Get<IForeground>().StartService();
            }

            if (state.Equals(AppState.onStart) || state.Equals(AppState.onClose) || state.Equals(AppState.onResume))
            {
                DependencyService.Get<IForeground>().StopService();
            }

        }

        public void ReceiveNotification(string resourceName)
        {
            var args = new NotificationEventArgs()
            {
                ResourceName = resourceName,
            };

            NotificationReceived?.Invoke(null, args);
        }

    }


}