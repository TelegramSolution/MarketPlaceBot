using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class OrderHistory
    {
        public OrderHistory()
        {
            OrdersConfirm = new HashSet<Orders>();
            OrdersDelete = new HashSet<Orders>();
            OrdersDone = new HashSet<Orders>();
        }

        public int Id { get; set; }
        public string Text { get; set; }
        public bool? Value { get; set; }
        public int? OrderId { get; set; }
        public int? FollowerId { get; set; }
        public int? ActionId { get; set; }
        public DateTime? Timestamp { get; set; }

        public OrderAction Action { get; set; }
        public Follower Follower { get; set; }
        public ICollection<Orders> OrdersConfirm { get; set; }
        public ICollection<Orders> OrdersDelete { get; set; }
        public ICollection<Orders> OrdersDone { get; set; }
    }
}
