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
using System.Data.Entity.Core.Objects;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Data.Entity.Infrastructure;

namespace CinemaWeb.Areas.Admin.Controllers
{
    public class ScheduleController : Controller
    {
        private Cinema_Web_Entities db = new Cinema_Web_Entities();

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
        // GET: Admin/Schedule
        public ActionResult Index()
        {
            var roomList = db.rooms.ToList();
            ViewBag.RoomList = roomList;
            List<movy> movielist = db.movies.ToList();
            GetMovieStatus(movielist);
            movielist = movielist.Where(x => x.movie_status != null).ToList();
            ViewBag.MovieList = movielist;
            var scheduleList = db.schedules.ToList();
            ViewBag.ScheduleList = scheduleList;
            return View();
        }

        [HttpPost]
        public ActionResult GetSchedule(DateTime displayDate, int roomId)
        {
            var movies = (from m in db.movies
                          join mdd in db.movie_display_date on m.id equals mdd.movie_id
                          join dd in db.display_date on mdd.display_date_id equals dd.id
                          join sd in db.schedule_detail on mdd.id equals sd.movie_display_date_id
                          join s in db.schedules on sd.schedule_id equals s.id
                          join rsd in db.room_schedule_detail on sd.id equals rsd.schedule_detail_id
                          where dd.display_date1 == displayDate &&
                            (roomId == 0 || rsd.room_id == roomId)
                          orderby s.id
                          select new
                          {
                              MovieId = m.id,
                              MovieTitle = m.title,
                              MovieImage = m.url_image,
                              Duration = m.duration_minutes,
                              ShowTimes = s.schedule_time,
                              RoomId = rsd.room_id,
                          }).ToList();

            var movieList = movies.GroupBy(m => new { m.MovieId, m.MovieTitle, m.MovieImage, m.Duration })
                  .Select(g => new
                  {
                      MovieId = g.Key.MovieId,
                      MovieTitle = g.Key.MovieTitle,
                      MovieImage = g.Key.MovieImage,
                      Duration = g.Key.Duration,
                      ShowTimes = g.Select(m => m.ShowTimes.Value.ToString(@"hh\:mm")).ToList() 
                  }).ToList();

            return Json(new {success = true, movieList});
        }

        [HttpPost]
        public ActionResult MovieByRoom(int roomId, DateTime displayDate)
        {
            var movies = (from m in db.movies
                          join mdd in db.movie_display_date on m.id equals mdd.movie_id
                          join dd in db.display_date on mdd.display_date_id equals dd.id
                          join sd in db.schedule_detail on mdd.id equals sd.movie_display_date_id
                          join s in db.schedules on sd.schedule_id equals s.id
                          join rsd in db.room_schedule_detail on sd.id equals rsd.schedule_detail_id
                          where dd.display_date1 == displayDate &&
                            (roomId == 0 || rsd.room_id == roomId)
                          orderby s.id
                          select new
                          {
                              MovieId = m.id,
                              MovieTitle = m.title,
                              MovieImage = m.url_image,
                              Duration = m.duration_minutes,
                              ShowTimes = s.schedule_time,
                              RoomId = rsd.room_id,
                          }).ToList();
            //if (roomId != 0)
            //{
            //    movies.Where(x => x.RoomId == roomId).ToList();
            //}

            var movieList = movies.GroupBy(m => new {m.MovieId, m.MovieTitle, m.MovieImage, m.Duration })
                  .Select(g => new
                  {
                      MovieId = g.Key.MovieId,
                      MovieTitle = g.Key.MovieTitle,
                      MovieImage = g.Key.MovieImage,
                      Duration = g.Key.Duration,
                      ShowTimes = g.Select(m => m.ShowTimes.Value.ToString(@"hh\:mm")).ToList() 
                  }).ToList();

            return Json(new { success = true, movieList });
        }

        [HttpPost]
        public ActionResult MovieSelected(int movieId)
        {
            var movie = db.movies
                  .Where(x => x.id == movieId)
                  .Select(x => new
                  {
                      x.title,
                      x.url_image,
                      x.release_date,
                      x.end_date
                  })
                  .FirstOrDefault();

            if (movie != null)
            {
                DateTime currentDate = DateTime.Now;
                DateTime minDate;

                if (movie.release_date < currentDate)
                {
                    minDate = currentDate;
                }
                else
                {
                    minDate = movie.release_date.Value;
                }

                var displayDate = new
                {
                    movieTitle = movie.title,
                    movieImage = movie.url_image,
                    dateFrom = minDate.ToString("yyyy-MM-dd"),
                    dateEnd = movie.end_date.Value.ToString("yyyy-MM-dd")
                };
                return Json(new { success = true, displayDate });
            }
            else
            {
                return Json(new { success = false, message = "Không tìm thấy phim" });
            }
        }

        [HttpPost]
        public ActionResult GetScheduleValid(int movieId, DateTime displayDate, int roomId)
        {
            var movie = db.movies.FirstOrDefault(x => x.id == movieId);

            int durationMinute;
            int.TryParse(movie.duration_minutes, out durationMinute);
            var schedules = (from m in db.movies
                             join mdd in db.movie_display_date on m.id equals mdd.movie_id
                             join dd in db.display_date on mdd.display_date_id equals dd.id
                             join sd in db.schedule_detail on mdd.id equals sd.movie_display_date_id
                             join s in db.schedules on sd.schedule_id equals s.id
                             join rsd in db.room_schedule_detail on sd.id equals rsd.schedule_detail_id
                             where dd.display_date1 == displayDate && rsd.room_id == roomId
                             orderby s.id
                             select new
                             {
                                 DurationMinute = m.duration_minutes,
                                 ScheduleDetailId = sd.id,
                                 StartTime = s.schedule_time,
                                 EndTime = s.schedule_time
                             }).ToList();

            var schedulesInDay = (from m in db.movies
                             join mdd in db.movie_display_date on m.id equals mdd.movie_id
                             join dd in db.display_date on mdd.display_date_id equals dd.id
                             join sd in db.schedule_detail on mdd.id equals sd.movie_display_date_id
                             join s in db.schedules on sd.schedule_id equals s.id
                             join rsd in db.room_schedule_detail on sd.id equals rsd.schedule_detail_id
                             where dd.display_date1 == displayDate
                             orderby s.id
                             select new
                             {
                                 DurationMinute = m.duration_minutes,
                                 ScheduleDetailId = sd.id,
                                 StartTime = s.schedule_time,
                                 EndTime = s.schedule_time
                             }).ToList();

            var scheduleTime = schedules.Select(x => new { x.StartTime, EndTime = x.EndTime.Value.Add(TimeSpan.FromMinutes(int.Parse(x.DurationMinute))) }).ToList();
            var duration = TimeSpan.FromMinutes(durationMinute);

            var scheduleAll = db.schedules.ToList();

            var scheduleList = scheduleAll.Select(x => new
            {
                scheduleId = x.id,
                timeFrom = x.schedule_time,
                timeEnd = x.schedule_time.Value.Add(duration)
            }).ToList();

            var scheduleValid = (from sv in scheduleList
                                 where !scheduleTime.Any(x => sv.timeEnd >= x.StartTime && sv.timeFrom <= x.EndTime)
                                 && !schedulesInDay.Any(sc => sc.StartTime == sv.timeFrom)
                                 select new
                                 {
                                     sv.timeFrom,
                                     sv.scheduleId
                                 }
                          ).ToList();

            if (scheduleValid.Any())
            {
                var allschedule = scheduleValid.Select(x => new
                {
                    ScheduleId = x.scheduleId,
                    Schedule = x.timeFrom.Value.ToString(@"hh\:mm")
                }).ToList();
                return Json(new { success = true, allschedule});
            }
            
            return Json(new { success = false, message = "Hiện không có suất chiếu trông"});
        }

        [HttpPost]
        public ActionResult AddSchedule()
        {
            string jsonData;
            using (var reader = new StreamReader(Request.InputStream))
            {
                jsonData = reader.ReadToEnd();
            }

            JObject jsonObject = JObject.Parse(jsonData);

            int movieId = (int)jsonObject["movieId"];
            DateTime displaydate = (DateTime)jsonObject["displayDate"];
            int roomId = (int)jsonObject["roomId"];
            JArray chosenSchedule = (JArray)jsonObject["scheduleDetail"];
            int[] scheduleDetail = chosenSchedule.ToObject<int[]>();
            var movieItem = db.movies.FirstOrDefault(x => x.id == movieId);
            var movieDisplayDate = db.movie_display_date.FirstOrDefault(x => x.display_date.display_date1 == displaydate && x.movie_id == movieId);
            TimeSpan bufferTime = TimeSpan.FromMinutes(30);
            TimeSpan duration = TimeSpan.FromMinutes(int.Parse(movieItem.duration_minutes));
            var existingSchedules = new List<schedule>();
            foreach (var scheduleId in scheduleDetail)
            {
                var scheduleTime = db.schedules.FirstOrDefault(x => x.id == scheduleId);
                if (scheduleTime == null)
                {
                    continue;
                }
                existingSchedules.Add(scheduleTime);
            }

            for (int i = 0; i < existingSchedules.Count; i++)
            {
                for (int j = i + 1; j < existingSchedules.Count; j++)
                {
                    var startTime1 = existingSchedules[i].schedule_time;
                    var startTime2 = existingSchedules[j].schedule_time;

                    if (startTime1.HasValue && startTime2.HasValue)
                    {
                        TimeSpan endTime1 = (startTime1.Value + duration + bufferTime).TotalHours >= 24
                                            ? (startTime1.Value + duration + bufferTime).Subtract(new TimeSpan(24, 0, 0))
                                            : startTime1.Value + duration + bufferTime;

                        if ((startTime2.Value >= startTime1.Value && startTime2.Value < endTime1) ||
                            (startTime1.Value >= startTime2.Value && startTime1.Value < (startTime2.Value + duration + bufferTime)))
                        {
                            return Json(new { success = false, message = "Vui lòng chọn các suất chiếu cách nhau " + (duration + bufferTime) + " phút!" });
                        }
                    }
                }
            }

            foreach (var scheduleId in scheduleDetail)
            {
                var scheduleTime = db.schedules.FirstOrDefault(x => x.id == scheduleId)?.schedule_time;
                if (scheduleTime == null)
                {
                    continue;
                }
                TimeSpan startTime = scheduleTime.Value;
                TimeSpan endTime = (startTime + duration + bufferTime).TotalHours >= 24
                            ? (startTime + duration + bufferTime).Subtract(new TimeSpan(24, 0, 0))
                            : startTime + duration + bufferTime;

                var _schedule = new schedule_detail
                {
                    movie_display_date_id = movieDisplayDate.id,
                    schedule_id = scheduleId,
                    start_time = startTime,
                    end_time = endTime
                };
                db.schedule_detail.Add(_schedule);
                db.SaveChanges();
                var roomscheduleDetail = new room_schedule_detail
                {
                    room_id = roomId,
                    schedule_detail_id = _schedule.id
                };
                db.room_schedule_detail.Add(roomscheduleDetail);
                db.SaveChanges();
                var seatsInRoom = db.seats.Where(s => s.room_id == roomId).ToList();
                foreach (var seat in seatsInRoom)
                {
                    var seatStatus = new seat_status
                    {
                        seat_id = seat.id,
                        room_schedule_detail_id = roomscheduleDetail.id,
                        is_booked = false,
                    };
                    db.seat_status.Add(seatStatus);
                }
                db.SaveChanges();
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult GetScheduleDetails(int movieId, DateTime displayDate, int roomId)
        {
            var movie = db.movies.FirstOrDefault(x => x.id == movieId);

            if (movie == null)
            {
                return Json(new { success = false, message = "Phim không tồn tại" });
            }

            var displayDateEntity = db.display_date.FirstOrDefault(x => x.display_date1 == displayDate);

            if (displayDateEntity == null)
            {
                return Json(new { success = false, message = "Ngày chiếu không tồn tại" });
            }

            var scheduleDetails = (from mdd in db.movie_display_date
                                   join sd in db.schedule_detail on mdd.id equals sd.movie_display_date_id
                                   join s in db.schedules on sd.schedule_id equals s.id
                                   join rsd in db.room_schedule_detail on sd.id equals rsd.schedule_detail_id
                                   join r in db.rooms on rsd.room_id equals r.id
                                   where mdd.movie_id == movieId && mdd.display_date_id == displayDateEntity.id && (roomId == 0 || r.id == roomId)
                                   orderby s.id
                                   select new
                                   {
                                       MovieTitle = movie.title,
                                       DisplayDate = displayDateEntity.display_date1,
                                       RoomName = r.room_name,
                                       ShowTime = s.schedule_time,
                                       RoomScheduleDetailId = rsd.id,
                                       ScheduleDetailId = sd.id
                                   }).ToList()
                                   .Select(x => new
                                   {
                                       x.MovieTitle,
                                       DisplayDate = x.DisplayDate.Value.ToString("dd/MM/yyyy"),
                                       x.RoomName,
                                       ShowTime = x.ShowTime.Value.ToString(@"hh\:mm"),
                                       x.RoomScheduleDetailId,
                                       x.ScheduleDetailId
                                   }).ToList();

            if (scheduleDetails.Any())
            {
                var firstDetail = scheduleDetails.First();
                return Json(new
                {
                    success = true,
                    details = new
                    {
                        MovieTitle = firstDetail.MovieTitle,
                        DisplayDate = firstDetail.DisplayDate,
                        RoomName = firstDetail.RoomName,
                        ShowTimes = scheduleDetails.Select(x => new
                        {
                            x.ShowTime,
                            x.RoomScheduleDetailId,
                            x.ScheduleDetailId
                        }).ToList(),
                    }
                });
            }

            return Json(new { success = false, message = "Không tìm thấy thông tin chi tiết suất chiếu" });
        }

        [HttpPost]
        public ActionResult GetSeatDetails(int roomScheduleDetailId)
        {
            var seatList = (from ss in db.seat_status
                            join seat in db.seats on ss.seat_id equals seat.id
                            where ss.room_schedule_detail_id == roomScheduleDetailId
                            select new
                            {
                                Seat = seat.seat_column + seat.seat_row,
                                IsBooked = ss.is_booked
                            }).ToList();

            if (seatList.Any())
            {
                return Json(new
                {
                    success = true,
                    seatList = seatList.Select(x => new
                    {
                        x.Seat,
                        x.IsBooked
                    }).ToList()
                });
            }

            return Json(new { success = false, message = "Không tìm thấy ghế cho suất chiếu này" });
        }

        [HttpPost]
        public ActionResult RemoveSchedule(int scheduleDetailId)
        {
            try
            {
                var schedule_detail = db.schedule_detail.FirstOrDefault(x => x.id == scheduleDetailId);
                if (schedule_detail == null)
                    return Json(new { success = false, message = "Không tìm thấy suất chiếu!" });

                db.schedule_detail.Remove(schedule_detail);
                db.SaveChanges();
                return Json(new { success = true, message = "Xóa suất chiếu thành công!" });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex);

                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa suất chiếu!" });
            }
        }

        [HttpGet]
        public ActionResult SearchSchedule(string query, DateTime displayDate)
        {
            var schedules = (from mdd in db.movie_display_date
                             join d in db.display_date on mdd.display_date_id equals d.id
                             join sd in db.schedule_detail on mdd.id equals sd.movie_display_date_id
                             join s in db.schedules on sd.schedule_id equals s.id
                             join rsd in db.room_schedule_detail on sd.id equals rsd.schedule_detail_id
                             join r in db.rooms on rsd.room_id equals r.id
                             join m in db.movies on mdd.movie_id equals m.id
                             where (m.title.Contains(query) || r.room_name.Contains(query)) && d.display_date1 == displayDate
                             orderby s.id
                             select new
                             {
                                 MovieId = m.id,
                                 MovieTitle = m.title,
                                 MovieImage = m.url_image,
                                 Duration = m.duration_minutes,
                                 ShowTimes = s.schedule_time,
                                 RoomId = rsd.room_id,
                             }).ToList();

            if (schedules.Any())
            {
                var movieList = schedules.GroupBy(m => new { m.MovieId, m.MovieTitle, m.MovieImage, m.Duration })
                  .Select(g => new
                  {
                      MovieId = g.Key.MovieId,
                      MovieTitle = g.Key.MovieTitle,
                      MovieImage = g.Key.MovieImage,
                      Duration = g.Key.Duration,
                      ShowTimes = g.Select(m => m.ShowTimes.Value.ToString(@"hh\:mm")).ToList()
                  }).ToList();

                return Json(new { success = true, movieList}, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Không tìm thấy suất chiếu" }, JsonRequestBehavior.AllowGet);
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
