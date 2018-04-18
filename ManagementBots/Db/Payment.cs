using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class Payment
    {
        public int Id { get; set; }
        public int? InvoiceId { get; set; }
        public double? Summ { get; set; }
        public DateTime? CreateTimeStamp { get; set; }

        public DateTime? PaymentTimeStamp { get; set; }

        public string TxId { get; set; }

        public string SenderAccountNumber { get; set; }

        public Invoice Invoice { get; set; }
    }
}
