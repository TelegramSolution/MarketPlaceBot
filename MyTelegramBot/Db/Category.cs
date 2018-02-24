using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class Category
    {
        public Category()
        {
            Product = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        
        public bool Enable { get; set; }
        public ICollection<Product> Product { get; set; }
    }
}
