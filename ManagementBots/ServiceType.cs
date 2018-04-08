using System;
using System.Collections.Generic;

namespace ManagementBots
{
    public partial class ServiceType
    {
        public ServiceType()
        {
            ServiceList = new HashSet<ServiceList>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool? Enable { get; set; }

        public ICollection<ServiceList> ServiceList { get; set; }
    }
}
