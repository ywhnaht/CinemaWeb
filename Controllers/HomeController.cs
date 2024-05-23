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

namespace CinemaWeb.Controllers
{
    public class HomeController : Controller
    {
        Cinema_Web_Entities db = new Cinema_Web_Entities();
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
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
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
                    ViewBag.ErrorPass = "Mật khẩu không đúng!";
                    ViewBag.Email = email;
                    ViewBag.ReturnUrl = returnUrl;
                    return View("SignIn");
                }
            }
            else
            {
                ViewBag.ErrorEmail = "Email không tồn tại!";
                ViewBag.ReturnUrl = returnUrl;
                return View("SignIn");
            }
        }

        private ActionResult RedirectToLocal(string returnUrl, string action, string controller, string area)
        {
            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(action, controller, new { area });
        }


        //HTTP GET
        public ActionResult SignUp(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        //HTTP POST
        [HttpPost]
        public ActionResult SignUp(string name, string email, DateTime dateofbirth, string pass, string confirmpass, string returnUrl)
        {
            user newUser = new user();
            var _user = db.users.FirstOrDefault(x => x.email == email);
            if (db.users.Any(x => x.email == email))
            {
                ViewBag.ErrorEmailExist = "Email đã tồn tại!";
                ViewBag.ReturnUrl = returnUrl;
                return View("SignUp");
            }
            else {
                if (pass != confirmpass)
                {
                    ViewBag.ErrorPassNotsame = "Mật khẩu không khớp!";
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.Name = name; // Truyền lại name và các thông tin khác để hiển thị lại trong modal
                    ViewBag.Email = email;
                    ViewBag.DateOfBirth = dateofbirth;
                    return View("SignUp");
                }
                else
                {
                    if (newUser.user_type == null) newUser.user_type = 1;
                    newUser.full_name = name;
                    newUser.email = email;
                    newUser.date_of_birth = dateofbirth;
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(pass);
                    newUser.hashed_pass = hashedPassword;
                    db.users.Add(newUser);
                    db.SaveChanges();
                    Session["user"] = newUser;
                    return RedirectToLocal(returnUrl, "Index", "UserHome", "User");
                }
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

    }
}