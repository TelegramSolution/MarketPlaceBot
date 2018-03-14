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
using MyTelegramBot.BusinessLayer;

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

        public const string EnterTextForceReply = "Введите описание товара:";

        public const string StockValueForceReply = "Введите текущее количество:";

        MarketBotDbContext db { get; set; }

        Messages.Admin.ProductFuncMessage ProductFuncMsg { get; set; }

        ProductFunction ProductFunction { get; set; }

        Product Product { get; set; }

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

                if (base.OriginalMessage.Contains(EnterTextForceReply))
                    return await UpdText();

                if (base.OriginalMessage.Contains(UploadImageForceReply))
                    return await UpdPhoto();

                if (base.OriginalMessage.Contains(StockValueForceReply))
                    return await UpdStock();

                else
                    return null;
            }

            else
                return null;
        }

        private async Task<IActionResult> AddProduct()
        {

            string product_name = ReplyToMessageText.Trim();

            ProductFunction = new ProductFunction();

            var product= ProductFunction.GetProduct(product_name);

            var categorys = CategoryList();

            bool IsProhibited = ProductFunction.NameIsProhibited(product_name);

            if (product != null)
                return await SendTextMessageAndForceReply("Товар с таким именем уже существует", EnterProductNameForceReply);

            if(IsProhibited)
                return await SendTextMessageAndForceReply("Запрещенное название!", EnterProductNameForceReply);

            else
            {
                 product = ProductFunction.InsertProduct(product_name,true);

                if (product != null && !IsProhibited)
                    return await SendTextMessageAndForceReply("Введите название новой категории или выберите уже существующую."+BotMessage.NewLine()
                        + BotMessage.Bold("Список категорий:") + categorys, EnterCategoryForceReply + product.Name);

                else
                    return await SendTextMessageAndForceReply("Неизвестная ошибка", EnterProductNameForceReply);
            }
                
        }

        private async Task<IActionResult> UpdCategory()
        {

            string product_name = OriginalMessage.Substring(EnterCategoryForceReply.Length);

            string category_name = ReplyToMessageText;

            var category = CategoryFunction.GetCategory(category_name);

            var category_list = CategoryList();

            ProductFunction = new ProductFunction();

            Product=ProductFunction.GetProduct(product_name);


            if (category == null)
            {
                category = CategoryFunction.InsertCategory(category_name);
            }

            if (category != null && Product != null)
            {
                Product = ProductFunction.GetProduct(product_name);

                ProductFunction.UpdateCategory(Product.Id, category.Id);

                ProductFunction.Dispose();

                return await SendForceReplyMessage(EnterTextForceReply + Product.Name);
            }

            else
            {
                ProductFunction.Dispose();
                return await SendTextMessageAndForceReply("Введите название новой категории или выберите уже существующую."
                + "Список категорий:" + category_list, EnterCategoryForceReply + product_name);
            }
        }

        private async Task<IActionResult> UpdUnit()
        {
            string product_name = OriginalMessage.Substring(EnterUnitForceReply.Length);

            string unit_name = ReplyToMessageText.Trim();

            var UnitList = UnitsList();

            ProductFunction = new ProductFunction();

            var product = ProductFunction.GetProduct(product_name);

            var Unit = UnitsFunction.GetUnits(unit_name);

            ProductFunction = new ProductFunction();

            if (Unit != null && product != null)
            {
                product = ProductFunction.UpdateUnit(product.Id, Unit.Id);
                ProductFunction.Dispose();
            }

            if(Unit==null && product != null) // пользваотель указа ед. измерения но ее не удалось найти. Выбираем первую из существующих
            {
               var list= UnitsFunction.UnitsList();
               await SendMessage(new BotMessage { TextMessage = "Не удалось найти еденицу измерения. Выбрано:" + list.FirstOrDefault().Name });
               product = ProductFunction.UpdateUnit(product.Id, list.FirstOrDefault().Id);
               ProductFunction.Dispose();
            }

            if (product != null && product.UnitId > 0)
                return await SendTextMessageAndForceReply(product.Name + " /adminproduct" + product.Id, EnterPriceForceReply + product.Name);
            

            else
                return await SendTextMessageAndForceReply(product.Name + " /adminproduct" + product.Id +
                    " Еденицы измерения:" + UnitList, EnterUnitForceReply + product.Name);


        }

        private async Task<IActionResult> UpdPrice()
        {

            ProductFunction = new ProductFunction();

            string product_name = OriginalMessage.Substring(EnterPriceForceReply.Length);

            var product = ProductFunction.GetProduct(product_name);

            try
            {
                double price = Convert.ToDouble(ReplyToMessageText);

                if (price > 0 && product!=null)
                {
                    product=ProductFunction.UpdatePrice(product.Id, price,Convert.ToInt32(BotInfo.Configuration.CurrencyId));
                    ProductFunction.Dispose();
                    return await SendForceReplyMessage(StockValueForceReply+product.Name);
                }

                else
                    return await SendTextMessageAndForceReply(product.Name + "Ошибка! Значение должно быть больше 0", EnterPriceForceReply + product.Name);

            }

            catch
            {
                return await SendTextMessageAndForceReply(product_name + "Ошибка! Неверный формат данных", EnterPriceForceReply + product_name);
            }
        }

        private async Task<IActionResult> UpdText()
        {
            ProductFunction = new ProductFunction();

            string product_name = OriginalMessage.Substring(EnterTextForceReply.Length);

            Product=ProductFunction.GetProduct(product_name);

            string units = UnitsList();

            if (Product != null)
            {
                Product = ProductFunction.UpdateText(Product.Id, ReplyToMessageText);
                ProductFunction.Dispose();
                return await SendTextMessageAndForceReply("Еденица измерения:" + units, EnterUnitForceReply + Product.Name);
            }

            else
            {
                ProductFunction.Dispose();
                return await SendForceReplyMessage(EnterTextForceReply + Product.Name);
            }
        }

        private async Task<IActionResult> UpdPhoto()
        {
            ProductFunction = new ProductFunction();

            string product_name = OriginalMessage.Substring(UploadImageForceReply.Length);

            Product = ProductFunction.GetProduct(product_name);

            int ProductId = -1;

            if(base.PhotoId!=null && Product != null)
            {
                int FsId= await InsertToAttachmentFs(base.PhotoId);
                Product=ProductFunction.UpdatepMainPhoto(Product.Id, FsId);
                AttachmentTelegramFunction.AddAttachmentTelegram(FsId, base.BotInfo.Id, base.PhotoId);
                ProductFunction.Dispose();
                ProductId = Product.Id;
            }

            if (Product != null) // удалось загрузить файл
            {
                ProductFuncMsg = new ProductFuncMessage(Product);
                await SendMessage(ProductFuncMsg.BuildMsg());
            }

            else
            {
                await SendMessage(new BotMessage { TextMessage = "Не удалось загрузить файл!" });
                ProductFuncMsg = new ProductFuncMessage(ProductId);
                await SendMessage(ProductFuncMsg.BuildMsg());
            }

            return OkResult;
        }

        private async Task<IActionResult> UpdStock()
        {
            ProductFunction = new ProductFunction();

            string product_name = OriginalMessage.Substring(StockValueForceReply.Length);

            var product = ProductFunction.GetProduct(product_name);

            try
            {
                int balance = Convert.ToInt32(ReplyToMessageText);

                if (balance >= 0 && product != null)
                {
                    product = ProductFunction.UpdateStock(product.Id, balance,"Добавление нового товара через диалог с ботом");
                    ProductFunction.Dispose();
                    return await SendForceReplyMessage(UploadImageForceReply + product.Name);
                }

                else
                    return await SendTextMessageAndForceReply(product.Name + " /adminproduct" + product.Id + " Ошибка! Значение должно быть больше 0", StockValueForceReply + product.Name);

            }

            catch
            {
                return await SendTextMessageAndForceReply(product_name + "Ошибка! Неверный формат данных", StockValueForceReply + product_name);
            }
        }

        private string CategoryList()
        {

            var list = CategoryFunction.GetListCategory();

            string res = String.Empty;

            foreach (Category cat in list)
            {
                res += cat.Name + ",";
            }

            return res;
        }

        private string UnitsList()
        {

            var list = UnitsFunction.UnitsList();

            string res = String.Empty;

            foreach (var u in list)
                res += u.Name + ",";

            return res;
        }
    }
}
