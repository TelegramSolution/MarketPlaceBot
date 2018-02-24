using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class Currency
    {
        public Currency()
        {
            Configuration = new HashSet<Configuration>();
            ProductPrice = new HashSet<ProductPrice>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Code { get; set; }

        public ICollection<Configuration> Configuration { get; set; }
        public ICollection<ProductPrice> ProductPrice { get; set; }
    }
}
