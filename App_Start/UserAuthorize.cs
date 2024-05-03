using CinemaWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CinemaWeb.App_Start
{
    public class UserAuthorize : AuthorizeAttribute
    {
        public int roleId { get; set; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // 1. Check session : Đã đăng nhập => cho thực hiện Filter
            // Ngược lại cho trở lại trang đăng nhập
            user currentUser = (user)HttpContext.Current.Session["user"];
            if (currentUser != null)
            {
                //2. Check quyền : Có quyền thì cho thực hiện Filter
                // Ngược lại thì báo lỗi
                Cinema_Web_Entities db = new Cinema_Web_Entities();
                var count = db.user_type_user_role.Count(x => x.user_type_id == currentUser.user_type && x.user_role_id == roleId);
                if (count > 0)
                {
                    return;
                }
                else
                {
                    var returnUrl = filterContext.RequestContext.HttpContext.Request.RawUrl;
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Home",
                        action = "Error",
                        area = "",
                        returnUrl = returnUrl.ToString()
                    }));
                }
                return;
            }
            else
            {
                var returnUrl = filterContext.RequestContext.HttpContext.Request.RawUrl;
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "SignIn",
                    area = "",
                    returnUrl = returnUrl.ToString()
                }));
            }
        }
    }
}