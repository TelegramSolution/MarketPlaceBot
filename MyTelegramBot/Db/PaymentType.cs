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

        public static Services.PaymentTypeEnum GetPaymentTypeEnum (int? Id)
        {
            if (Id == 1)
                return Services.PaymentTypeEnum.PaymentOnReceipt;

            if (Id == 2)
                return Services.PaymentTypeEnum.Qiwi;

            if (Id == 3)
                return Services.PaymentTypeEnum.Litecoin;

            else
                return Services.PaymentTypeEnum.PaymentOnReceipt;
        }

        public static int GetTypeId(Services.PaymentTypeEnum paymentTypeEnum)
        {
            if (paymentTypeEnum == Services.PaymentTypeEnum.PaymentOnReceipt)
                return 1;

            if (paymentTypeEnum == Services.PaymentTypeEnum.Qiwi)
                return 2;

            if (paymentTypeEnum == Services.PaymentTypeEnum.Litecoin)
                return 3;

            else
                return 1;
        }

        public static async Task<bool> CheckQiwi(string Telephone, string Toke, Services.PaymentTypeEnum typeEnum= Services.PaymentTypeEnum.Qiwi)
        {
            return await Services.Qiwi.QiwiFunction.TestConnection(Telephone, Toke);
        }
    }

}
