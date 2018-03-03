using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.Payments;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.Bot.Core
{
    public class TelegramDebitCardInvoice
    {
        public string Title { get; set; }

        public string Desc { get; set; }

        public string PayLoad { get; set; }

        public string ProviderToken { get; set; }

        public string StartParametr { get; set; }

        public string CurrencyCode { get; set; }

        public LabeledPrice[] labeledPrice { get; set; }

        public string ProviderData { get; set; }

        private Orders Order { get; set; }

        private MarketBotDbContext db { get; set; }

        private BotInfo BotInfo { get; set; }

        private PaymentTypeConfig PaymentTypeConfig { get; set; }

        public TelegramDebitCardInvoice(Orders order, BotInfo botInfo)
        {
            this.Order = order;
            this.BotInfo = botInfo;
        }

        public TelegramDebitCardInvoice CreateInvoice()
        {
            db = new MarketBotDbContext();

            if (this.Order != null)
                this.Order.OrderProduct = db.OrderProduct.Where(p => p.OrderId == Order.Id).Include(p => p.Product).Include(p => p.Price).ToList();

            if (this.Order != null && this.Order.OrderAddress == null)
                this.Order.OrderAddress = db.OrderAddress.Where(a => a.OrderId == Order.Id).FirstOrDefault();

            if (this.Order != null && this.Order.OrderAddress == null)
                this.Order.Invoice = db.Invoice.Where(a => a.Id == Order.InvoiceId).FirstOrDefault();

            if (this.BotInfo.Configuration.Currency == null)
                this.BotInfo.Configuration.Currency = db.Currency.Find(this.BotInfo.Configuration.CurrencyId);

            this.PaymentTypeConfig = db.PaymentTypeConfig.Where(p=>p.PaymentId==this.Order.Invoice.PaymentTypeId).FirstOrDefault();

            var group = this.Order.OrderProduct.GroupBy(o => o.Product).ToList();

            if (PaymentTypeConfig!=null) {

                if (this.Order.OrderAddress == null)
                    labeledPrice = new LabeledPrice[group.Count];

                else
                {
                    labeledPrice = new LabeledPrice[group.Count+1];
                    labeledPrice[labeledPrice.Length - 1] = new LabeledPrice();
                    labeledPrice[labeledPrice.Length - 1].Amount = Convert.ToInt32(this.Order.OrderAddress.ShipPriceValue * 100);
                    labeledPrice[labeledPrice.Length - 1].Label = "Доставка";
                }      

                int counter = 0;

                foreach (var g in group)
                {

                    if (g.Key.Unit == null)
                        g.Key.Unit = db.Units.Find(g.Key.UnitId);

                    labeledPrice[counter] = new LabeledPrice();
                    labeledPrice[counter].Amount = Convert.ToInt32(this.Order.PositionPrice(g.Key.Id) * 100);
                    labeledPrice[counter].Label = g.Key.Name+ " x " + this.Order.PositionCount(g.Key.Id).ToString()+ "  " + g.Key.Unit.ShortName;
                    counter++;
                }

                this.Title = "Заказ №" + this.Order.Number.ToString();
                this.Desc = "Оплатите заказ с помощью банковской карты, через Telegram";
                this.PayLoad = this.Order.Id.ToString();
                this.StartParametr = "pay";
                this.ProviderToken = PaymentTypeConfig.Pass;
                this.CurrencyCode = this.BotInfo.Configuration.Currency.Code;

                return this;

            }

            else return null;
               
        }
    }
}
