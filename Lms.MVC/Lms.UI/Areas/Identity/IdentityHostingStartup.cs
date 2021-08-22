using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Lms.MVC.UI.Areas.Identity.IdentityHostingStartup))]

namespace Lms.MVC.UI.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}