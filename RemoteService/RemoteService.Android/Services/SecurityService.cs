using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using RemoteService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace RemoteService.Droid.Services
{
    [Service(Name = "com.xerasystems.SecurityService1", Exported = true, Process = ":securityservice_process")]
    public class SecurityService : Service
    {
        static readonly int NotificationId = 51232;
        static readonly string ChannelId = "XERA_CHANNEL_01";
        static readonly string TAG = Constants.TAG;
        private Messenger messenger;
        private bool isStarted;

        private CancellationTokenSource alertCancellation;

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug(TAG, "Service - OnCreate()");
            messenger = new Messenger(new ServiceHandler(this));

            CreateNotificationChannel();
            RegisterForeground();
        }

        public override IBinder OnBind(Intent intent)
        {
            Log.Debug(TAG, "Service - OnBind()");
            return messenger.Binder;
        }

        public override bool OnUnbind(Intent intent)
        {
            Log.Debug(TAG, "Service - OnUnbind()");
            return base.OnUnbind(intent);
            //return true;
        }

        public override void OnDestroy()
        {
            Log.Debug(TAG, "Service - OnDestroy()");
            isStarted = false;
            base.OnDestroy();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Log.Debug(Constants.TAG, "OnStartCommand()");

            if (isStarted)
            {
                var action = intent.Action;

                if (action == "CANCEL_ALERT")
                {
                    alertCancellation?.Cancel();
                }

                Log.Info(Constants.TAG, "OnStartCommand() - Service already started");

            } 
            else
            {
                isStarted = true;
                RegisterForeground();
                Log.Debug(Constants.TAG, "OnStartCommand() - Registered as foreground service");
            }

            return StartCommandResult.Sticky;
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                return;

            var channel = new NotificationChannel(ChannelId, "Default Channel", NotificationImportance.Default)
            {
                Description = "Default notification channel."
            };
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        private void RegisterForeground()
        {
            var notification = GetNotification();
            notification.Flags = NotificationFlags.ForegroundService | NotificationFlags.NoClear;

            StartForeground(NotificationId, notification);
            //var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            //notificationManager.Notify(NotificationId, notification);
        }

        private Notification GetNotification()
        {
            return new NotificationCompat.Builder(this, ChannelId)
                .SetSmallIcon(Resource.Drawable.shield1)
                
                .SetContentTitle("Security Service")
                .SetContentText("Rest assured, service is running")
                //.SetProgress(100, 50, true)
                .SetOngoing(true)
                .Build();
        }

        // Implement methods
        public async Task<string> GetLocation()
        {
            Log.Debug(TAG, "Service - Obtaining location...");
            var locationRequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
            var location = await Geolocation.GetLocationAsync(locationRequest);

            return $"Lat: {location.Latitude} Lng: {location.Longitude}";
        }

        public void SendAlert()
        {
            StartProgress().ConfigureAwait(false);
        }

        private async Task StartProgress(int secs = 10)
        {
            var notificationId = 65324;
            var manager = (NotificationManager)GetSystemService(NotificationService);

            alertCancellation = new CancellationTokenSource();

            int i = 0;

            while (i <= secs && !alertCancellation.Token.IsCancellationRequested)
            {
                var intent = new Intent(this, typeof(SecurityService));
                intent.SetAction("CANCEL_ALERT");
                var pendingIntent = PendingIntent.GetService(this, 0, intent, PendingIntentFlags.OneShot);
                var cancelAction = new NotificationCompat.Action(0, "Cancelar", pendingIntent);

                var notification = new NotificationCompat.Builder(this, ChannelId)
                    .SetSmallIcon(Resource.Drawable.shield1)
                    .AddAction(cancelAction)
                    .SetContentTitle("Enviando alerta")
                    .SetContentText("Te encuentras bien? Vamos a enviar una alerta.")
                    .SetProgress(secs, i, false)
                    .SetOngoing(true)
                    .Build();

                manager.Notify(notificationId, notification);

                i++;
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            if (!alertCancellation.Token.IsCancellationRequested)
            {
                // Enviar alerta
                Toast.MakeText(this, "Alerta enviada!", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "Alerta CANCELADA!", ToastLength.Long).Show();
            }

            manager.Cancel(notificationId);
        }
    }
}