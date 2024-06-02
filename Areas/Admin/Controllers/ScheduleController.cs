using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CinemaWeb.Models;
using System.Data.Entity.Core.Objects;

namespace CinemaWeb.Areas.Admin.Controllers
{
    public class ScheduleController : Controller
    {
        private Cinema_Web_Entities db = new Cinema_Web_Entities();

        // GET: Admin/Schedule
        public ActionResult Index()
        {
            var roomList = db.rooms.ToList();
            ViewBag.RoomList = roomList;
            var movieList = db.movies.Where(x => x.movie_status != null).ToList();
            ViewBag.MovieList = movieList;
            var scheduleList = db.schedules.ToList();
            ViewBag.ScheduleList = scheduleList;
            foreach (var item in scheduleList)
            {
            }
            var schedule_detail = db.schedule_detail.Include(s => s.movie_display_date).Include(s => s.schedule);
            return View();
        }

        [HttpPost]
        public ActionResult GetSchedule(DateTime displayDate)
        {
            var movies = (from m in db.movies
                         join mdd in db.movie_display_date on m.id equals mdd.movie_id
                         join dd in db.display_date on mdd.display_date_id equals dd.id
                         join sd in db.schedule_detail on mdd.id equals sd.movie_display_date_id
                         join s in db.schedules on sd.schedule_id equals s.id
                         join rsd in db.room_schedule_detail on sd.id equals rsd.schedule_detail_id
                         where dd.display_date1 == displayDate
                         orderby s.id
                         select new
                         {
                             MovieTitle = m.title,
                             MovieImage = m.url_image,
                             Duration = m.duration_minutes,
                             ShowTimes = s.schedule_time
                         }).ToList();

            var movieList = movies.GroupBy(m => new { m.MovieTitle, m.MovieImage, m.Duration })
                  .Select(g => new
                  {
                      MovieTitle = g.Key.MovieTitle,
                      MovieImage = g.Key.MovieImage,
                      Duration = g.Key.Duration,
                      ShowTimes = g.Select(m => m.ShowTimes.Value.ToString(@"hh\:mm")).ToList() // Chuyển đổi thời gian sang chuỗi ở đây
                  }).ToList();

            return Json(new {success = true, movieList});
        }

        [HttpPost]
        public ActionResult MovieByRoom(int roomId, DateTime displayDate)
        {
            var movies = (from m in db.movies
                          join mdd in db.movie_display_date on m.id equals mdd.movie_id
                          join dd in db.display_date on mdd.display_date_id equals dd.id
                          join sd in db.schedule_detail on mdd.id equals sd.movie_display_date_id
                          join s in db.schedules on sd.schedule_id equals s.id
                          join rsd in db.room_schedule_detail on sd.id equals rsd.schedule_detail_id
                          where dd.display_date1 == displayDate &&
                            (roomId == 0 || rsd.room_id == roomId)
                          orderby s.id
                          select new
                          {
                              MovieTitle = m.title,
                              MovieImage = m.url_image,
                              Duration = m.duration_minutes,
                              ShowTimes = s.schedule_time,
                              RoomId = rsd.room_id,
                          }).ToList();
            //if (roomId != 0)
            //{
            //    movies.Where(x => x.RoomId == roomId).ToList();
            //}

            var movieList = movies.GroupBy(m => new { m.MovieTitle, m.MovieImage, m.Duration })
                  .Select(g => new
                  {
                      MovieTitle = g.Key.MovieTitle,
                      MovieImage = g.Key.MovieImage,
                      Duration = g.Key.Duration,
                      ShowTimes = g.Select(m => m.ShowTimes.Value.ToString(@"hh\:mm")).ToList() 
                  }).ToList();

            return Json(new { success = true, movieList });
        }

        [HttpPost]
        public ActionResult MovieSelected(int movieId)
        {
            var movie = db.movies
                  .Where(x => x.id == movieId)
                  .Select(x => new
                  {
                      x.title,
                      x.url_image,
                      x.release_date,
                      x.end_date
                  })
                  .FirstOrDefault();

            if (movie != null)
            {
                DateTime currentDate = DateTime.Now;
                DateTime minDate;

                if (movie.release_date < currentDate)
                {
                    minDate = currentDate;
                }
                else
                {
                    minDate = movie.release_date.Value;
                }

                var displayDate = new
                {
                    movieTitle = movie.title,
                    movieImage = movie.url_image,
                    dateFrom = minDate.ToString("yyyy-MM-dd"),
                    dateEnd = movie.end_date.Value.ToString("yyyy-MM-dd")
                };
                return Json(new { success = true, displayDate });
            }
            else
            {
                return Json(new { success = false, message = "Không tìm thấy phim" });
            }
        }

        [HttpPost]
        public ActionResult GetScheduleValid(int movieId, DateTime displayDate, int roomId)
        {
            var movie = db.movies.FirstOrDefault(x => x.id == movieId);

            int durationMinute;
            int.TryParse(movie.duration_minutes, out durationMinute);
            var schedules = (from m in db.movies
                             join mdd in db.movie_display_date on m.id equals mdd.movie_id
                             join dd in db.display_date on mdd.display_date_id equals dd.id
                             join sd in db.schedule_detail on mdd.id equals sd.movie_display_date_id
                             join s in db.schedules on sd.schedule_id equals s.id
                             join rsd in db.room_schedule_detail on sd.id equals rsd.schedule_detail_id
                             where dd.display_date1 == displayDate && rsd.room_id == roomId
                             orderby s.id
                             select new
                             {
                                 DurationMinute = m.duration_minutes,
                                 ScheduleDetailId = sd.id,
                                 StartTime = s.schedule_time,
                                 EndTime = s.schedule_time
                             }).ToList();

            var scheduleTime = schedules.Select(x => new { x.StartTime, EndTime = x.EndTime.Value.Add(TimeSpan.FromMinutes(int.Parse(x.DurationMinute))) }).ToList();
            var duration = TimeSpan.FromMinutes(durationMinute);

            var scheduleAll = db.schedules.ToList();

            var scheduleList = scheduleAll.Select(x => new
            {
                scheduleId = x.id,
                timeFrom = x.schedule_time,
                timeEnd = x.schedule_time.Value.Add(duration)
            }).ToList();

            //var scheduleValid = (from sv in scheduleList
            //                     where !scheduleTime.Any(x => sv.timeFrom.Value >= x.StartTime.Value && sv.timeEnd <= x.EndTime)
            //                     select sv.timeFrom
            //                     ).ToList();

            var scheduleValid = (from sv in scheduleList
                                 where !scheduleTime.Any(x => sv.timeEnd >= x.StartTime && sv.timeFrom <= x.EndTime)
                                 select new
                                 {
                                     sv.timeFrom,
                                     sv.scheduleId
                                 }
                          ).ToList();

            if (scheduleValid.Any())
            {
                var allschedule = scheduleValid.Select(x => new
                {
                    ScheduleId = x.scheduleId,
                    Schedule = x.timeFrom.Value.ToString(@"hh\:mm")
                }).ToList();
                return Json(new { success = true, allschedule});
            }

            return Json(new { success = false, message = "Hiện không có suất chiếu trông"});
        }


        // GET: Admin/Schedule/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //schedule_detail schedule_detail = await db.schedule_detail.FindAsync(id);
            //if (schedule_detail == null)
            //{
            //    return HttpNotFound();
            //}
            return View();
        }

        // GET: Admin/Schedule/Create
        public ActionResult Create()
        {
            ViewBag.movie_display_date_id = new SelectList(db.movie_display_date, "id", "id");
            ViewBag.schedule_id = new SelectList(db.schedules, "id", "id");
            return View();
        }

        // POST: Admin/Schedule/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,movie_display_date_id,schedule_id,start_time,end_time")] schedule_detail schedule_detail)
        {
            if (ModelState.IsValid)
            {
                db.schedule_detail.Add(schedule_detail);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.movie_display_date_id = new SelectList(db.movie_display_date, "id", "id", schedule_detail.movie_display_date_id);
            ViewBag.schedule_id = new SelectList(db.schedules, "id", "id", schedule_detail.schedule_id);
            return View(schedule_detail);
        }

        // GET: Admin/Schedule/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            schedule_detail schedule_detail = await db.schedule_detail.FindAsync(id);
            if (schedule_detail == null)
            {
                return HttpNotFound();
            }
            ViewBag.movie_display_date_id = new SelectList(db.movie_display_date, "id", "id", schedule_detail.movie_display_date_id);
            ViewBag.schedule_id = new SelectList(db.schedules, "id", "id", schedule_detail.schedule_id);
            return View(schedule_detail);
        }

        // POST: Admin/Schedule/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,movie_display_date_id,schedule_id,start_time,end_time")] schedule_detail schedule_detail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(schedule_detail).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.movie_display_date_id = new SelectList(db.movie_display_date, "id", "id", schedule_detail.movie_display_date_id);
            ViewBag.schedule_id = new SelectList(db.schedules, "id", "id", schedule_detail.schedule_id);
            return View(schedule_detail);
        }

        // GET: Admin/Schedule/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            schedule_detail schedule_detail = await db.schedule_detail.FindAsync(id);
            if (schedule_detail == null)
            {
                return HttpNotFound();
            }
            return View(schedule_detail);
        }

        // POST: Admin/Schedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            schedule_detail schedule_detail = await db.schedule_detail.FindAsync(id);
            db.schedule_detail.Remove(schedule_detail);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
