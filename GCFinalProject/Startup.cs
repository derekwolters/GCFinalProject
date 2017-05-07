using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GCFinalProject.Startup))]
namespace GCFinalProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
