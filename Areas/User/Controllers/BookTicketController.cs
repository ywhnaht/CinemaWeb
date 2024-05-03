using CinemaWeb.App_Start;
using CinemaWeb.Models;
using CinemaWeb.Models.Services;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace CinemaWeb.Areas.User.Controllers
{
    public class BookTicketController : Controller
    {
        Cinema_Web_Entities db = new Cinema_Web_Entities();
        IVnPayService _vnPayService = new VnPayService();
        public enum RoleName
        {
            AddMovie = 1, 
            RemoveMovie = 2,
            EditMovie = 3,
            Statistical = 4,
            AddRoom = 5,
            RemoveRoom = 6,
            AddSchedule = 7,
            RemoveSchedule = 8,
            AddStaff = 9,
            RemoveStaff = 10,
            RemoveUser = 11,
            BookTicket = 12,
            EditProfile = 13,
            Payment = 14,
            CheckIn = 15
        }
        public void GetMovieStatus(List<movy> movielist)
        {
            DateTime currentDate = DateTime.Now;
            foreach (movy movie in movielist)
            {
                if (movie.release_date <= currentDate && movie.end_date >= currentDate)
                {
                    movie.movie_status = true; // Đang chiếu
                }
                else if (movie.release_date > currentDate)
                {
                    movie.movie_status = false; // Sắp chiếu
                }
                else
                {
                    movie.movie_status = null;
                }
            }
        }
        // GET: User/BookTicket

        [UserAuthorize(roleId = (int)RoleName.BookTicket)]
        public ActionResult BookTicket()
        {
            List<movy> movielist = db.movies.ToList();
            GetMovieStatus(movielist);
            movielist = movielist.OrderByDescending(m => m.release_date).ToList();
            ViewBag.MovieList = movielist;
            return View();
        }
        [HttpGet]
        public ActionResult GetDisplayDate(int movieId)
        {
            var movieDateList = db.movie_display_date
                        .Where(x => x.movie_id == movieId)
                        .Select(x => new {
                            x.display_date.id,
                            x.display_date.display_date1
                        })
                        .ToList();
           
            return Json(movieDateList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetScheduleTime(int displaydateId, int movieId)
        {
            var scheduleList = db.schedule_detail
                        .Where(x => x.movie_display_date.display_date_id == displaydateId &&
                                    x.movie_display_date.movie_id == movieId)
                        .Select(x => new
                        {
                            x.schedule.id,
                            schedule_time = DbFunctions.CreateDateTime(x.movie_display_date.display_date.display_date1.Value.Year,
                                                                       x.movie_display_date.display_date.display_date1.Value.Month,
                                                                       x.movie_display_date.display_date.display_date1.Value.Day, 
                                                                       x.schedule.schedule_time.Value.Hours,
                                                                       x.schedule.schedule_time.Value.Minutes, 
                                                                       x.schedule.schedule_time.Value.Seconds) // Chuyển đổi TimeSpan sang DateTime
                        })
                        .ToList();
            return Json(scheduleList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetRoomSeat(int scheduleId, int displaydateId, int movieId)
        {
            var roomSeatsList = db.room_schedule_detail
                .Where(x => x.schedule_detail.schedule_id == scheduleId &&
                            x.schedule_detail.movie_display_date.display_date_id == displaydateId &&
                            x.schedule_detail.movie_display_date.movie_id == movieId)
                .Select(x => new
                {
                    RoomId = x.room.id,
                    RoomName = x.room.room_name,
                    Seats = x.room.seats.Select(seat => new
                    {
                        SeatId = seat.id,
                        SeatCol = seat.seat_column,
                        SeatRow = seat.seat_row,
                        SeatStt = seat.seat_status,
                        SeatPrice = seat.price
                    }).ToList()
                })
                .ToList();

            return Json(roomSeatsList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MovieDetail()
        {
            List<movy> movielist = db.movies.ToList();
            GetMovieStatus(movielist);
            movielist = movielist.OrderByDescending(m => m.release_date).ToList();
            ViewBag.MovieList = movielist;

            var segments = System.Web.HttpContext.Current.Request.Url.Segments;
            var movieIdSegment = segments[segments.Length - 1].TrimEnd('/');
            var movieId = int.Parse(movieIdSegment);

            var movieItem = db.movies.FirstOrDefault(x => x.id == movieId);
            ViewBag.movieItem = movieItem;

            var movieActor = db.movie_actor.Where(x => x.movie_id == movieId).ToList();
            ViewBag.movieActor = movieActor;

            var movieDateList = db.movie_display_date
                        .Where(x => x.movie_id == movieId)
                        .ToList();
            ViewBag.movieDateList = movieDateList;
            return View();
        }

        public ActionResult MovieType()
        {
            List<movy> movielist = db.movies.ToList();
            GetMovieStatus(movielist);

            movielist = movielist.OrderByDescending(m => m.release_date).ToList();
            ViewBag.MovieList = movielist;


            var segments = System.Web.HttpContext.Current.Request.Url.Segments;
            var typeIdSegment = segments[segments.Length - 1].TrimEnd('/');
            int? typeId = null;
            int? countryId = null;
            int? yearValue = null;
            bool? movieStatus = null;
            bool? movieSelect = null;

            if (!string.IsNullOrEmpty(typeIdSegment))
            {
                int mtIndex = typeIdSegment.IndexOf("mt");
                int mcIndex = typeIdSegment.IndexOf("mc");
                int myIndex = typeIdSegment.IndexOf("my");
                int stIndex = typeIdSegment.IndexOf("st");
                int ssIndex = typeIdSegment.IndexOf("ss");

                // Kiểm tra xem "mt" có tồn tại và nằm ở vị trí đầu tiên
                if (mtIndex == 0)
                {
                    // Lấy chuỗi con sau "mt"
                    string typeIdStr = typeIdSegment.Substring(mtIndex + 2);

                    // Kiểm tra xem có "mc" trong chuỗi typeIdStr hay không
                    int mcIndexInTypeIdStr = typeIdStr.IndexOf("mc");

                    // Nếu có "mc", lấy typeId từ đầu chuỗi typeIdStr đến "mc"
                    if (mcIndexInTypeIdStr >= 0)
                    {
                        typeIdStr = typeIdStr.Substring(0, mcIndexInTypeIdStr);
                    }

                    int parsedTypeId;
                    if (int.TryParse(typeIdStr, out parsedTypeId))
                    {
                        typeId = parsedTypeId;
                    }
                }

                if (mcIndex > mtIndex)
                {
                    string countryIdStr = typeIdSegment.Substring(mcIndex + 2);

                    int myIndexInTypeIdStr = countryIdStr.IndexOf("my");

                    if (myIndexInTypeIdStr >= 0)
                    {
                        countryIdStr = countryIdStr.Substring(0, myIndexInTypeIdStr);
                    }

                    int parsedCountryId;
                    if (int.TryParse(countryIdStr, out parsedCountryId))
                    {
                        countryId = parsedCountryId;
                    }
                }

                if (myIndex > mcIndex)
                {
                    string yearIdStr = typeIdSegment.Substring(myIndex + 2);

                    int stIndexInTypeIdStr = yearIdStr.IndexOf("st");

                    if (stIndexInTypeIdStr >= 0)
                    {
                        yearIdStr = yearIdStr.Substring(0, stIndexInTypeIdStr);
                    }

                    int parsedYearValue;
                    if (int.TryParse(yearIdStr, out parsedYearValue))
                    {
                        yearValue = parsedYearValue;
                    }
                }

                if (stIndex > myIndex)
                {
                    string movieSttStr = typeIdSegment.Substring(stIndex + 2);

                    int ssIndexInTypeIdStr = movieSttStr.IndexOf("ss");

                    if (ssIndexInTypeIdStr >= 0)
                    {
                        movieSttStr = movieSttStr.Substring(0, ssIndexInTypeIdStr);
                    }

                    if (movieSttStr == "1") movieStatus = true;
                    else if (movieSttStr == "0") movieStatus = false;
                }

                if (ssIndex > stIndex)
                {
                    string movieSelectStr = typeIdSegment.Substring(ssIndex + 2);

                    if (movieSelectStr == "1") movieSelect = true;
                    else if (movieSelectStr == "0") movieSelect = false;
                }
            }

            if (typeId != null)
            {
                movielist = movielist.Where(x => x.type_id == typeId).ToList();
                var typeName = db.movie_type.FirstOrDefault(x => x.id == typeId).movie_type1;
                ViewBag.TypeName = typeName;
                ViewBag.SelectedTypeId = typeId;
            }

            if (countryId != null)
            {
                movielist = movielist.Where(x => x.country_id == countryId).ToList();
                var countryName = db.countries.FirstOrDefault(x => x.id == countryId).country_name;
                ViewBag.CountryName = countryName;
                ViewBag.SelectedCountryId = countryId;
            }

            if (yearValue != null)
            {
                movielist = movielist.Where(x => x.release_date.Value.Year == yearValue).ToList();
                ViewBag.MovieYear = yearValue;
            }

            if (movieStatus != null)
            {
                movielist = movielist.Where(x => x.movie_status == movieStatus).ToList();
                ViewBag.MovieStatus = (movieStatus == true) ? "1" : "0";
                ViewBag.StatusName = (movieStatus == true) ? "Đang chiếu" : "Sắp chiếu";
            }

            if (movieSelect != null)
            {
                if (movieSelect == true)
                {
                    movielist = movielist.OrderByDescending(x => x.release_date).ToList();
                }
                else
                {
                    movielist = movielist.OrderByDescending(x => x.rating).ToList();
                }

                ViewBag.MovieSelect = (movieSelect == true) ? "1" : "0";
                ViewBag.SelectName = (movieSelect == true) ? "Mới nhất" : "Đánh giá tốt nhất";
            }

            if (!movielist.Any())
            {
                TempData["MovieNotExist"] = "Không tìm thấy phim!";
            }

            ViewBag.MovieListType = movielist;

            var movieType = db.movie_type.ToList();
            ViewBag.movieType = movieType;

            var movieCountry = db.countries.ToList();
            ViewBag.movieCountry = movieCountry;


            return View();
        }

        //[HttpGet]
        //public ActionResult GetInvoice(int movieId, int displaydateId, int scheduleId, int roomId, int[] chosenSeats)
        //{
        //    var newInvoice = new invoice();
        //    newInvoice.room_schedule_detail_id = GetRoomScheduleDetailId(roomId, displaydateId, scheduleId, movieId);
        //    newInvoice.day_create = DateTime.Now; 
        //    newInvoice.invoice_status = false;
        //    newInvoice.total_ticket = chosenSeats.Length;
        //    newInvoice.total_money = CalculateTotalMoney(chosenSeats, newInvoice.id);
        //    //db.invoices.Add(newInvoice);
        //    //db.SaveChanges();

        //    return Json(new { success = true });
        //}
        public ActionResult PaymentFail()
        {
            return View();
        }
        public ActionResult PaymentSuccess()
        {
            return View();
        }

        public ActionResult VnPayReturn()
        {
            //var response = _vnPayService.PaymentExecute(formCollection);
            //if (response == null || response.VnPayResponseCode != "00")
            //{
            //    TempData["Message"] = $"Đã xảy ra lỗi khi thanh toán : {response.VnPayResponseCode}";
            //    return RedirectToAction("BookTicket", "PaymentFail", new { area = "User" });
            //}

            //TempData["Message"] = $"Thanh toán thành công : {response.VnPayResponseCode}";

            //return RedirectToAction("BookTicket", "PaymentSuccess", new { area = "User" });
            var b = _vnPayService.CreatePaymentUrl(ControllerContext.HttpContext, db.invoices.FirstOrDefault(x => x.id == 3));
            var a = UrlPayment(ControllerContext.HttpContext, 3);
            return View();
        }

        [HttpPost]
        public ActionResult GetInvoice()
        {
            var code = new { success = false, code = -1, Url = "" };
            // Chuyển đổi JSON thành đối tượng hoặc mảng phù hợp
            string jsonData;
            using (var reader = new StreamReader(Request.InputStream))
            {
                jsonData = reader.ReadToEnd();
            }

            // Chuyển đổi chuỗi JSON thành đối tượng JObject
            JObject jsonObject = JObject.Parse(jsonData);

            //return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, 4));
            // Lấy các giá trị từ đối tượng JObject
            int movieId = (int)jsonObject["movieId"];
            int displaydateId = (int)jsonObject["displaydateId"];
            int scheduleId = (int)jsonObject["scheduleId"];
            int roomId = (int)jsonObject["roomId"];
            JArray chosenSeatsArray = (JArray)jsonObject["chosenSeats"];
            int[] chosenSeats = chosenSeatsArray.ToObject<int[]>();

            if (Session["user"] == null)
            {
                return RedirectToAction("SignIn", "Home", new { area = "" });
            }
            var currentUser = (user)Session["user"];
            var newInvoice = new invoice();
            newInvoice.user_id = currentUser.id;
            newInvoice.room_schedule_detail_id = GetRoomScheduleDetailId(roomId, displaydateId, scheduleId, movieId);
            newInvoice.day_create = DateTime.Now;
            newInvoice.invoice_status = false;
            newInvoice.total_ticket = chosenSeats.Length;

            db.invoices.Add(newInvoice);
            db.SaveChanges();
            newInvoice.total_money = CalculateTotalMoney(chosenSeats, newInvoice.id);

            code = new { success = true, code = 1, Url = "" };
            //var url = 
            db.SaveChanges();

            return Json(code);

            // Tiếp tục xử lý dữ liệu và trả về kết quả
        }
        public int GetRoomScheduleDetailId(int roomId, int displaydateId, int scheduleId, int movieId)
        {
            var roomScheduleDetail = db.room_schedule_detail
                                        .FirstOrDefault(x => x.room_id == roomId
                                                          && x.schedule_detail.movie_display_date.display_date_id == displaydateId
                                                          && x.schedule_detail.schedule_id == scheduleId
                                                          && x.schedule_detail.movie_display_date.movie_id == movieId);
            if (roomScheduleDetail != null)
            {
                return roomScheduleDetail.id;
            }
            throw new Exception("Không tìm thấy thông tin phòng và lịch chiếu tương ứng.");
        }

        // Hàm để tính tổng tiền dựa trên số lượng ghế và giá vé
        public int? CalculateTotalMoney(int[] chosenSeats, int invoiceId)
        {
            int? totalPrice = 0;
            foreach (var seatId in chosenSeats)
            {
                var newTicket = new ticket();
                newTicket.seat_id = seatId;
                newTicket.invoice_id = invoiceId;
                var seat = db.seats.FirstOrDefault(s => s.id == seatId);
                if (seat != null)
                {
                    newTicket.ticket_price = seat.price;
                    totalPrice += seat.price; 
                }

                var invoiceItem = db.invoices.FirstOrDefault(x => x.id == invoiceId);
                if (invoiceItem.invoice_status == true)
                {
                    db.tickets.Add(newTicket);
                    seat.seat_status = true;
                    db.SaveChanges();
                }
            }
            return totalPrice;
        }

        #region
        //Thanh toán vnpay
        public string UrlPayment(HttpContextBase contextBase, int invoiceId)
        {
            string paymentUrl = string.Empty;
            var invoiceDetail = db.invoices.FirstOrDefault(x => x.id == invoiceId);

            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key
            var invoicePrice = (long)invoiceDetail.total_money * 100;
            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", invoicePrice.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000

            vnpay.AddRequestData("vnp_CreateDate", invoiceDetail.day_create.Value.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            string ip = Utils.GetIpAddress(contextBase);
            vnpay.AddRequestData("vnp_IpAddr", ip);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfor", "Thanh toán hóa đơn:" + invoiceDetail.id.ToString());
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", invoiceDetail.id.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày


            //Add Params of 2.1.0 Version
            //Billing

            paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return paymentUrl;
        }
        #endregion
    }
}