using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class OrderAction
    {
        public OrderAction()
        {
            OrderHistory = new HashSet<OrderHistory>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool? Enable { get; set; }

        public ICollection<OrderHistory> OrderHistory { get; set; }
    }


    public enum OrderActionEnum
    {
        Confirm=1,
        Done=2,
        Delete=3,
        Recovery=4
    }
}
