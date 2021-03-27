using System;
using StrawberryM.Model;

namespace StrawberryM.Services
{
    /// <summary>
    /// notification 통신 인터페이스
    /// </summary>
    public interface INotificationManager
    {

            event EventHandler NotificationReceived;

            void ScheduleNotification(AppState state, string title);

            void ReceiveNotification(string resourceName);

    }



    public class NotificationEventArgs : EventArgs
    {
        public string ResourceName { get; set; }
    }
}
