using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waf.MusicManager.Applications.Services
{
    internal class ApplicationBusyContext : IDisposable
    {
        public Action<ApplicationBusyContext> DisposeCallback { get; set; }
        
        public void Dispose()
        {
            DisposeCallback(this);
        }
    }
}
