using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using RemoteService.DependencyServices;
using RemoteService.Droid.Services;
using RemoteService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(RemoteService.Droid.DependencyServices.SecurityDependencyService))]
namespace RemoteService.Droid.DependencyServices
{
    public class SecurityDependencyService : ISecurityDependencyService
    {
        SecurityServiceConnection Connection;
        Messenger ClientMessenger;

        // taskCompletionSources
        TaskCompletionSource<string> LocationTcs;


        public void BindService()
        {
            MainActivity.Instance.ConnectToService();

            // Enable two-way communication with the service
            var handler = new ClientHandler(this);
            ClientMessenger = new Messenger(handler);

            // Get service current connection
            Connection = MainActivity.Instance.ServiceConnection;
        }

        public async Task<string> GetLocation()
        {
            if (Connection != null && Connection.IsConnected)
            {
                LocationTcs = new TaskCompletionSource<string>();

                var msg = Message.Obtain(null, Constants.LocationRequest);
                msg.ReplyTo = ClientMessenger;

                try
                {
                    Connection.Messenger.Send(msg);
                }
                catch (Exception ex)
                {
                    return $"Exception - {ex.Message}";
                }

                return await LocationTcs.Task;
            }
            return "Not connected to service";
            
        }

        public void OnLocation(string location)
        {
            LocationTcs.SetResult(location);
        }

        public void SendAlert()
        {
            if (Connection != null && Connection.IsConnected)
            {
                var msg = Message.Obtain(null, Constants.AlertRequest);

                try
                {
                    Connection.Messenger.Send(msg);
                }
                catch (Exception ex)
                {

                }
            }
        }

        public void StopService()
        {
            var intent = new Intent(Android.App.Application.Context, typeof(SecurityService));
            MainActivity.Instance.StopService(intent);
        }

        public void UnbindService()
        {
            MainActivity.Instance.DisconnectService();
            Connection = null;
        }

    }
}