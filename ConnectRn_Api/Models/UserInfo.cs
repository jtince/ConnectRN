using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectRn_Api.Models
{
    public class UserInfo
    {
        public int user_id { get; set; }
        public string name { get; set; }
        public DateTime date_of_birth { get; set; }
        public string day_of_week_of_birth { get; set; }
        public long created_on { get; set; }
        public DateTimeOffset? created_on_rfc { get; set; }
    }
}
