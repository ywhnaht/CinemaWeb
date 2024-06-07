using CinemaWeb.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CinemaWeb.SupportFile
{
    public class RemoveInvoiceJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            using (var db = new Cinema_Web_Entities())
            {
                DateTime threshold = DateTime.Now.AddMinutes(-15);
                var unpaidInvoices = db.invoices.Where(i => i.invoice_status != true && i.day_create < threshold).ToList();

                foreach (var invoice in unpaidInvoices)
                {
                    var tickets = db.tickets.Where(t => t.invoice_id == invoice.id).ToList();
                    List<seat_status> chosenSeat = new List<seat_status>();

                    foreach (var ticket in tickets)
                    {
                        chosenSeat.AddRange(ticket.seat.seat_status.Where(x => x.room_schedule_detail_id == invoice.room_schedule_detail_id && x.is_booked == true && x.seat_id == ticket.seat_id));
                        db.tickets.Remove(ticket);
                        db.SaveChanges();
                    }

                    foreach (var seat in chosenSeat)
                    {
                        seat.is_booked = false;
                        seat.hold_until = null;
                    }
                    db.invoices.Remove(invoice);
                }

                db.SaveChanges();
            }

            return Task.CompletedTask;
        }
    }
}