
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using RemoteService.Droid.Services;
using Android.Content;
using Acr.UserDialogs;
using RemoteService.Helpers;
using Android.Util;

namespace RemoteService.Droid
{
    [Activity(Label = "RemoteService", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public SecurityServiceConnection ServiceConnection { get; private set; }
        public static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            UserDialogs.Init(this);
            LoadApplication(new App());

            if (Instance == null)
            {
                Instance = this;
            }

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void StartServiceCompat(Intent intent)
         {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                StartForegroundService(intent);
            }
            else
            {
                StartService(intent);
            }
        }

        public void ConnectToService()
        {
            Log.Debug(Constants.TAG, "MainActivity - ConnectToService()");
            if (ServiceConnection == null)
            {
                ServiceConnection = new SecurityServiceConnection(this);
            }

            var intent = new Intent(this, typeof(SecurityService));

            StartServiceCompat(intent);
            BindService(intent, ServiceConnection, Bind.AutoCreate);
            Log.Debug(Constants.TAG, "MainActivity - BindService() called");
        }

        public void DisconnectService()
        {
            Log.Debug(Constants.TAG, "MainActivity - DisconnectService()");
            if (ServiceConnection == null || !ServiceConnection.IsConnected)
                return;

            UnbindService(ServiceConnection);
            ServiceConnection = null;

            Log.Debug(Constants.TAG, "MainActivity - UnbindService() called");
        }
    }
}