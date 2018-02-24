using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class OrderAddress
    {
        public int AdressId { get; set; }
        public int OrderId { get; set; }

        /// <summary>
        /// Стоимость заказа
        /// </summary>
        public double ShipPriceValue { get; set; }

        public Address Adress { get; set; }
        public Orders Order { get; set; }
    }
}
