using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class ServiceType
    {
        public ServiceType()
        {
            Service = new HashSet<Service>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool? Enable { get; set; }
        public bool? IsDemo { get; set; }
        public int? MinDuration { get; set; }
        public int? MaxDuration { get; set; }
        public double? Price { get; set; }
        public string Comment { get; set; }

        public ICollection<Service> Service { get; set; }
    }
}
