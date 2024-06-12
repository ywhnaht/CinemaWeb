using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using CinemaWeb.Areas.Admin.Helper;

namespace CinemaWeb.Areas.Admin.Controllers
{
    public class UploadController : Controller
    {
        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    var uploadsDir = Server.MapPath("~/up");
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsDir, fileName);

                    file.SaveAs(filePath);

                    var baseUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}";
                    var publicUrl = $"{baseUrl}/up/{fileName}";

                    return Json(new { success = true, url = publicUrl });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, error = ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, error = "Không có tệp nào được tải lên hoặc tệp trống." });
            }
        }
    }
}
