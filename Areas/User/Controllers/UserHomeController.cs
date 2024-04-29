using CinemaWeb.Controllers;
using CinemaWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;
using CinemaWeb.App_Start;

namespace CinemaWeb.Areas.User.Controllers
{
    public class UserHomeController : Controller
    {
        Cinema_Web_Entities db = new Cinema_Web_Entities();
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("SignIn", "Home", new { area = "" });
            }
            List<movy> movielist = db.movies.ToList();
            DateTime currentDate = DateTime.Now.Date;
            foreach (var movie in movielist)
            {
                if (movie.release_date <= currentDate && movie.end_date >= currentDate)
                {
                    movie.movie_status = true; // Đang chiếu
                }
                else
                {
                    movie.movie_status = false; // Sắp chiếu
                }
            }

            movielist = movielist.OrderByDescending(m => m.release_date).ToList();
            ViewBag.MovieList = movielist;
            return View();
        }

        public ActionResult UserProfile()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("SignIn", "Home", new { area = "" });
            }
            var currentUser = (user)Session["user"];
            List<movy> movielist = db.movies.ToList();
            DateTime currentDate = DateTime.Now.Date;

            foreach (var movie in movielist)
            {
                if (movie.release_date <= currentDate && movie.end_date >= currentDate)
                {
                    movie.movie_status = true; // Đang chiếu
                }
                else
                {
                    movie.movie_status = false; // Sắp chiếu
                }
            }
            movielist = movielist.OrderByDescending(m => m.release_date).ToList();
            ViewBag.MovieList = movielist;

            int totalSpent = 0;
            ViewBag.MovieList = movielist;
            var invoiceList = currentUser.invoices.OrderBy(x => x.day_create).ToList();
            ViewBag.invoiceList = invoiceList;
            foreach (var invoiceitem in invoiceList)
            {
                if (invoiceitem.invoice_status == true)
                {
                    if (invoiceitem.day_create.Value.Year == currentDate.Year)
                    {
                        totalSpent += (int)invoiceitem.total_money;
                    }
                    var ticketList = invoiceitem.tickets.ToList();
                    ViewBag.ticketList = ticketList;
                }
            }

            ViewBag.totalSpent = totalSpent;
            return View();
        }
        public ActionResult HistoryTicket()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("SignIn", "Home", new { area = "" });
            }
            var currentUser = (user)Session["user"];
            List<movy> movielist = db.movies.ToList();
            DateTime currentDate = DateTime.Now;
            foreach (var movie in movielist)
            {
                if (movie.release_date <= currentDate.Date && movie.end_date >= currentDate.Date)
                {
                    movie.movie_status = true; // Đang chiếu
                }
                else
                {
                    movie.movie_status = false; // Sắp chiếu
                }
            }
            movielist = movielist.OrderByDescending(m => m.release_date).ToList();
            int totalSpent = 0;
            ViewBag.MovieList = movielist;
            var invoiceList = currentUser.invoices.OrderBy(x => x.day_create).ToList();
            ViewBag.invoiceList = invoiceList;
            foreach (var invoiceitem in invoiceList)
            {
                if (invoiceitem.invoice_status == true) { 
                    if (invoiceitem.day_create.Value.Year == currentDate.Year)
                    {
                        totalSpent += (int)invoiceitem.total_money;
                    }
                    var ticketList = invoiceitem.tickets.ToList();
                    ViewBag.ticketList = ticketList;
                }
            }
            
            ViewBag.totalSpent = totalSpent;
            return View();
        }

        [HttpPost]
        [UserAuthorize(roleId = 13)]        
        public ActionResult UpdateInfor(string name, string pass)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("SignIn", "Home", new { area = "" });
            }
            var currentUser = (user)Session["user"];
            var updateUser = db.users.FirstOrDefault(x => x.id == currentUser.id);

            if (!string.IsNullOrEmpty(pass))
            {
                currentUser.user_password = pass;
                if (updateUser != null)
                {
                    updateUser.user_password = pass;
                }
            }

            // Kiểm tra nếu chỉ có name được nhập mới
            if (!string.IsNullOrEmpty(name))
            {
                currentUser.full_name = name;
                if (updateUser != null)
                {
                    updateUser.full_name = name;
                }
            }
            db.SaveChanges();

            // Trả về kết quả thành công
            var redirectUrl = Url.Action("UserProfile", "UserHome", new { area = "User" });
            return Json(new { success = true, redirectUrl = redirectUrl });
        }
    }
}