﻿using CinemaWeb.App_Start;
using CinemaWeb.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
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
            scheduleList = scheduleList.OrderBy(x => x.id).ToList();
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
                    Seats = x.seat_status.OrderBy(seat => seat.seat_id).Select(seat => new
                    {
                        SeatId = seat.seat_id,
                        SeatCol = seat.seat.seat_column,
                        SeatRow = seat.seat.seat_row,
                        SeatStt = seat.is_booked,   
                        SeatPrice = seat.seat.price
                    })
                    .ToList()
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

        public ActionResult PaymentFail()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("SignIn", "Home", new { area = "" });
            }
            var currentUser = (user)Session["user"];
            List<movy> movielist = db.movies.ToList();
            GetMovieStatus(movielist);
            movielist = movielist.OrderByDescending(m => m.release_date).ToList();
            ViewBag.MovieList = movielist;
            return View();
        }
        public ActionResult PaymentSuccess()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("SignIn", "Home", new { area = "" });
            }
            var currentUser = (user)Session["user"];
            List<movy> movielist = db.movies.ToList();
            GetMovieStatus(movielist);
            movielist = movielist.OrderByDescending(m => m.release_date).ToList();
            ViewBag.MovieList = movielist;
            return View();
        }

        public ActionResult VnPayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long invoiceId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                var invoiceItem = db.invoices.FirstOrDefault(x => x.id == (int)invoiceId);
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        //Thanh toan thanh cong
                        TempData["Message"] = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        invoiceItem.invoice_status = true;
                        var chosenSeat = db.seat_status.Where(x => x.room_schedule_detail_id == invoiceItem.room_schedule_detail_id && x.is_booked == true).ToList();
                        AddTicket(chosenSeat, (int)invoiceId);
                        var invoiceData = new
                        {
                            InvoiceId = invoiceItem.id,
                            UserId = invoiceItem.user_id,
                            TotalMoney = invoiceItem.total_money,
                            MovieName = invoiceItem.room_schedule_detail.schedule_detail.movie_display_date.movy.title,
                            Date = invoiceItem.room_schedule_detail.schedule_detail.movie_display_date.display_date.display_date1,
                            Schedule = invoiceItem.room_schedule_detail.schedule_detail.schedule.schedule_time,
                            RoomName = invoiceItem.room_schedule_detail.room.room_name,
                            Seat = invoiceItem.tickets.Select(t => new { t.seat.seat_row, t.seat.seat_column, t.seat.price })
                        };

                        string invoiceDataJson = JsonConvert.SerializeObject(invoiceData); 
                        CreateQrCode(invoiceDataJson, invoiceItem);
                        SendMail(invoiceItem);
                        return RedirectToAction("PaymentSuccess", "BookTicket", new { area = "User" });
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        TempData["Message"] = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        db.invoices.Remove(invoiceItem);
                        var chosenSeat = db.seat_status.Where(x => x.room_schedule_detail_id == invoiceItem.room_schedule_detail_id && x.is_booked == true).ToList();
                        chosenSeat.ForEach(x => x.is_booked = false);
                        db.SaveChanges();
                        return RedirectToAction("PaymentFail", "BookTicket", new { area = "User" });
                        //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                    }
                }
            }
            return View();
        }
        public void CreateQrCode(string invoiceDataJson, invoice invoiceItem)
        {
            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
            QRCodeData qRCodeData = QRCodeGenerator.GenerateQrCode(invoiceDataJson, QRCodeGenerator.ECCLevel.Q);
            QRCode qRCode = new QRCode(qRCodeData);

            using (MemoryStream ms = new MemoryStream())
            {
                using (Bitmap bitmap = qRCode.GetGraphic(20))
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    invoiceItem.qrcode_image = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }

            // Lấy thông tin từ qrcode
            // Giả sử qrData là chuỗi dữ liệu nhận được từ mã QR
            //string qrData = "Chuỗi_dữ_liệu_từ_mã_QR";
            //var invoiceData = JsonConvert.DeserializeObject<InvoiceData>(qrData);
            // invoiceData bây giờ là một đối tượng có các thuộc tính tương ứng với thông tin của hóa đơn

            db.SaveChanges();
        }

        public void SendMail(invoice invoiceItem)
        {
            // Gửi mail khi thanh toán thành công
            var MovieImage = invoiceItem.room_schedule_detail.schedule_detail.movie_display_date.movy.url_large_image;
            var MovieTitle = invoiceItem.room_schedule_detail.schedule_detail.movie_display_date.movy.title;
            var MovieSchedule = invoiceItem.room_schedule_detail.schedule_detail.schedule.schedule_time.Value.ToString(@"hh\:mm");
            var MovieDayOfWeek = invoiceItem.room_schedule_detail.schedule_detail.movie_display_date.display_date.display_date1.Value.DayOfWeek;
            var MovieDate = invoiceItem.room_schedule_detail.schedule_detail.movie_display_date.display_date.display_date1.Value.ToString("dd/MM/yyyy");

            var TicketSeat = "";
            foreach (var ticketItem in invoiceItem.tickets)
            {
                //if (ticketItem.seat.room_id == newInvoice.room_schedule_detail.room_id)
                {
                    TicketSeat += ticketItem.seat.seat_column + ticketItem.seat.seat_row;
                    TicketSeat += " ";
                }
            }

            var QrCode = "";
            if (invoiceItem.qrcode_image != null)
            {
                QrCode = invoiceItem.qrcode_image;
            }

            string contentInvoice = System.IO.File.ReadAllText(Server.MapPath("~/Areas/User/Common/invoice.html"));
            //contentInvoice = contentInvoice.Replace("{{MovieImage}}", MovieImage);
            contentInvoice = contentInvoice.Replace("{{MovieTitle}}", MovieTitle);
            contentInvoice = contentInvoice.Replace("{{MovieSchedule}}", MovieSchedule);
            contentInvoice = contentInvoice.Replace("{{MovieDayOfWeek}}", MovieDayOfWeek.ToString());
            contentInvoice = contentInvoice.Replace("{{MovieDate}}", MovieDate);
            contentInvoice = contentInvoice.Replace("{{TicketSeat}}", TicketSeat);
            //contentInvoice = contentInvoice.Replace("{{QrCode}}", QrCode);
            contentInvoice = contentInvoice.Replace("{{InvoiceId}}", invoiceItem.id.ToString());
            contentInvoice = contentInvoice.Replace("{{TotalMoney}}", invoiceItem.total_money.ToString());
            CinemaWeb.Areas.User.Common.Common.SendMail("Ohayou Cinema", "Chúc mừng bạn đã đặt vé thành công!", contentInvoice, invoiceItem.user.email);
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
            //newInvoice.day_create = DateTime.Now;
            newInvoice.day_create = DateTime.UtcNow;
            newInvoice.total_ticket = chosenSeats.Length;

            db.invoices.Add(newInvoice);
            db.SaveChanges();
            newInvoice.total_money = CalculateTotalMoney(chosenSeats);

            foreach (var seatId in chosenSeats)
            {
                var seatStatus = db.seat_status.FirstOrDefault(s => s.seat_id == seatId && s.room_schedule_detail_id == newInvoice.room_schedule_detail_id);
                if (seatStatus != null)
                {
                    seatStatus.is_booked = true;
                }
            }
            db.SaveChanges();

            var url = UrlPayment(newInvoice.id, currentUser.id);
            code = new { success = true, code = 1, Url = url };
            return Json(code);
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
        public int? CalculateTotalMoney(int[] chosenSeats)
        {
            int? totalPrice = 0;
            
            foreach (var seatId in chosenSeats)     
            {
                var seat = db.seats.FirstOrDefault(s => s.id == seatId);
                if (seat != null)
                {
                    totalPrice += seat.price; 
                }
            }
            return totalPrice;
        }

        public void AddTicket(List<seat_status> chosenSeats, int invoiceId)
        {
            var invoiceItem = db.invoices.FirstOrDefault(x => x.id == invoiceId);
            if (invoiceItem.invoice_status == true)
            {
                var newTicket = new ticket();
                foreach (var seatId in chosenSeats)
                {
                    var seat = db.seats.FirstOrDefault(s => s.id == seatId.seat_id);
                    if (seat != null)
                    {
                        newTicket.invoice_id = invoiceId;
                        newTicket.seat_id = seat.id;
                        newTicket.ticket_status = true;
                        newTicket.ticket_price = seat.price;
                        db.tickets.Add(newTicket);
                        db.SaveChanges();
                    }
                }
            }
        }

        #region
        //Thanh toán vnpay
        public string UrlPayment(int invoiceId, int userId)
        {
            string paymentUrl = string.Empty;
            var invoiceDetail = db.invoices.FirstOrDefault(x => x.id == invoiceId && x.user_id == userId);

            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key
            long invoicePrice = (long)invoiceDetail.total_money * 100;
            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", invoicePrice.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000

            vnpay.AddRequestData("vnp_CreateDate", invoiceDetail.day_create.Value.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            string ip = Utils.GetIpAddress();
            vnpay.AddRequestData("vnp_IpAddr", ip);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + invoiceDetail.id);
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