using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot
{
    public class AdminKey
    {
        public AdminKey()
        {
            Admin = new HashSet<Admin>();
        }

        public int Id { get; set; }
        public string KeyValue { get; set; }
        public DateTime? DateAdd { get; set; }
        public bool? Enable { get; set; }

        public ICollection<Admin> Admin { get; set; }
    }
}
