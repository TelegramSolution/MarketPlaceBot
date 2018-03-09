using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot
{
    public partial class Product
    {
        public Product()
        {
            Basket = new HashSet<Basket>();
            FeedBack = new HashSet<FeedBack>();
            OrderProduct = new HashSet<OrderProduct>();
            ProductPhoto = new HashSet<ProductPhoto>();
            ProductPrice = new HashSet<ProductPrice>();
            Stock = new HashSet<Stock>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string PhotoId { get; set; }
        public int? CategoryId { get; set; }
        public bool Enable { get; set; }
        public string TelegraphUrl { get; set; }
        public DateTime? DateAdd { get; set; }
        public string PhotoUrl { get; set; }
        public int? UnitId { get; set; }
        public string Code { get; set; }
        public int? MainPhoto { get; set; }
        public int? CurrentPriceId { get; set; }

        public Category Category { get; set; }
        public ProductPrice CurrentPrice { get; set; }
        public AttachmentFs MainPhotoNavigation { get; set; }
        public Units Unit { get; set; }
        public ICollection<Basket> Basket { get; set; }
        public ICollection<FeedBack> FeedBack { get; set; }
        public ICollection<OrderProduct> OrderProduct { get; set; }
        public ICollection<ProductPhoto> ProductPhoto { get; set; }

        public ICollection<ProductPrice> ProductPrice { get; set; }

        public ICollection<Stock> Stock { get; set; }

        public override string ToString()
        {
            try
            {
                string StockStatus = String.Empty;

                const string StockStatusOutOfStock = "Нет в наличии";

                if (Stock.Count > 0 && Stock.ElementAt(Stock.Count - 1) == null || Stock.Count > 0 && Stock.ElementAt(Stock.Count - 1) != null && Stock.ElementAt(Stock.Count - 1).Balance == 0)
                    StockStatus = StockStatusOutOfStock;

                if (Stock.Count == 0)
                    StockStatus = StockStatusOutOfStock;

                string price = CurrentPrice.ToString();

                return "Название: " + Name + BotMessage.NewLine() +
                "Цена: " + price + " / " + Unit.ShortName + BotMessage.NewLine() +
                "Описание: " + Text + BotMessage.NewLine() 
                 + StockStatus;
            }

            catch
            {
                return String.Empty;
            }
        }

        public string AdminMessage()
        {
            string MenuStatus = "Активно";
            string MainPhotoString = "";
            string CodeString = "";
            string Url = "";

            int? Balance = 0;

            if (Enable == false)

                MenuStatus = "Скрыто от пользователей";
            if (Stock.Count > 0)
                Balance = Stock.OrderByDescending(s => s.Id).FirstOrDefault().Balance;

            if (Unit == null)
                Unit = Connection.getConnection().Units.Where(u => u.Id == UnitId).FirstOrDefault();

            if (MainPhoto > 0)
                MainPhotoString = "Есть";

            if (TelegraphUrl != null)
                Url = TelegraphUrl;

            if (Code != null)
                CodeString = Code;


            else
                MainPhotoString = "Отсутствует";


            try
            {
                    return BotMessage.Bold("Название: ") + Name + BotMessage.NewLine() +
                    BotMessage.Bold("Цена: ") + CurrentPrice.ToString() + " / " + Unit.ShortName + BotMessage.NewLine() +
                    BotMessage.Bold("Категория: ") + Category.Name + BotMessage.NewLine() +
                    BotMessage.Bold("Описание: ") + Text + BotMessage.NewLine() +
                    BotMessage.Bold("В наличии: ") + Balance.ToString() + BotMessage.NewLine() +
                    BotMessage.Bold("Артикул:")+CodeString+BotMessage.NewLine()+
                    BotMessage.Bold("Ссылка на подробное описание:")+Url+BotMessage.NewLine()+
                    BotMessage.Bold("В меню: ") + MenuStatus + BotMessage.NewLine() +
                    BotMessage.Bold("Фотография:") + MainPhotoString + BotMessage.NewLine() +
                    BotMessage.Bold("Доп. фото:") + ProductPhoto.Count.ToString() + " шт.";
            }

            catch (Exception e)
            {
                return String.Empty;
            }
        }
    }
}
