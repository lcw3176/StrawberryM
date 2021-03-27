using System;
using Xamarin.Forms;
using StrawberryM.Model;
using StrawberryM.Services;

[assembly: Dependency(typeof(StrawberryM.Droid.NotificationManager))]
namespace StrawberryM.Droid
{
    class NotificationManager : INotificationManager 
    {

        public event EventHandler NotificationReceived;


        public void ScheduleNotification(AppState state, string title)
        {

            if (state.Equals(AppState.onSleep))
            {
                DependencyService.Get<IForegroundHelper>().StartService();
            }

            if (state.Equals(AppState.onStart) || state.Equals(AppState.onClose) || state.Equals(AppState.onResume))
            {
                DependencyService.Get<IForegroundHelper>().StopService();
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