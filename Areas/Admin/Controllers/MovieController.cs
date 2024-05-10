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
    public class MovieController : Controller
    {
        private Cinema_Web_Entities db = new Cinema_Web_Entities();

        // GET: Admin/Movie
        public async Task<ActionResult> Index()
        {
            var movies = db.movies.Include(m => m.country).Include(m => m.director).Include(m => m.movie_type);
            return View(await movies.ToListAsync());
        }

        // GET: Admin/Movie/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            movy movy = await db.movies.FindAsync(id);
            if (movy == null)
            {
                return HttpNotFound();
            }
            return View(movy);
        }

        // GET: Admin/Movie/Create
        public ActionResult Create()
        {
            ViewBag.country_id = new SelectList(db.countries, "id", "country_name");
            ViewBag.director_id = new SelectList(db.directors, "id", "director_name");
            ViewBag.type_id = new SelectList(db.movie_type, "id", "movie_type1");
            return View();
        }

        // POST: Admin/Movie/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,title,description,director_id,type_id,release_date,end_date,duration_minutes,country_id,created_at,movie_status,url_image,rating,url_trailer,url_large_image")] movy movy)
        {
            if (ModelState.IsValid)
            {
                db.movies.Add(movy);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.country_id = new SelectList(db.countries, "id", "country_name", movy.country_id);
            ViewBag.director_id = new SelectList(db.directors, "id", "director_name", movy.director_id);
            ViewBag.type_id = new SelectList(db.movie_type, "id", "movie_type1", movy.type_id);
            return View(movy);
        }

        // GET: Admin/Movie/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            movy movy = await db.movies.FindAsync(id);
            if (movy == null)
            {
                return HttpNotFound();
            }
            ViewBag.country_id = new SelectList(db.countries, "id", "country_name", movy.country_id);
            ViewBag.director_id = new SelectList(db.directors, "id", "director_name", movy.director_id);
            ViewBag.type_id = new SelectList(db.movie_type, "id", "movie_type1", movy.type_id);
            return View(movy);
        }

        // POST: Admin/Movie/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,title,description,director_id,type_id,release_date,end_date,duration_minutes,country_id,created_at,movie_status,url_image,rating,url_trailer,url_large_image")] movy movy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movy).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.country_id = new SelectList(db.countries, "id", "country_name", movy.country_id);
            ViewBag.director_id = new SelectList(db.directors, "id", "director_name", movy.director_id);
            ViewBag.type_id = new SelectList(db.movie_type, "id", "movie_type1", movy.type_id);
            return View(movy);
        }

        // GET: Admin/Movie/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            movy movy = await db.movies.FindAsync(id);
            if (movy == null)
            {
                return HttpNotFound();
            }
            return View(movy);
        }

        // POST: Admin/Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            movy movy = await db.movies.FindAsync(id);
            db.movies.Remove(movy);
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
