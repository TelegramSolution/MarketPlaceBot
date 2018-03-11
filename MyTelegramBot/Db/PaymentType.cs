using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyTelegramBot
{
    public partial class PaymentType
    {
        public PaymentType()
        {
            Invoice = new HashSet<Invoice>();
            OrderTemp = new HashSet<OrderTemp>();
            PaymentTypeConfig = new HashSet<PaymentTypeConfig>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enable { get; set; }
        public string Code { get; set; }

        public ICollection<Invoice> Invoice { get; set; }
        public ICollection<OrderTemp> OrderTemp { get; set; }
        public ICollection<PaymentTypeConfig> PaymentTypeConfig { get; set; }

        public static async Task<bool> CheckQiwi(string Telephone, string Toke)
        {
            return await Services.Qiwi.QiwiFunction.TestConnection(Telephone, Toke);
        }
    }

}
