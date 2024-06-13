using CinemaWeb.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Web.Services.Description;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System.Threading.Tasks;

[assembly: OwinStartupAttribute(typeof(CinemaWeb.Startup))]
namespace CinemaWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Task.Run(() => QuartzConfig.ConfigureQuartz()).Wait();
            app.MapSignalR();
        }
    }
}
