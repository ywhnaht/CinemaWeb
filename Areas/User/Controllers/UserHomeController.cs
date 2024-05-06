﻿using CinemaWeb.Controllers;
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
        DateTime currentDate = DateTime.Now;
        public void GetMovieStatus(List<movy> movielist)
        {
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
        public enum RoleName
        {
            AddMovie = 1,
            RemoveMovie = 2,
            EditMovie = 3,
            Statistical = 4,
            AddRoom = 5,
            RemoveRoom = 6,
            AddSchedule = 7,
            RemoveSchedule = 8,
            AddStaff = 9,
            RemoveStaff = 10,
            RemoveUser = 11,
            BookTicket = 12,
            EditProfile = 13,
            Payment = 14,
            CheckIn = 15
        }
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("SignIn", "Home", new { area = "" });
            }
            List<movy> movielist = db.movies.ToList();
            GetMovieStatus(movielist);

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
            GetMovieStatus(movielist);

            movielist = movielist.OrderByDescending(m => m.release_date).ToList();
            ViewBag.MovieList = movielist;

            int totalSpent = 0;
            ViewBag.MovieList = movielist;
            var invoiceList = db.invoices.Where(x => x.user_id == currentUser.id).OrderByDescending(x => x.room_schedule_detail.schedule_detail.movie_display_date.display_date.display_date1).ToList();
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
            GetMovieStatus(movielist);
            movielist = movielist.OrderByDescending(m => m.release_date).ToList();
            int totalSpent = 0;
            ViewBag.MovieList = movielist;
            var invoiceList = db.invoices.Where(x => x.user_id == currentUser.id).OrderByDescending(x => x.room_schedule_detail.schedule_detail.movie_display_date.display_date.display_date1).ToList();
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

            //ViewBag.QrCodeImg = Session["QrCodeImg"] as string; // hoặc Session["QRCodeImg"] as string;
            //ViewBag.InvoiceId = Session["InvoiceId"];
            ViewBag.totalSpent = totalSpent;
            return View();
        }

        [HttpPost]
        [UserAuthorize(roleId = (int)RoleName.EditProfile)]        
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