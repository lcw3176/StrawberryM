using Android.Content;
using StrawberryM.Droid;
using StrawberryM.Services;

[assembly: Xamarin.Forms.Dependency(typeof(ForegroundHelper))]

namespace StrawberryM.Droid
{
    class ForegroundHelper : IForegroundHelper
    {
        public void StartService()
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(ForegroundService));

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                Android.App.Application.Context.StartForegroundService(intent);
            }
            else
            {
                Android.App.Application.Context.StartService(intent);
            }
        }

        public void StopService()
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(ForegroundService));

            Android.App.Application.Context.StopService(intent);
        }
    }
}