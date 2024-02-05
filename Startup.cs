using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(n01629177_passion_project.Startup))]
namespace n01629177_passion_project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
