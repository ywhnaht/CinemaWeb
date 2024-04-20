using CinemaWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaWeb.Areas.User.Controllers
{
    public class BookTicketController : Controller
    {
        Cinema_Web_Entities db = new Cinema_Web_Entities();
        // GET: User/BookTicket
        public ActionResult BookTicket()
        {
            List<movy> movielist = db.movies.ToList();
            DateTime currentDate = DateTime.Now.Date;
            ViewBag.currentDate = currentDate;
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
        [HttpGet]
        public ActionResult GetDisplayDate(int movieId)
        {
            var movieDateList = db.movie_display_date
                        .Where(x => x.movie_id == movieId)
                        .Select(x => new {
                            x.display_date.id,
                            x.display_date.display_date1
                        })
                        .ToList();
           
            return Json(movieDateList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetScheduleTime(int displaydateId)
        {
            var scheduleList = db.schedule_detail
                        .Where(x => x.display_date_id == displaydateId)
                        .Select(x => new
                        {
                            x.schedule.id,
                            schedule_time = DbFunctions.CreateDateTime(1, 1, 1, x.schedule.schedule_time.Value.Hours, x.schedule.schedule_time.Value.Minutes, x.schedule.schedule_time.Value.Seconds) // Chuyển đổi TimeSpan sang DateTime
                        })
                        .ToList();
            return Json(scheduleList, JsonRequestBehavior.AllowGet);
        }
    }
}