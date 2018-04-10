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

        public ICollection<Service> Service { get; set; }
    }
}
