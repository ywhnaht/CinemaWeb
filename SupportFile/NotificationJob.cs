using CinemaWeb.Models;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;

public class NotificationJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        using (var db = new Cinema_Web_Entities())
        {
            DateTime currentDate = DateTime.Now;
            var invoices = db.invoices
                             .Where(x => x.invoice_status == true)
                             .ToList();

            foreach (var invoiceItem in invoices)
            {
                if (invoiceItem.room_schedule_detail.schedule_detail.movie_display_date.display_date.display_date1 == currentDate.Date &&
                (invoiceItem.room_schedule_detail.schedule_detail.start_time - currentDate.TimeOfDay) < TimeSpan.FromHours(3) &&
                (invoiceItem.room_schedule_detail.schedule_detail.start_time - currentDate.TimeOfDay) > TimeSpan.FromHours(0))
                {
                    var notice = new notification
                    {
                        content = "Bạn ơi, 3 tiếng nữa suất chiếu bắt đầu đó!",
                        sub_content = invoiceItem.room_schedule_detail.schedule_detail.movie_display_date.movy.title + " sẽ bắt đầu lúc " +
                                                   invoiceItem.room_schedule_detail.schedule_detail.schedule.schedule_time.Value.ToString(@"hh\:mm") + ", " +
                                                   invoiceItem.room_schedule_detail.schedule_detail.movie_display_date.display_date.display_date1.Value.ToString("dd/MM/yyyy"),
                        user_id = invoiceItem.user_id,
                        date_create = currentDate,
                        status = false
                    };
                    var existingNotification = db.notifications.FirstOrDefault(n =>
                        n.user_id == notice.user_id &&
                        n.content == notice.content &&
                        n.sub_content == notice.sub_content);

                    if (existingNotification == null)
                    {
                        db.notifications.Add(notice);
                    }
                }
                db.SaveChanges();
            }
        }

        return Task.CompletedTask;
    }
}
