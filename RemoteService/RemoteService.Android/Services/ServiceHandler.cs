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
    public class ServiceHandler: Handler
    {
        SecurityService service;

        public ServiceHandler(SecurityService service)
        {
            this.service = service;
        }

        public override void HandleMessage(Message msg)
        {
            int code = msg.What;
            Log.Debug(Constants.TAG, $"ServiceHandler - Handling message {code}");

            switch (code)
            {
                case Constants.LocationRequest:
                    GetLocation(code, msg.ReplyTo).ConfigureAwait(false);
                    break;
            }
        }

        private async Task GetLocation(int code, Messenger clientMessenger)
        {
            // Get location from service
            var location = await service.GetLocation();

            var data = new Bundle();
            data.PutString(code.ToString(), location);

            ReplyMessage(code, clientMessenger, data);
        }

        private void ReplyMessage(int code, Messenger clientMessenger, Bundle data)
        {
            var msg = Message.Obtain(null, code);
            msg.Data = data;

            try
            {
                clientMessenger.Send(msg);
            }
            catch (Exception ex)
            {
                Log.Error(Constants.TAG, ex.ToString());
            }
        }

    }
}