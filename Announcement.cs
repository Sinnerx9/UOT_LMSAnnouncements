using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMSAnnouncements
{
    class Announcement
    {
        public int id { get; set; }
        public string title { get; set; }
        public DateTime posted_at { get; set; }
        public string user_name { get; set; }
        public string message { get; set; }
    }
}
