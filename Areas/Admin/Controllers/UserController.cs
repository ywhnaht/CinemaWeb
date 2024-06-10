using CinemaWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaWeb.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        Cinema_Web_Entities db = new Cinema_Web_Entities();
        // GET: Admin/User
        public ActionResult UserList()
        {
            var userList = db.users.Where(x => x.user_type == 1).ToList();
            ViewBag.UserList = userList;
            return View();
        }

        [HttpGet]
        public ActionResult SearchUser(string query)
        {
            var userLists = db.users
                .Where(x => (x.full_name.Contains(query) || x.email.Contains(query)) && x.user_type == 1)
                .Select(x => new {
                    id = x.id,
                    full_name = x.full_name,
                    date_of_birth = x.date_of_birth,
                    email = x.email
                })
                .ToList();

            if (userLists.Count == 0)
            {
                return Json(new { success = false, message = "Không tìm thấy nhân viên" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var userList = userLists.GroupBy(x => new { x.id, x.full_name, x.date_of_birth, x.email })
                                          .Select(g => new
                                          {
                                              g.Key.id,
                                              g.Key.full_name,
                                              userDate = g.Key.date_of_birth.Value.ToString("dd/MM/yyyy"),
                                              g.Key.email
                                          }).ToList();
                return Json(new { success = true, userList }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}