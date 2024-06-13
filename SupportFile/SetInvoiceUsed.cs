using CinemaWeb.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CinemaWeb.SupportFile
{
    public class SetInvoiceUsed : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            using (var db = new Cinema_Web_Entities())
            {
                DateTime now = DateTime.Now;

                var invoiceExist = db.invoices.Where(i => i.is_used == false).ToList();

                foreach (var invoice in invoiceExist)
                {
                    var movieDisplayDate = invoice.room_schedule_detail.schedule_detail.movie_display_date.display_date.display_date1;
                    var endTime = invoice.room_schedule_detail.schedule_detail.end_time;

                    if (movieDisplayDate <= now && endTime < now.TimeOfDay)
                    {
                        invoice.is_used = true;
                    }
                }

                db.SaveChanges();
            }

            return Task.CompletedTask;
        }
    }
}