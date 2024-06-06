using CinemaWeb.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CinemaWeb.SupportFile
{
    public class RemoveSeatStatusJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            using (var dbContext = new Cinema_Web_Entities())
            {
                var currentDate = DateTime.Now.Date;

                var expiredSeats = from ss in dbContext.seat_status
                                   join rsd in dbContext.room_schedule_detail on ss.room_schedule_detail_id equals rsd.id
                                   join sd in dbContext.schedule_detail on rsd.schedule_detail_id equals sd.id
                                   join mdd in dbContext.movie_display_date on sd.movie_display_date_id equals mdd.id
                                   join dd in dbContext.display_date on mdd.display_date_id equals dd.id
                                   where dd.display_date1 < currentDate
                                   select ss;

                dbContext.seat_status.RemoveRange(expiredSeats);
                dbContext.SaveChanges();
            }
            return Task.CompletedTask;
        }
    }
}