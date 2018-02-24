using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot
{
    public partial class PickupPoint
    {
        public PickupPoint()
        {
            OrderTemp = new HashSet<OrderTemp>();
            Orders = new HashSet<Orders>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enable { get; set; }

        public ICollection<OrderTemp> OrderTemp { get; set; }
        public ICollection<Orders> Orders { get; set; }
    }
}
