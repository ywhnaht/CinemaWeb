using CinemaWeb.Models;
using System;
using System.Collections.Generic;
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
            List<display_date> displaydate = db.display_date.ToList();
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
                foreach (var moviedisplaydate in movie.movie_display_date)
                {
                    
                }
                
            }
            var movieItem = db.movies.Select(movie => new
            {
                displayDates = movie.movie_display_date.Select(md => new
                {
                    displayDate = md.display_date.display_date1,
                    scheduleTimes = md.display_date.schedule_detail.Select(sd => new
                    {
                        scheduleTime = sd.schedule.schedule_time,
                        Rooms = sd.display_date.room_display_date.Select(rd => new
                        {
                            Room = rd.room.room_name,
                            SeatQuantity = rd.room.seat_quantity
                        })
                    })
                })
            });  
           

            ViewBag.MovieList = movielist;
            ViewBag.DisplayDate = displaydate;
            return View();
        }
        [HttpGet]
        public ActionResult GetScheduleForDate(int movieId, DateTime date)
        {
            //var scheduleLists = db.schedules.ToList();
            //foreach (var schedule in scheduleLists)
            //{
            //    schedule.schedule_detail.ToList();
            //}

            //Truy vấn lịch chiếu từ cơ sở dữ liệu dựa trên movieId và date
                var scheduleList = (from sd in db.schedule_detail
                                    join d in db.display_date on sd.display_date_id equals d.id
                                    join s in db.schedules on sd.schedule_id equals s.id
                                    join md in db.movie_display_date on d.id equals md.display_date_id
                                    where md.movie_id == movieId && d.display_date1 == date
                                    select new schedule
                                    {
                                        id = s.id,
                                        schedule_time = s.schedule_time
                                    }).ToList();
            return Json(new {schedule = scheduleList}, JsonRequestBehavior.AllowGet);
            
        }
    }
}