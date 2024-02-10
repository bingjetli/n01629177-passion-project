using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using n01629177_passion_project.Models;
using Owin;

[assembly: OwinStartupAttribute(typeof(n01629177_passion_project.Startup))]
namespace n01629177_passion_project {
  public partial class Startup {
    public void Configuration(IAppBuilder app) {
      ConfigureAuth(app);
    }

    public void ConfigureServices(IServiceCollection services) {
      //services.AddScoped<UserManager<ApplicationUser>>();
    }

  }
}
