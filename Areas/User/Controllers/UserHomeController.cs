using CinemaWeb.Controllers;
using CinemaWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;

namespace CinemaWeb.Areas.User.Controllers
{
    public class UserHomeController : Controller
    {
        Cinema_Web_Entities db = new Cinema_Web_Entities();
        public ActionResult Index()
        {
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
            ViewBag.MovieList = movielist;
            return View();
        }

        public ActionResult UserProfile()
        {
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
            ViewBag.MovieList = movielist;
            return View();
        }
        public ActionResult HistoryTicket()
        {
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
            ViewBag.MovieList = movielist;
            return View();
        }
        
        public ActionResult UpdateInfor(string email, string pass)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            var currentUser = (user)Session["user"];
            var existingUser = db.users.FirstOrDefault(x => x.email == email);
            if (existingUser != null)
            {
                // Nếu email đã tồn tại, trả về thông báo lỗi
                return Json(new { success = false, message = "Email đã tồn tại!" });
            }
            // Cập nhật thông tin người dùng
            
            currentUser.email = email;
            currentUser.user_password = pass;

            var updateUser = db.users.FirstOrDefault(x => x.id == currentUser.id);
            updateUser.email = email;
            updateUser.user_password = pass;

            // Lưu thông tin mới vào cơ sở dữ liệu
            db.SaveChanges();

            // Trả về kết quả thành công
            var redirectUrl = Url.Action("UserProfile", "UserHome", new { area = "User" });
            return Json(new { success = true, redirectUrl = redirectUrl });
        }
    }
}