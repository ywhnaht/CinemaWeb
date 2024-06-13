using CinemaWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaWeb.Areas.Admin.Controllers
{
    public class RevenueController : Controller
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
        // GET: Admin/Revenue
        public ActionResult OveralRevenue()
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            List<movy> movieList = db.movies.ToList();
            GetMovieStatus(movieList);
            db.SaveChanges();
            movieList = movieList.Where(x => x.movie_status != false).ToList();
            var roomList = db.rooms.ToList();

            ViewBag.MovieList = movieList;
            ViewBag.RoomList = roomList;

            var monthlySales = db.invoices
                .Where(i => i.day_create.Value.Year == currentYear)
                .GroupBy(i => i.day_create.Value.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalRevenue = g.Sum(i => i.total_money)
                }).ToList();

            var allMonths = Enumerable.Range(1, 12).Select(m => new
            {
                Month = m,
                TotalRevenue = monthlySales.FirstOrDefault(ms => ms.Month == m)?.TotalRevenue ?? 0
            }).ToList();

            var yearlySales = db.invoices
                .GroupBy(i => i.day_create.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    TotalRevenue = g.Sum(i => i.total_money)
                }).ToList();

            var minYear = DateTime.Now.AddYears(-3).Year;
            var maxYear = currentYear;
            var allYears = Enumerable.Range(minYear, maxYear - minYear + 1).Select(y => new
            {
                Year = y,
                TotalRevenue = yearlySales.FirstOrDefault(ys => ys.Year == y)?.TotalRevenue ?? 0
            }).ToList();

            ViewBag.MonthlySales = allMonths;
            ViewBag.YearlySales = allYears;

            return View();
        }

        [HttpGet]
        public ActionResult GetRevenue(int movieId, int roomId, DateTime dateFrom, DateTime dateEnd)
        {
            var invoiceList = (from m in db.movies
                              join mdd in db.movie_display_date on m.id equals mdd.movie_id
                              join dd in db.display_date on mdd.display_date_id equals dd.id
                              join sd in db.schedule_detail on mdd.id equals sd.movie_display_date_id
                              join s in db.schedules on sd.schedule_id equals s.id
                              join rsd in db.room_schedule_detail on sd.id equals rsd.schedule_detail_id
                              join r in db.rooms on rsd.room_id equals r.id
                              join i in db.invoices on rsd.id equals i.room_schedule_detail_id
                              join t in db.tickets on i.id equals t.invoice_id
                              join seat in db.seats on t.seat_id equals seat.id
                              where i.day_create >= dateFrom && i.day_create <= dateEnd &&
                                (roomId == 0 || rsd.room_id == roomId) &&
                                (movieId == 0 || m.id == movieId) &&
                                i.invoice_status == true
                              select new
                              {
                                  MovieId = m.id,
                                  RoomId = r.id,
                                  MovieTitle = m.title,
                                  MovieImage = m.url_image,
                                  ScheduleTime = s.schedule_time,
                                  DisplayDate = dd.display_date1,
                                  RoomName = r.room_name,
                                  TotalMoney = i.total_money,
                                  TotalTicket = i.total_ticket,
                                  SeatList = new
                                  {
                                      seat.seat_row,
                                      seat.seat_column
                                  }
                              }).ToList();

            var totalInvoice = invoiceList.GroupBy(m => new { m.MovieTitle, m.MovieImage, m.RoomName, m.DisplayDate, m.ScheduleTime, m.TotalMoney, m.TotalTicket})
                               .Select(g => new
                               {
                                   MovieTitle = g.Key.MovieTitle,
                                   MovieImage = g.Key.MovieImage,
                                   RoomName = g.Key.RoomName,
                                   DisplayDate = g.Key.DisplayDate.Value.ToString("dd/MM/yyyy"),
                                   ScheduleTime = g.Key.ScheduleTime.Value.ToString(@"hh\:mm"),
                                   TotalMoney = g.Key.TotalMoney,
                                   TotalTicket = g.Key.TotalTicket,
                                   SeatList = g.Select(x => x.SeatList).ToList()
                               }).ToList();
            if (totalInvoice.Count != 0) 
                return Json(new { success = true , totalInvoice}, JsonRequestBehavior.AllowGet);
            else 
                return Json(new { success = false, message = "Không tìm thấy vé!" }, JsonRequestBehavior.AllowGet);
        }
    }
}