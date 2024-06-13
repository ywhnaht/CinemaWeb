using CinemaWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CinemaWeb.Areas.Staff.Controllers
{
    public class StaffHomeController : Controller
    {
        // GET: Staff/StaffHome
        Cinema_Web_Entities db = new Cinema_Web_Entities();
        public ActionResult Index()
        {   
            return View();
        }

        [HttpGet]
        public ActionResult GetInvoice(string invoiceId)
        {
            int id = int.Parse(invoiceId);
            var invoiceItem = db.invoices.FirstOrDefault(x => x.id == id);

            if (invoiceItem == null)
            {
                return Json(new { success = false, message = "Không tìm thấy vé!" }, JsonRequestBehavior.AllowGet);
            }

            var displayDate = invoiceItem.room_schedule_detail.schedule_detail.movie_display_date.display_date.display_date1;
            var scheduleTime = invoiceItem.room_schedule_detail.schedule_detail.schedule.schedule_time;

            if (invoiceItem.is_used == true)
            {
                return Json(new { success = false, message = "Vé đã được sử dụng!" }, JsonRequestBehavior.AllowGet);
            }

            var invoiceData = new
            {
                InvoiceId = invoiceItem.id,
                UserId = invoiceItem.user_id,
                TotalMoney = invoiceItem.total_money,
                MovieImage = invoiceItem.room_schedule_detail.schedule_detail.movie_display_date.movy.url_image,
                MovieName = invoiceItem.room_schedule_detail.schedule_detail.movie_display_date.movy.title,
                DayOfWeek = displayDate.HasValue ? displayDate.Value.DayOfWeek.ToString() : "N/A",
                Date = displayDate.HasValue ? displayDate.Value.ToString("dd/MM/yyyy") : "N/A",
                Schedule = scheduleTime != null ? scheduleTime.Value.ToString(@"hh\:mm") : "N/A",
                RoomName = invoiceItem.room_schedule_detail.room.room_name,
                Seat = invoiceItem.tickets.Select(t => new
                {
                    Row = t.seat.seat_row, 
                    Column = t.seat.seat_column 
                }).ToList()
            };
            return Json(new { success = true, invoiceData }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CheckIn(string invoiceId)
        {
            int id = int.Parse(invoiceId);
            var invoiceItem = db.invoices.FirstOrDefault(x => x.id == id);
            if (invoiceItem == null)
            {
                return Json(new { success = false, message = "Không tìm thấy vé!" });
            }

            var currentDateTime = DateTime.Now;
            var displayDate = invoiceItem.room_schedule_detail.schedule_detail.movie_display_date.display_date.display_date1;
            var startTime = invoiceItem.room_schedule_detail.schedule_detail.schedule.schedule_time;
            var movieStartDateTime = displayDate.Value.Date + startTime.Value;
            var checkInAllowedTime = movieStartDateTime.AddMinutes(-5);

            if (currentDateTime < checkInAllowedTime)
            {
                return Json(new { success = false, message = "Hiện chưa tới giờ Check In!" });
            }

            if (invoiceItem.is_used == false || invoiceItem.is_used == null)
            {
                invoiceItem.is_used = true;
                db.SaveChanges();
                return Json(new {success = true, message = "Check In thành công!"});
            }
            return Json(new { success = false, message = "Vé đã được sử dụng!" });
        }

        [HttpPost]
        public ActionResult SearchInvoice(string invoiceId)
        {
            int id = int.Parse(invoiceId);
            var invoiceItem = db.invoices.FirstOrDefault(x => x.id == id);

            if (invoiceItem == null)
            {
                return Json(new { success = false, message = "Không tìm thấy vé!" });
            }

            var currentDateTime = DateTime.Now;
            var displayDate = invoiceItem.room_schedule_detail.schedule_detail.movie_display_date.display_date.display_date1;
            var startTime = invoiceItem.room_schedule_detail.schedule_detail.schedule.schedule_time;
            var movieStartDateTime = displayDate.Value.Date + startTime.Value;
            var checkInAllowedTime = movieStartDateTime.AddMinutes(-5);

            if (currentDateTime < checkInAllowedTime)
            {
                return Json(new { success = false, message = "Hiện chưa tới giờ Check In!" });
            }

            if (invoiceItem.is_used == false || invoiceItem.is_used == null)
            {
                invoiceItem.is_used = true;
                db.SaveChanges();
                return Json(new { success = true, message = "Check In thành công!" });
            }
            return Json(new { success = false, message = "Vé đã được sử dụng!" });
        }

        public ActionResult StaffProfile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateInfor(string name, string pass)
        {
            var currentUser = (user)Session["staff"];
            var updateUser = db.users.FirstOrDefault(x => x.id == currentUser.id);

            if (!string.IsNullOrEmpty(pass))
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(pass);

                if (updateUser != null)
                {
                    updateUser.hashed_pass = hashedPassword;
                }
            }

            if (!string.IsNullOrEmpty(name))
            {
                currentUser.full_name = name;
                if (updateUser != null)
                {
                    updateUser.full_name = name;
                }
            }
            db.SaveChanges();

            var redirectUrl = Url.Action("StaffProfile", "StaffHome", new { area = "Staff" });
            return Json(new { success = true, redirectUrl = redirectUrl, message = "Cập nhật thông tin thành công" });
        }

        public ActionResult SignOut()
        {
            Session.Remove("staff");
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home", new {area = ""});
        }
    }
}