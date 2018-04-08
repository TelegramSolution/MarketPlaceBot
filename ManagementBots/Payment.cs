using System;
using System.Collections.Generic;

namespace ManagementBots
{
    public partial class Payment
    {
        public int Id { get; set; }
        public int? InvoiceId { get; set; }
        public double? Summ { get; set; }
        public DateTime? CreateTimeStamp { get; set; }

        public Invoice Invoice { get; set; }
    }
}
