using Microsoft.Owin;
using Owin;


[assembly: OwinStartupAttribute(typeof(WebSiteProject.Startup))]
namespace WebSiteProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
