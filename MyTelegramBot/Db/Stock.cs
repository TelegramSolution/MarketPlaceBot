using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class Stock
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int? Balance { get; set; }
        public DateTime? DateAdd { get; set; }
        public string Text { get; set; }

        public Product Product { get; set; }
    }
}
