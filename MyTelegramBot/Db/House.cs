using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class House
    {
        public House()
        {
            Address = new HashSet<Address>();
        }

        public int Id { get; set; }
        public string Number { get; set; }
        public int? StreetId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public int? ZipCode { get; set; }

        public string Apartment { get; set; }

        public Street Street { get; set; }
        public ICollection<Address> Address { get; set; }
    }
}
