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
    public class Schedule_detailController : Controller
    {
        private Cinema_Web_Entities db = new Cinema_Web_Entities();

        // GET: Admin/Schedule_detail
        public async Task<ActionResult> Index()
        {
            var schedule_detail = db.schedule_detail.Include(s => s.movie_display_date).Include(s => s.schedule);
            return View(await schedule_detail.ToListAsync());
        }

        // GET: Admin/Schedule_detail/Details/5
        public async Task<ActionResult> Details(int? id)
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

        // GET: Admin/Schedule_detail/Create
        public ActionResult Create()
        {
            ViewBag.movie_display_date_id = new SelectList(db.movie_display_date, "id", "id");
            ViewBag.schedule_id = new SelectList(db.schedules, "id", "id");
            return View();
        }

        // POST: Admin/Schedule_detail/Create
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

        // GET: Admin/Schedule_detail/Edit/5
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

        // POST: Admin/Schedule_detail/Edit/5
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

        // GET: Admin/Schedule_detail/Delete/5
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

        // POST: Admin/Schedule_detail/Delete/5
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
