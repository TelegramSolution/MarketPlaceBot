using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class ActionHistory
    {
        public int Id { get; set; }
        public int? ActionTypeId { get; set; }
        public string Text { get; set; }
        public DateTime? TimeStamp { get; set; }

        public ActionType ActionType { get; set; }
    }
}
