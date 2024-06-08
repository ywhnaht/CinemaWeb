using CinemaWeb.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaWeb.Areas.Admin.Controllers
{
    public class StaffController : Controller
    {
        Cinema_Web_Entities db = new Cinema_Web_Entities();
        // GET: Admin/Staff
        public ActionResult StaffList()
        {
            var staffList = db.users.Where(x => x.user_type == 3).ToList();
            foreach (var item in staffList)
            {
            }
            ViewBag.StaffList = staffList;
            return View();
        }

        [HttpPost]
        public ActionResult StaffDetail(int staffId)
        {
            var staffItemQuery = db.users.Where(x => x.id == staffId && x.user_type == 3)
                                        .Select(x => new
                                        {
                                            x.id,
                                            x.user_type,
                                            x.full_name,
                                            x.email,
                                            x.date_of_birth
                                        })
                                        .FirstOrDefault();

            if (staffItemQuery == null)
            {
                return Json(new { success = false, message = "Không tìm thấy nhân viên tương ứng" });
            }
            else
            {
                var staffItem = new
                {
                    staffItemQuery.id,
                    staffItemQuery.user_type,
                    staffItemQuery.full_name,
                    staffItemQuery.email,
                    StaffDate = staffItemQuery.date_of_birth.HasValue ? staffItemQuery.date_of_birth.Value.ToString("yyyy-MM-dd") : null
                };
                return Json(new { success = true, staffItem });
            }
        }

        [HttpPost]
        public ActionResult UpdateStaffInfor(int staffId, string name, string email, DateTime dateofbirth, string pass)
        {
            var checkEmail = db.users.FirstOrDefault(x => x.email == email && x.id != staffId);
            if (checkEmail != null)
                return Json(new { success = false, message = "Email đã tồn tại" });
            
            var updateStaff = db.users.FirstOrDefault(x => x.id == staffId && x.user_type == 3);
            if (!string.IsNullOrEmpty(pass))
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(pass);

                if (updateStaff != null)
                {
                    updateStaff.hashed_pass = hashedPassword;
                }
            }

            if (!string.IsNullOrEmpty(name))
            {
                updateStaff.full_name = name;
                if (updateStaff != null)
                {
                    updateStaff.full_name = name;
                }
            }
            updateStaff.date_of_birth = dateofbirth;
            updateStaff.email = email;

            db.SaveChanges();
            var redirectUrl = Url.Action("StaffList", "Staff", new { area = "Admin" });
            return Json(new { success = true, redirectUrl = redirectUrl });
        }
    }
}