﻿using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class Payment
    {
        public int Id { get; set; }
        public string TxId { get; set; }
        public DateTime? TimestampDataAdd { get; set; }
        public string Comment { get; set; }
        public double? Summ { get; set; }
        public int? InvoiceId { get; set; }

        public DateTime? TimestampTx { get; set; }
        public Invoice Invoice { get; set; }


    }
}
