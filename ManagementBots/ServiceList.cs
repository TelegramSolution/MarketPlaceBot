using System;
using System.Collections.Generic;

namespace ManagementBots
{
    public partial class ServiceList
    {
        public ServiceList()
        {
            Bot = new HashSet<Bot>();
        }

        public int Id { get; set; }
        public int? BotId { get; set; }
        public int? ServiceTypeId { get; set; }
        public int? InvoiceId { get; set; }
        public DateTime? CreateTimeStamp { get; set; }
        public DateTime? StartTimeStamp { get; set; }
        public int? DayDuration { get; set; }
        public bool? Visable { get; set; }
        public bool? IsStart { get; set; }

        public Bot BotNavigation { get; set; }
        public ServiceType ServiceType { get; set; }
        public ICollection<Bot> Bot { get; set; }
    }
}
