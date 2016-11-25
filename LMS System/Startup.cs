using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LMS_System.Startup))]
namespace LMS_System
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
