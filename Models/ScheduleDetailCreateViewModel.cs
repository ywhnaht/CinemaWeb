using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CinemaWeb.Models
{
    public class ScheduleDetailCreateViewModel
    {
        public int movie_display_date_id { get; set; }
        public int schedule_id { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public int room_id { get; set; }
        public int movie_id { get; set; }
        public int display_date_id { get; set; }
    }
}
