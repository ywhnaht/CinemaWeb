using CinemaWeb.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CinemaWeb.SupportFile
{
    public class DeleteNotificationJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            using (var db = new Cinema_Web_Entities())
            {
                DateTime dateBefore = DateTime.Now.AddDays(-2);

                var notificationsToDelete = db.notifications
                    .Where(n => n.status == true && n.date_create < dateBefore)
                    .ToList();

                db.notifications.RemoveRange(notificationsToDelete);
                db.SaveChanges();
            }

            return Task.CompletedTask;
        }
    }
}