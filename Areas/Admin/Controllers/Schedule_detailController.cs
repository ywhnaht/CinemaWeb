using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CinemaWeb.Models;

namespace CinemaWeb.Areas.Admin.Controllers
{
    public class Schedule_detailController : Controller
    {
        private Cinema_Web_Entities db = new Cinema_Web_Entities();

        // GET: Admin/Schedule_detail
        public ActionResult Index()
        {
            var scheduleDetails = db.schedule_detail
            .Include(s => s.movie_display_date)
            .Include(s => s.movie_display_date.movy.title)
            .Include(s => s.schedule)
            .Include(s => s.room_schedule_detail.Select(rsd => rsd.room))
            .Select(s => new ScheduleDetailViewModel
            {
                ScheduleDetailId = s.id,
                MovieName = s.movie_display_date.movy.title,
                RoomName = s.room_schedule_detail.Select(rsd => rsd.room.room_name).FirstOrDefault(),
                MovieDisplayDate = (DateTime)s.movie_display_date.display_date.display_date1,
                ScheduleTime = s.schedule.schedule_time
            })
            .ToList();

            return View(scheduleDetails);
        }

        // GET: Admin/Schedule_detail/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            schedule_detail schedule_detail = db.schedule_detail.Find(id);
            if (schedule_detail == null)
            {
                return HttpNotFound();
            }
            return View(schedule_detail);
        }

        // GET: Admin/Schedule_detail/Create
        public ActionResult Create()
        {
            var movieNameList = db.movies
            .Select(m => new SelectListItem
            {
                Value = m.id.ToString(),
                Text = m.title
            });
            SelectList movieNameSelectList = new SelectList(movieNameList, "Value", "Text");

            // Tạo SelectList cho danh sách display_date
            var displayDateList = db.movie_display_date
                .Include(mdd => mdd.display_date)
                .Select(mdd => new SelectListItem
                {
                    Value = mdd.id.ToString(),
                    Text = mdd.display_date.display_date1.ToString()
                });
            SelectList displayDateSelectList = new SelectList(displayDateList, "Value", "Text");

            // Đưa SelectList vào ViewBag để sử dụng trong view
            ViewBag.movieNameSelectList = movieNameSelectList;
            ViewBag.displayDateSelectList = displayDateSelectList;
            ViewBag.schedule_id = new SelectList(db.schedules, "id", "schedule_time");
            ViewBag.room_id = new SelectList(db.rooms, "id", "room_name");
            return View();
        }

        // POST: Admin/Schedule_detail/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "movie_id, display_date_id, schedule_id, start_time, end_time, room_id")] ScheduleDetailCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var movie = db.movies.Find(model.movie_id);
                if (movie == null)
                {
                    ModelState.AddModelError("movie_id", "Invalid movie selected.");
                    ViewBag.movie_display_date_id = new SelectList(db.movie_display_date.Include(mdd => mdd.movy), "id", "movy.title", model.movie_display_date_id);
                    ViewBag.schedule_id = new SelectList(db.schedules, "id", "schedule_time", model.schedule_id);
                    ViewBag.room_id = new SelectList(db.rooms, "id", "room_name", model.room_id);
                    return View(model);
                }

                var movieDisplayDate = new movie_display_date
                {
                    movie_id = model.movie_id,
                    display_date_id = model.display_date_id
                };

                db.movie_display_date.Add(movieDisplayDate);
                db.SaveChanges();

                var movieDisplayDateId = movieDisplayDate.id;

                var scheduleDetail = new schedule_detail
                {
                    movie_display_date_id = movieDisplayDateId,
                    schedule_id = model.schedule_id,
                    //start_time = model.start_time.ToString(),
                    //end_time = model.end_time
                };

                db.schedule_detail.Add(scheduleDetail);
                db.SaveChanges();

                var scheduleDetailId = scheduleDetail.id;

                var roomScheduleDetail = new room_schedule_detail
                {
                    schedule_detail_id = scheduleDetailId,
                    room_id = model.room_id
                };

                db.room_schedule_detail.Add(roomScheduleDetail);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.movie_display_date_id = new SelectList(db.movie_display_date.Include(mdd => mdd.movy), "id", "movie.name", model.movie_display_date_id);
            ViewBag.schedule_id = new SelectList(db.schedules, "id", "schedule_time", model.schedule_id);
            ViewBag.room_id = new SelectList(db.rooms, "id", "name", model.room_id);
            return View(model);
        }

        // GET: Admin/Schedule_detail/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            schedule_detail schedule_detail = db.schedule_detail.Find(id);
            if (schedule_detail == null)
            {
                return HttpNotFound();
            }
            ViewBag.movie_display_date_id = new SelectList(db.movie_display_date, "id", "id", schedule_detail.movie_display_date_id);
            ViewBag.schedule_id = new SelectList(db.schedules, "id", "id", schedule_detail.schedule_id);
            return View(schedule_detail);
        }

        // POST: Admin/Schedule_detail/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,movie_display_date_id,schedule_id,start_time,end_time")] schedule_detail schedule_detail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(schedule_detail).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.movie_display_date_id = new SelectList(db.movie_display_date, "id", "id", schedule_detail.movie_display_date_id);
            ViewBag.schedule_id = new SelectList(db.schedules, "id", "id", schedule_detail.schedule_id);
            return View(schedule_detail);
        }

        // GET: Admin/Schedule_detail/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            schedule_detail schedule_detail = db.schedule_detail.Find(id);
            if (schedule_detail == null)
            {
                return HttpNotFound();
            }
            return View(schedule_detail);
        }

        // POST: Admin/Schedule_detail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            schedule_detail schedule_detail = db.schedule_detail.Find(id);
            db.schedule_detail.Remove(schedule_detail);
            db.SaveChanges();
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
