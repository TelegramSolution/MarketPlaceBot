using System;
using System.Collections.Generic;

namespace ManagementBots
{
    public partial class Invoice
    {
        public Invoice()
        {
            Payment = new HashSet<Payment>();
        }

        public int Id { get; set; }
        public int? PaymentSystemId { get; set; }
        public string AccountNumber { get; set; }
        public double? Summ { get; set; }
        public bool? Paid { get; set; }
        public DateTime? CreateTimeStamp { get; set; }

        public PaymentSystem PaymentSystem { get; set; }
        public ICollection<Payment> Payment { get; set; }
    }
}
