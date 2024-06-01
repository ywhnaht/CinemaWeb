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
                              RoomId = rsd.room_id
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
