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

namespace RemoteService.Droid.DependencyServices
{
    public class ClientHandler: Handler
    {
        SecurityDependencyService service;

        public ClientHandler(SecurityDependencyService service)
        {
            this.service = service;
        }

        public override void HandleMessage(Message msg)
        {
            int code = msg.What;
            Log.Debug(Constants.TAG, $"ClientHandler - Handling message {code}");

            switch (code)
            {
                case Constants.LocationRequest:
                    var location = msg.Data?.GetString(code.ToString());
                    service.OnLocation(location);
                    break;
            }
        }
    }
}