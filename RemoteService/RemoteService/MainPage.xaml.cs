using Acr.UserDialogs;
using RemoteService.DependencyServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RemoteService
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void BindService(object sender, EventArgs e)
        {
            var service = DependencyService.Get<ISecurityDependencyService>();
            service.BindService();
        }

        private void RequestLocation(object sender, EventArgs e)
        {
            var service = DependencyService.Get<ISecurityDependencyService>();
            GetLocation(service).ConfigureAwait(true);
        }

        private async Task GetLocation(ISecurityDependencyService service)
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

            if (status != PermissionStatus.Granted)
            {
                await UserDialogs.Instance.AlertAsync(new AlertConfig()
                {
                    Title = "Location Permission",
                    Message = "We need access to your device location.",
                    OkText = "Ok"
                });

                var newStatus = await Permissions.RequestAsync<Permissions.LocationAlways>();

                if (newStatus != PermissionStatus.Granted)
                {
                    UserDialogs.Instance.Toast("Location not available", TimeSpan.FromSeconds(3));
                    return;
                }
            }

            var location = await service.GetLocation();

            UserDialogs.Instance.Toast(location, TimeSpan.FromSeconds(3));
        }

        private void UnbindService(object sender, EventArgs e)
        {
            var service = DependencyService.Get<ISecurityDependencyService>();
            service.UnbindService();
        }

        private void StopService(object sender, EventArgs e)
        {
            var service = DependencyService.Get<ISecurityDependencyService>();
            service.StopService();
        }

        private void SendAlert(object sender, EventArgs e)
        {
            var service = DependencyService.Get<ISecurityDependencyService>();
            service.SendAlert();
        }
    }
}
