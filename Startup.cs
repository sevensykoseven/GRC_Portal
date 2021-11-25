using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(protean.Startup))]
namespace protean
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
