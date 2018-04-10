using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class PaymentSystemConfig
    {
        public int Id { get; set; }
        public int PaymentSystemId { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string Login { get; set; }
        public string Pass { get; set; }
        public string Server { get; set; }

        public PaymentSystem PaymentSystem { get; set; }
    }
}
