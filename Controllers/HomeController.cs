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

namespace CinemaWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
                return View();
        }

        [AllowAnonymous]
        public ActionResult SignIn()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(string email, string pass)
        {
            Cinema_Web_Entities db = new Cinema_Web_Entities();
            var user = db.users.FirstOrDefault(x => x.email == email);
            if (user != null)
            {
               if (user.user_password == pass)
                {
                    Session["user"] = user;
                    return RedirectToAction("Index", "UserHome", new { area = "User" });
                }
               else
                {
                    ViewBag.ErrorPass = "Mật khẩu không đúng!";
                    ViewBag.Email = email;
                    return View("SignIn");
                }
            }
            else
            {
                ViewBag.ErrorEmail = "Email không tồn tại!";
                return View("SignIn");
            }

        }
       
        //HTTP GET
        public ActionResult SignUp()
        {
            return View();
        }
        //HTTP POST
        [HttpPost]
        public ActionResult SignUp(string name, string email, DateTime dateofbirth, string pass, string confirmpass)
        {
            Cinema_Web_Entities db = new Cinema_Web_Entities();
            user newUser = new user();
            var _user = db.users.FirstOrDefault(x => x.email == email);
            if (_user != null)
            {
                ViewBag.ErrorEmailExist = "Email đã tồn tại!";
                return View("SignUp");
            }
            else {
                if (pass != confirmpass)
                {
                    ViewBag.ErrorPassNotsame = "Mật khẩu không khớp!";
                    ViewBag.Name = name; // Truyền lại name và các thông tin khác để hiển thị lại trong modal
                    ViewBag.Email = email;
                    ViewBag.DateOfBirth = dateofbirth;
                    return View("SignUp");
                }
                else
                {
                    newUser.full_name = name;
                    newUser.email = email;
                    newUser.date_of_birth = dateofbirth;
                    newUser.user_password = pass;
                    db.users.Add(newUser);
                    db.SaveChanges();
                    Session["user"] = newUser;
                    return RedirectToAction("Index", "UserHome", new { area = "User" });
                }
            }       
        }
        public ActionResult SignOut()
        {
            Session.Remove("user");
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}