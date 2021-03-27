using Android.Widget;
using StrawberryM.Droid;
using StrawberryM.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(MessageToast))]
namespace StrawberryM.Droid
{
    class MessageToast : IMessageToast
    {
        public void Alert(string message)
        {
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
        }
    }
}