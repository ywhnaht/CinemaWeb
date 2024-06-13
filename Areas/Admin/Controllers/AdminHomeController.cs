using CinemaWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaWeb.Areas.Admin.Controllers
{
    public class AdminHomeController : Controller
    {
        Cinema_Web_Entities db = new Cinema_Web_Entities();
        // GET: Admin/AdminHome
        public ActionResult Index()
        {
            var today = DateTime.Now.Date;
            var totalTicketsToday = db.tickets
                                  .Join(db.invoices, t => t.invoice_id, i => i.id, (t, i) => new { Ticket = t, Invoice = i })
                                  .Where(ti => DbFunctions.TruncateTime(ti.Invoice.day_create) == today && ti.Invoice.invoice_status == true)
                                  .Count();

            var totalRevenueToday = db.invoices
                                  .Where(i => DbFunctions.TruncateTime(i.day_create) == today && i.invoice_status == true)
                                  .Sum(i => (int?)i.total_money) ?? 0;

            var yesterday = DateTime.Now.AddDays(-1).Date;
            var totalRevenueYesterday = db.invoices
                                          .Where(i => DbFunctions.TruncateTime(i.day_create) == yesterday && i.invoice_status == true)
                                          .Sum(i => (int?)i.total_money) ?? 0;

            var revenueIncreasePercentage = (totalRevenueYesterday == 0) ? 100 :
                                            ((totalRevenueToday - totalRevenueYesterday) / (double)totalRevenueYesterday) * 100;

            var totalUsersToday = db.users
                                    .Where(u => DbFunctions.TruncateTime(u.created) == today && u.user_type == 1)
                                    .Count();

            var notice = db.notifications.Where(x => x.user.user_type == 2 && x.status == false).OrderByDescending(x => x.date_create).ToList();

            ViewBag.Notifications = notice;
            ViewBag.TotalUsers = totalUsersToday;
            ViewBag.TotalTicketsToday = totalTicketsToday;
            ViewBag.TotalRevenueToday = totalRevenueToday;
            ViewBag.RevenueIncreasePercentage = revenueIncreasePercentage;

            return View();
        }

        public ActionResult InvoiceList()
        {
            return View();
        }
    }
}