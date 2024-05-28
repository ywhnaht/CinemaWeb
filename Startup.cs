using CinemaWeb.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Web.Services.Description;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

[assembly: OwinStartupAttribute(typeof(CinemaWeb.Startup))]
namespace CinemaWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            scheduler.Start().Wait();

            // Định nghĩa job và trigger
            IJobDetail job = JobBuilder.Create<NotificationJob>()
                .WithIdentity("notificationJob", "group1")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("notificationTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(1)  // Lập lịch mỗi giờ
                    .RepeatForever())
                .Build();

            // Lên lịch job với trigger
            scheduler.ScheduleJob(job, trigger).Wait();
        }
        
    }
}
