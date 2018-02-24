using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class PaymentTypeConfig
    {
        public int Id { get; set; }
        public int? PaymentId { get; set; }
        public bool Enable { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string Login { get; set; }
        public string Pass { get; set; }
        public DateTime? TimeStamp { get; set; }

        public PaymentType Payment { get; set; }
    }
}
