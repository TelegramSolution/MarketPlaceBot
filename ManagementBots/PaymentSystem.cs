using System;
using System.Collections.Generic;

namespace ManagementBots
{
    public partial class PaymentSystem
    {
        public PaymentSystem()
        {
            Invoice = new HashSet<Invoice>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool? Enable { get; set; }

        public ICollection<Invoice> Invoice { get; set; }
    }
}
