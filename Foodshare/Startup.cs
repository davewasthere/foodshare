using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Foodshare.Startup))]
namespace Foodshare
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
