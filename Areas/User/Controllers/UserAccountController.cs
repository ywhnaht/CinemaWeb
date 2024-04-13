using CinemaWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaWeb.Areas.User.Controllers
{
    public class UserAccountController : Controller
    {
        // GET: User/UserAccount
        private readonly Cinema_Web_Entities db;
        public UserAccountController()
        {
            db = new Cinema_Web_Entities();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserProfile() 
        {
            //var result = from user in db.users
            //             join invoice in db.invoices on user.id equals invoice.user_id
            //             select new UserProfileViewModel
            //             {
            //                 UserName = user.full_name,
            //                 Email = user.email,
            //                 DateOfBirth = (DateTime)user.date_of_birth,
            //                 PhoneNum = user.user_phone,
            //                 Password = user.user_password,
            //                 TotalMoneySpent = (int)invoice.total_money
            //             };
            //ViewBag.result = result;
            return View();
        }
    }
}