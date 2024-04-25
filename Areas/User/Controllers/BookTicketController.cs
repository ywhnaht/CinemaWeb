using CinemaWeb.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

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
            movielist = movielist.OrderByDescending(m => m.release_date).ToList();

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
        public ActionResult GetScheduleTime(int displaydateId, int movieId)
        {
            var scheduleList = db.schedule_detail
                        .Where(x => x.movie_display_date.display_date_id == displaydateId &&
                                    x.movie_display_date.movie_id == movieId)
                        .Select(x => new
                        {
                            x.schedule.id,
                            schedule_time = DbFunctions.CreateDateTime(x.movie_display_date.display_date.display_date1.Value.Year,
                                                                       x.movie_display_date.display_date.display_date1.Value.Month,
                                                                       x.movie_display_date.display_date.display_date1.Value.Day, 
                                                                       x.schedule.schedule_time.Value.Hours,
                                                                       x.schedule.schedule_time.Value.Minutes, 
                                                                       x.schedule.schedule_time.Value.Seconds) // Chuyển đổi TimeSpan sang DateTime
                        })
                        .ToList();
            return Json(scheduleList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetRoomSeat(int scheduleId, int displaydateId, int movieId)
        {
            var roomSeatsList = db.room_schedule_detail
                .Where(x => x.schedule_detail.schedule_id == scheduleId &&
                            x.schedule_detail.movie_display_date.display_date_id == displaydateId &&
                            x.schedule_detail.movie_display_date.movie_id == movieId)
                .Select(x => new
                {
                    RoomId = x.room.id,
                    RoomName = x.room.room_name,
                    Seats = x.room.seats.Select(seat => new
                    {
                        SeatId = seat.id,
                        SeatCol = seat.seat_column,
                        SeatRow = seat.seat_row,
                        SeatStt = seat.seat_status,
                        SeatPrice = seat.price
                    }).ToList()
                })
                .ToList();

            return Json(roomSeatsList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MovieDetail()
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
            movielist = movielist.OrderByDescending(m => m.release_date).ToList();
            ViewBag.MovieList = movielist;

            var segments = System.Web.HttpContext.Current.Request.Url.Segments;
            var movieIdSegment = segments[segments.Length - 1].TrimEnd('/');
            var movieId = int.Parse(movieIdSegment);

            var movieItem = db.movies.FirstOrDefault(x => x.id == movieId);
            ViewBag.movieItem = movieItem;

            var movieActor = db.movie_actor.Where(x => x.movie_id == movieId).ToList();
            ViewBag.movieActor = movieActor;

            var movieDateList = db.movie_display_date
                        .Where(x => x.movie_id == movieId)
                        .ToList();
            ViewBag.movieDateList = movieDateList;
            return View();
        }
       
        //[HttpGet]
        //public ActionResult GetInvoice(int scheduleId, int displaydateId, int movieId, seat[] seats)
        //{
        //    List<int> chosenSeats = new List<int>();
        //    int index = chosenSeats.IndexOf(selectedSeatId);
        //    if (index == -1)
        //    {
        //        // Nếu chưa được chọn, thêm ghế vào danh sách
        //        chosenSeats.Add(selectedSeatId);
        //    }
        //    else
        //    {
        //        // Nếu đã được chọn, loại bỏ ghế khỏi danh sách
        //        chosenSeats.RemoveAt(index);
        //    }
        //    return Json(new { success = true });
        //}
    }
}