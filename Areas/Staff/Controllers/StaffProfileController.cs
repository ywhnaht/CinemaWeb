using CinemaWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaWeb.Areas.Staff.Controllers
{
    public class StaffProfileController : Controller
    {
        Cinema_Web_Entities db = new Cinema_Web_Entities();
        // GET: Staff/StaffProfile
        public ActionResult StaffProfile()
        {
            var currentUser = System.Web.HttpContext.Current.Session["staff"] as user;


            // Truyền thông tin qua ViewBag
            ViewBag.CurrentUser = currentUser;
            ViewBag.Name = currentUser.full_name;

            // Hoặc truyền qua Model
            return View(currentUser);
        }

        [HttpPost]
        public ActionResult UpdateInfo(string name, string pass)
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

            //var redirectUrl = Url.Action("UserProfile", "UserHome", new { area = "User" });
            return Json(new { success = true, message = "Cập nhật thông tin thành công" });
        }
    }
}