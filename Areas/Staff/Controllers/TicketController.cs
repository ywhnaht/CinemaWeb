using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using ZXing.Aztec;

namespace CinemaWeb.Areas.Staff.Controllers
{
    public class TicketController : Controller
    {
        // GET: Staff/Ticket
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Scan(string imageData)
        {
            // Decode the base64 image data
            var base64Data = imageData.Substring(imageData.IndexOf(",") + 1);
            var bytes = Convert.FromBase64String(base64Data);

            using (var ms = new MemoryStream(bytes))
            {
                using (var bitmap = new Bitmap(ms))
                {
                    // Use ZXing to decode the QR code
                    var reader = new BarcodeReader();
                    var result = reader.Decode(bitmap);

                    if (result != null)
                    {
                        return Json(new { content = result.Text });
                    }
                }
            }

            return Json(new { content = "No QR code detected" });
        }

        public ActionResult ScanQRCode()
        {
            return View();
        }
    }
}