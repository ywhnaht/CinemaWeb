using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaWeb.Areas.Admin.Controllers
{
    public class MovieController : Controller
    {
        // GET: Admin/Movie

        public ActionResult AllMovie()
        {
            return View();
        }
        public ActionResult AddMovie()
        {
            return View();
        }

        public ActionResult RemoveMovie()
        {
            return View();
        }
        public ActionResult EditMovie()
        {
            return View();
        }
    }
}