using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class Status
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enable { get; set; }

        public ICollection<OrderStatus> OrderStatus { get; set; }
    }
}
