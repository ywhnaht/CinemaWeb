using CinemaWeb.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CinemaWeb.SupportFile
{
    public class ChangeSeatStatusJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            using (var db = new Cinema_Web_Entities())
            {
                DateTime now = DateTime.Now;

                var expiredSeats = db.seat_status
                    .Where(s => s.is_booked == true && s.hold_until != null && s.hold_until < now)
                    .ToList();

                foreach (var seat in expiredSeats)
                {
                    seat.is_booked = false;
                    seat.hold_until = null;
                }

                db.SaveChanges();
            }

            return Task.CompletedTask;
        }
    }
}