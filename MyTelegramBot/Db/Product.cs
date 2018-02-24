using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyTelegramBot
{
    public partial class Product
    {
        public Product()
        {
            Basket = new HashSet<Basket>();
            Notification = new HashSet<Notification>();
            OrderProduct = new HashSet<OrderProduct>();
            ProductPrice = new HashSet<ProductPrice>();
            ProductPhoto = new HashSet<ProductPhoto>();
            Stock = new HashSet<Stock>();
            FeedBack = new HashSet<FeedBack>();
        }

        public int Id { get; set; }

        public string Name { get; set; }
        public string Text { get; set; }
        public string PhotoId { get; set; }
        public int CategoryId { get; set; }
        public bool Enable { get; set; }
        public string TelegraphUrl { get; set; }
        public DateTime? DateAdd { get; set; }
        public string PhotoUrl { get; set; }

        public int? UnitId { get; set; }

        public string Code { get; set; }

        public Category Category { get; set; }
        public ICollection<Basket> Basket { get; set; }
        public ICollection<Notification> Notification { get; set; }
        public ICollection<OrderProduct> OrderProduct { get; set; }
        public ICollection<ProductPrice> ProductPrice { get; set; }

        public ICollection<FeedBack> FeedBack { get; set; }

        public ICollection<ProductPhoto> ProductPhoto { get; set; }
        public ICollection<Stock> Stock { get; set; }
        public Units Unit { get; set; }

        public override string ToString()
        {
            try
            {
                string StockStatus = String.Empty;

                const string StockStatusMany = "Много";

                const string StockStatusFew = "Мало";

                const string StockStatusOutOfStock = "Нет в наличии";

                const int Many = 5;

                if (Stock.Count > 0 && Stock.ElementAt(Stock.Count - 1) != null && Stock.ElementAt(Stock.Count - 1).Balance >= Many)
                    StockStatus = StockStatusMany;

                if (Stock.Count > 0 && Stock.ElementAt(Stock.Count - 1) != null && Stock.ElementAt(Stock.Count - 1).Balance > 0 && Stock.ElementAt(Stock.Count - 1).Balance <= Many)
                    StockStatus = StockStatusFew;

                if (Stock.Count > 0 && Stock.ElementAt(Stock.Count - 1) == null || Stock.Count > 0 && Stock.ElementAt(Stock.Count - 1) != null && Stock.ElementAt(Stock.Count - 1).Balance == 0)
                    StockStatus = StockStatusOutOfStock;

                if (Stock.Count == 0)
                    StockStatus = StockStatusOutOfStock;

                var price = ProductPrice.Where(p => p.Enabled).FirstOrDefault().ToString();

                return "Название: " + Name + Bot.BotMessage.NewLine() +
                "Цена: " + ProductPrice.Where(p => p.Enabled).FirstOrDefault().ToString() + " / " + Unit.ShortName + Bot.BotMessage.NewLine() +
                "Описание: " + Text + Bot.BotMessage.NewLine() +
                "В наличии: " + StockStatus;
            }

            catch
            {
                return String.Empty;
            }
        }

        public string AdminMessage()
        {
            string MenuStatus = "Активно";

            int? Balance = 0;

            if (Enable == false)

                MenuStatus = "Скрыто от пользователей";
            if (Stock.Count > 0)
                Balance = Stock.OrderByDescending(s => s.Id).FirstOrDefault().Balance;

            if (Unit == null)
                Unit = Connection.getConnection().Units.Where(u => u.Id == UnitId).FirstOrDefault();

            try
            {
                return Bot.BotMessage.Bold("Название: ") + Name + Bot.BotMessage.NewLine() +
                Bot.BotMessage.Bold("Цена: ") + ProductPrice.Where(p => p.Enabled).OrderByDescending(o => o.Id).FirstOrDefault().ToString() + " / " + Unit.ShortName + Bot.BotMessage.NewLine() +
                Bot.BotMessage.Bold("Категория: ") + Category.Name + Bot.BotMessage.NewLine() +
                Bot.BotMessage.Bold("Описание: ") + Text + Bot.BotMessage.NewLine() +
                Bot.BotMessage.Bold("В наличии: ") + Balance.ToString() + Bot.BotMessage.NewLine() +
                Bot.BotMessage.Bold("В меню: ") + MenuStatus;
            }

            catch (Exception e)
            {
                return String.Empty;
            }
        }
    }
}
