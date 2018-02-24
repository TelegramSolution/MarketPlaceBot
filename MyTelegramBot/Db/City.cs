using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class City
    {
        public City()
        {
            Street = new HashSet<Street>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? RegionId { get; set; }

        public Region Region { get; set; }
        public ICollection<Street> Street { get; set; }
    }
}
