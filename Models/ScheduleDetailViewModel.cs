using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CinemaWeb.Models
{
    public class ScheduleDetailViewModel
    {
        public int ScheduleDetailId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string MovieName { get; set; }
        public string RoomName { get; set; }
        public DateTime MovieDisplayDate { get; set; }
        public TimeSpan? ScheduleTime { get; set; }
    }
}
