using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class OrderStatus
    {
        public OrderStatus()
        {
            Orders = new HashSet<Orders>();
        }

        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? StatusId { get; set; }
        public int? FollowerId { get; set; }
        public DateTime? Timestamp { get; set; }
        public string Text { get; set; }

        public bool Enable { get; set; }

        public Status Status { get; set; }

        public ICollection<Orders> Orders { get; set; }
    }
}
