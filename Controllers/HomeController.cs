using CinemaWeb.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BCrypt.Net;
using System.Data.Entity.Validation;
using System.Diagnostics;
using Microsoft.AspNet.SignalR;
using CinemaWeb.SupportFile;

namespace CinemaWeb.Controllers
{
    public class HomeController : Controller
    {
        Cinema_Web_Entities db = new Cinema_Web_Entities();
        IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        public void GetMovieStatus(List<movy> movielist)
        {
            DateTime currentDate = DateTime.Now;
            foreach (movy movie in movielist)
            {
                if (movie.release_date <= currentDate && movie.end_date >= currentDate)
                {
                    movie.movie_status = true; // Đang chiếu
                }
                else if (movie.release_date > currentDate)
                {
                    movie.movie_status = false; // Sắp chiếu
                }
                else
                {
                    movie.movie_status = null;
                }
            }
        }
        public ActionResult Index()
        {
            List<movy> movielist = db.movies.ToList();
            GetMovieStatus(movielist);
            db.SaveChanges();
            movielist = movielist.OrderByDescending(m => m.release_date).ToList();
            ViewBag.MovieList = movielist;
            return View();
        }

        [AllowAnonymous]
        public ActionResult SignIn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult SignIn(string email, string pass, string returnUrl)
        {
            var user = db.users.FirstOrDefault(x => x.email == email);
            if (user != null)
            {
               if (BCrypt.Net.BCrypt.Verify(pass, user.hashed_pass))
                {
                    if (user.user_type == 1)
                    {
                        Session["user"] = user;
                        return RedirectToLocal(returnUrl, "Index", "UserHome", "User");
                    }
                    else if (user.user_type == 2)
                    {
                        Session["admin"] = user;
                        return RedirectToLocal(returnUrl, "Index", "AdminHome", "Admin");
                    }
                    else 
                    {
                        Session["staff"] = user;
                        return RedirectToLocal(returnUrl, "Index", "StaffHome", "Staff");
                    }
                }
               else
                {
                    ViewBag.ReturnUrl = returnUrl;
                    return Json(new { success = false, message = "Mật khẩu không đúng!" });
                }
            }
            else
            {
                ViewBag.ReturnUrl = returnUrl;
                return Json(new { success = false, message = "Email không tồn tại!" });
            }
        }

        private ActionResult RedirectToLocal(string returnUrl, string action, string controller, string area)
        {
            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return Json(new { success = true, url = Url.Action(action, controller, new { area }) });
        }


        //HTTP GET
        public ActionResult SignUp(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        //HTTP POST
        [HttpPost]
        public ActionResult SignUp(string name, string email, DateTime dateofbirth, string pass, string returnUrl, string verifyCode)
        {
            if (Session["verifycode"].ToString() == verifyCode)
            {
                user newUser = new user();
                if (newUser.user_type == null) newUser.user_type = 1;
                newUser.full_name = name;
                newUser.email = email;
                newUser.date_of_birth = dateofbirth;
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(pass);
                newUser.hashed_pass = hashedPassword;
                newUser.created = DateTime.Now;
                db.users.Add(newUser);
                db.SaveChanges();

                DateTime startDate = new DateTime(DateTime.Now.Year, dateofbirth.Month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                var userDiscount = new user_discount
                {
                    discount_id = 1,
                    discount_status = false,
                    user_id = newUser.id,
                    start_date = startDate,
                    end_date = endDate
                };

                var adminList = db.users.Where(x => x.user_type == 2).ToList();
                var adminNotice = new notification();
                foreach (var admin in adminList)
                {
                    adminNotice.user_id = admin.id;
                    adminNotice.content = "Khách hàng mới";
                    adminNotice.sub_content = "Người dùng " + newUser.full_name + " đã đăng ký tài khoản thành công!";
                    adminNotice.date_create = DateTime.Now;
                    adminNotice.status = false;
                    db.notifications.Add(adminNotice);
                    db.SaveChanges();
                }

                _hubContext.Clients.All.broadcastNotification(adminNotice.content, adminNotice.sub_content);

                Session["user"] = newUser;
                return RedirectToLocal(returnUrl, "Index", "UserHome", "User");
            }
            else
            {
                return Json(new { success = false, message = "Mã xác thực không chính xác!" });
            }
        }
        [HttpPost]
        public ActionResult CheckExistAccount(string name, string email, string pass, string confirmpass, string returnUrl)
        {
            var _user = db.users.FirstOrDefault(x => x.email == email);
            if (db.users.Any(x => x.email == email))
            {
                ViewBag.ReturnUrl = returnUrl;
                return Json(new { success = false, message = "Email đã tồn tại!" });
            }
            else
            {
                if (pass != confirmpass)
                {
                    ViewBag.ReturnUrl = returnUrl;
                    return Json(new { success = false, message = "Mật khẩu không khớp!" });
                }
                else 
                    return Json(new { success = true, message = "" });
            }
        }
        public ActionResult SignOut()
        {
            Session.Remove("user");
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult CheckLoginStatus()
        {
            var currentUser = Session["user"];
            bool loggedIn = currentUser != null;
            return Json(new { loggedIn = loggedIn }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ForgetPassword(string registerEmail)
        {
            var user = db.users.FirstOrDefault(x => x.email == registerEmail);
            if (user != null)
            {
                string newPass = GenerateRandomPassword(8);
                string hassedPass = BCrypt.Net.BCrypt.HashPassword(newPass);
                user.hashed_pass = hassedPass;
                db.SaveChanges();

                string content = "<p>Xin chào " + user.full_name + "</p> <br><br>";
                content += "<span>Theo yêu cầu của bạn, Ohayou Cinema xin gửi lại bạn thông tin mật mã tài khoản</span> <br><br>";
                content += "<p>Password: " + newPass + "</p>";

                CinemaWeb.Areas.User.Common.Common.SendMail("Ohayou Cinema", "Reset Password", content, registerEmail);
                return Json(new { success = true, message = "Gửi mail thành công" });
            }
            else
            {
                return Json(new { success = false, message = "Email không tồn tại" });
            }
        }
        [HttpPost]
        public ActionResult VerifyEmail(string email, string name)
        {
            string verifyCode = GenerateRandomPassword(8);
            Session["verifycode"] = verifyCode;
            string content = "<p>Xin chào " + name + "</p> <br><br>";
            content += "<span>Cảm ơn bạn đã đăng ký tài khoản, Ohayou Cinema xin gửi bạn mã xác thực</span> <br><br>";
            content += "<p>Verify Code: " + verifyCode + "</p>";

            CinemaWeb.Areas.User.Common.Common.SendMail("Ohayou Cinema", "Verify Email", content, email);
            return Json(new { success = true, message = "Gửi mail thành công" });
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