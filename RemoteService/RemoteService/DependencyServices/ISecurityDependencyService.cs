using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteService.DependencyServices
{
    public interface ISecurityDependencyService
    {
        void BindService();
        void UnbindService();
        Task<string> GetLocation();
        void OnLocation(string location);
    }
}
