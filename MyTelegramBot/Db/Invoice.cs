using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class Invoice
    {
        public Invoice()
        {
            Orders = new HashSet<Orders>();
            Payment = new HashSet<Payment>();
        }

        public int Id { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public TimeSpan? LifeTimeDuration { get; set; }
        public int? PaymentTypeId { get; set; }
        public string AccountNumber { get; set; }
        public string Comment { get; set; }
        public double? Value { get; set; }
        public int? InvoiceNumber { get; set; }
        public bool Paid { get; set; }

        public PaymentType PaymentType { get; set; }
        public ICollection<Orders> Orders { get; set; }
        public ICollection<Payment> Payment { get; set; }
    }
}
