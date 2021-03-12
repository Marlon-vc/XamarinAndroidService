using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteService.Droid.Services
{
    public class SecurityServiceBinder: Binder
    {
        SecurityService service;

        public SecurityServiceBinder(SecurityService service)
        {
            this.service = service;
        }

        // Implement methods
        public async Task<string> GetLocation()
        {
            return await service.GetLocation();
        }
    }
}