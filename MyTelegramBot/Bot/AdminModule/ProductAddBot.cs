using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.AdminModule;
using System.IO;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Bot.AdminModule
{
    public class ProductAddBot:BotCore
    {

        public const string EnterProductNameForceReply = "Введите название нового товара";

        public const string EnterCategoryForceReply = "Введите название категории:";

        public const string EnterUnitForceReply = "Введите ед. езмерения:";

        public const string EnterPriceForceReply = "Введите цену:";

        public const string EnterDescriptionForceReply = "Введите описание:";

        public const string UploadImageForceReply = "Загрузите изборажение:";

        MarketBotDbContext db { get; set; }

        Messages.Admin.ProductFuncMessage ProductFuncMsg { get; set; }

        public ProductAddBot (Update update) : base(update)
        {

        }

        protected override void Initializer()
        {
            
        }

        public async override Task<IActionResult> Response()
        {
            if (IsOwner())
            {
                if (base.CommandName == "/addprod")
                    return await SendForceReplyMessage(EnterProductNameForceReply);

                if (base.OriginalMessage == EnterProductNameForceReply)
                    return await AddProduct();

                if (base.OriginalMessage.Contains(EnterCategoryForceReply))
                    return await UpdCategory();

                if (base.OriginalMessage.Contains(EnterUnitForceReply))
                    return await UpdUnit();

                if (base.OriginalMessage.Contains(EnterPriceForceReply))
                    return await UpdPrice();

                else
                    return null;
            }

            else
                return null;
        }

        private async Task<IActionResult> AddProduct()
        {
            db = new MarketBotDbContext();

            string product_name = ReplyToMessageText.Trim();

            var reapet = db.Product.Where(p => p.Name == product_name).FirstOrDefault();

            var categorys = CategoryList();

            if (reapet != null)
                return await SendTextMessageAndForceReply("Товар с таким именем уже существует", EnterProductNameForceReply);

            else
            {
                var product = InsertProductToDb(product_name);
               // db.Dispose();

                if (product != null)
                    return await SendTextMessageAndForceReply("Введите название новой категории или выберите уже существующую."
                        + "Список категорий:" + categorys, EnterCategoryForceReply + product.Name);

                else
                    return await SendTextMessageAndForceReply("Неизвестная ошибка", EnterProductNameForceReply);
            }
                
        }

        private Product InsertProductToDb(string name)
        {
            if (db == null)
                db = new MarketBotDbContext();

            try
            {
                Product product = new Product
                {
                    Name = name,
                    DateAdd = DateTime.Now,
                    Enable = false
                };

                db.Product.Add(product);
                db.SaveChanges();
                return product;
             
            }

            catch (Exception e)
            {
                return null;
            }
        }

        private string CategoryList()
        {
            if (db == null)
                db = new MarketBotDbContext();

            var list = db.Category.ToList();

            string res = String.Empty;

            foreach(Category cat in list)
            {
                res += cat.Name + ",";
            }

            return res;
        }

        private string UnitsList()
        {
            if (db == null)
                db = new MarketBotDbContext();

            var list = db.Units.ToList();

            string res = String.Empty;

            foreach (var u in list)
                res += u.Name + ",";

            return res;
        }

        private async Task<IActionResult> UpdCategory()
        {
            db = new MarketBotDbContext();

            string product_name = OriginalMessage.Substring(EnterCategoryForceReply.Length);

            string category_name = ReplyToMessageText;

            var category = db.Category.Where(c => c.Name == category_name).FirstOrDefault();

            var product = db.Product.Where(p => p.Name == product_name).FirstOrDefault();

            string units = UnitsList();

            if (category == null)
                category = InsertCategory(category_name);

            if (category != null)
            {
                product.CategoryId = category.Id;
                db.Update<Product>(product);
                db.SaveChanges();
                db.Dispose();
                return await SendTextMessageAndForceReply("Еденицы измерения:" + units, EnterUnitForceReply+product.Name);
            }

            else return await SendTextMessageAndForceReply(product.Name+ " /adminproduct"+product.Id+
                " Введите название новой категории или выберите уже существующую."
             + "Список категорий:" + CategoryList(), EnterCategoryForceReply + product.Name);

        }

        private Category InsertCategory(string name)
        {
            if (db == null)
                db = new MarketBotDbContext();

            Category newcat = new Category
            {
                Enable = true,
                Name = name
            };

            db.Add(newcat);
            db.SaveChanges();
            return newcat;
        }

        private async Task<IActionResult> UpdUnit()
        {
            db = new MarketBotDbContext();
            string product_name = OriginalMessage.Substring(EnterUnitForceReply.Length);

            string unit_name = ReplyToMessageText.Trim();

            var unit = db.Units.Where(u => u.Name == unit_name).FirstOrDefault();

            var product = db.Product.Where(p => p.Name == product_name).FirstOrDefault();

            var UnitList = UnitsList();

            if (unit != null && product!=null)
            {
                product.UnitId = unit.Id;
                db.Update<Product>(product);
                db.SaveChanges();
                db.Dispose();
                return await SendTextMessageAndForceReply(product.Name + " /adminproduct" + product.Id, EnterPriceForceReply + product.Name);
            }

            else
                return await SendTextMessageAndForceReply(product.Name + " /adminproduct" + product.Id+
                    " Еденицы измерения:" + UnitList, EnterUnitForceReply+product.Name);

        }

        private async Task<IActionResult> UpdPrice()
        {
            db = new MarketBotDbContext();

            string product_name = OriginalMessage.Substring(EnterPriceForceReply.Length);

            var product = db.Product.Where(p => p.Name == product_name).FirstOrDefault();      

            try
            {
                double price = Convert.ToDouble(ReplyToMessageText);
                if (price > 0)
                {
                    ProductFuncMsg = new ProductFuncMessage(product.Id);

                    ProductPrice productPrice = new ProductPrice
                    {
                        CurrencyId = BotInfo.Configuration.CurrencyId,
                        DateAdd = DateTime.Now,
                        Enabled = true,
                        Value = price,
                        ProductId = product.Id,
                        Volume = 1
                    };

                    db.ProductPrice.Add(productPrice);
                    db.SaveChanges();
                    product.CurrentPriceId = productPrice.Id;
                    db.Update<Product>(product);
                    db.SaveChanges();
                    await SendMessage(ProductFuncMsg.BuildMsg());
                    return OkResult;
                }

                else
                    return await SendTextMessageAndForceReply(product.Name + " /adminproduct" + product.Id + " Ошибка! Значение должно быть больше 0", EnterPriceForceReply + product.Name);

            }

            catch
            {
                return await SendTextMessageAndForceReply(product.Name + " /adminproduct" + product.Id + " Ошибка!", EnterPriceForceReply + product.Name);
            }
        }
    }
}
