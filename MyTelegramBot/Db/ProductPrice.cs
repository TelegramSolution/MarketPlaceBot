using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
namespace MyTelegramBot
{
    public partial class ProductPrice
    {
        public ProductPrice()
        {
            OrderProduct = new HashSet<OrderProduct>();
            Product = new HashSet<Product>();
        }

        public int Id { get; set; }
        public double Value { get; set; }
        public int ProductId { get; set; }
        public bool Enabled { get; set; }
        public DateTime? DateAdd { get; set; }
        public int? Volume { get; set; }
        public int? CurrencyId { get; set; }

        public Currency Currency { get; set; }
        public Product ProductNavigation { get; set; }
        public ICollection<OrderProduct> OrderProduct { get; set; }
        public ICollection<Product> Product { get; set; }

        public override string ToString()
        {
            if(Currency==null)
                using (MarketBotDbContext db=new MarketBotDbContext())
                {
                    Currency = db.Currency.Find(CurrencyId);
                }

            return Value.ToString() + " " + Currency.ShortName;
        }
    }
}
