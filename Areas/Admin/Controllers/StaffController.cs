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

        [HttpPost]
        public ActionResult DeleteStaff(int staffId)
        {
            var staffItem = db.users.FirstOrDefault(x => x.id == staffId && x.user_type == 3);
            if (staffItem ==  null)
            {
                return Json(new { success = false, message = "Không tìm thấy nhân viên" });
            }

            db.users.Remove(staffItem);
            db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult AddStaff(string name, string email, DateTime dateofbirth)
        {
            var newStaff = new user();
            var checkEmail = db.users.FirstOrDefault(x => x.email == email);
            if (checkEmail != null)
                return Json(new { success = false, message = "Email đã tồn tại" });
            newStaff.email = email;
            newStaff.full_name = name;
            newStaff.date_of_birth = dateofbirth;
            newStaff.user_type = 3;
            string newPass = GenerateRandomPassword(8);
            string hassedPass = BCrypt.Net.BCrypt.HashPassword(newPass);
            newStaff.hashed_pass = hassedPass;
            db.users.Add(newStaff);
            db.SaveChanges();

            string content = "<p>Xin chào " + newStaff.full_name + "</p> <br><br>";
            content += "<span>Chúc mừng bạn đã trở thành nhân viên chính thức của Ohayou Cinema, chúng tôi xin gửi tài khoản đăng nhập dành cho bạn</span> <br><br>";
            content += "<p>Email: " + email + "</p> <br>";
            content += "<p>Mật khẩu: " + newPass + "</p>";

            CinemaWeb.Areas.User.Common.Common.SendMail("Ohayou Cinema", "Mật khẩu tài khoản nhân viên", content, email);
            return Json(new { success = true, message = "Thêm nhân viên thành công" });
        }

        [HttpGet]
        public ActionResult SearchStaff(string query)
        {
            var staffLists = db.users
                .Where(x => (x.full_name.Contains(query) || x.email.Contains(query)) && x.user_type == 3)
                .Select(x => new {
                    id = x.id,
                    full_name = x.full_name,
                    date_of_birth = x.date_of_birth,
                    email = x.email
                })
                .ToList();

            if (staffLists.Count == 0)
            {
                return Json(new { success = false, message = "Không tìm thấy nhân viên" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var staffList = staffLists.GroupBy(x => new { x.id, x.full_name, x.date_of_birth, x.email })
                                          .Select(g => new
                                          {
                                              g.Key.id,
                                              g.Key.full_name,
                                              staffDate = g.Key.date_of_birth.Value.ToString("yyyy-MM-dd"),
                                              g.Key.email
                                          }).ToList();
                return Json(new { success = true, staffList }, JsonRequestBehavior.AllowGet);
            }
            
        }
        public string GenerateRandomPassword(int length)
        {
            string allowedLetterChars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
            string allowedNumberChars = "0123456789";
            char[] chars = new char[length];
            Random rd = new Random();
            bool useLetter = true;
            for (int i = 0; i < length; i++)
            {
                if (useLetter)
                {
                    chars[i] = allowedLetterChars[rd.Next(0, allowedLetterChars.Length)];
                    useLetter = false;
                }
                else
                {
                    chars[i] = allowedNumberChars[rd.Next(0, allowedNumberChars.Length)];
                    useLetter = true;
                }
            }
            return new string(chars);
        }
    }
}