using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using RemoteService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteService.Droid.Services
{
    public class SecurityServiceConnection : Java.Lang.Object, IServiceConnection
    {
        static readonly string TAG = Constants.TAG;
        public bool IsConnected { get; private set; }
        public Messenger Messenger { get; private set; }

        public SecurityServiceConnection(MainActivity activity)
        {
            IsConnected = false;
            Messenger = null;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            IsConnected = service != null;
            Messenger = new Messenger(service);

            var msg = "ServiceConnection - ";
            Log.Debug(TAG, $"ServiceConnection - OnServiceConnected() {name.ClassName}");

            if (IsConnected)
            {
                msg += $"bound to service {name.ClassName}";
            }
            else
            {
                msg += $"not bound to service {name.ClassName}";
            }

            Log.Info(TAG, msg);
            Toast.MakeText(Application.Context, msg, ToastLength.Long).Show();
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Log.Debug(TAG, $"ServiceConnection - OnServiceDisconnected() {name.ClassName}");
            IsConnected = false;
            Messenger = null;
            Toast.MakeText(Application.Context, $"ServiceConnection - not bound to service {name.ClassName}", ToastLength.Long).Show();
        }

    }
}