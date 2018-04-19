using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class PaymentSystem
    {
        public PaymentSystem()
        {
            Invoice = new HashSet<Invoice>();
            PaymentSystemConfig = new HashSet<PaymentSystemConfig>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool? Enable { get; set; }

        public ICollection<Invoice> Invoice { get; set; }
        public ICollection<PaymentSystemConfig> PaymentSystemConfig { get; set; }
    }
}
