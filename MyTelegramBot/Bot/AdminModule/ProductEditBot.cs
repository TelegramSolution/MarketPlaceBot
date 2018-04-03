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

namespace MyTelegramBot.Bot
{
    /// <summary>
    /// Редактирование товара
    /// </summary>
    public class ProductEditBot : BotCore
    {
        public const string ModuleName = "ProdEdit";


        private ProductFunction ProductFunction { get; set; }

        /// <summary>
        /// Id товара
        /// </summary>
        private int ProductId { get; set; }

        /// <summary>
        /// Название товара
        /// </summary>
        private string ProductName { get; set; }

        /// <summary>
        /// Изменить название товара
        /// </summary>
        public const string ProductEditNameCmd = "ProdEditName";

        /// <summary>
        /// Изменить категорию
        /// </summary>
        public const string ProductEditCategoryCmd = "ProdEditCat";

        /// <summary>
        /// Изменить стоимтость товара
        /// </summary>
        public const string ProductEditPriceCmd = "ProdEditPrice";

        /// <summary>
        /// Изменить остаток товара
        /// </summary>
        public const string ProductEditStockCmd = "ProdEditStock";

        /// <summary>
        /// Пользователь нажал на кнопку изменить товар
        /// </summary>
        public const string ProductEditorCmd = "ProdEditor";

        /// <summary>
        /// Изменить описание товара
        /// </summary>
        public const string ProductEditTextCmd = "ProdEditText";

        /// <summary>
        /// Показать / Скрыть товар от пользователей
        /// </summary>
        public const string ProductEditEnableCmd = "ProdEditEnable";

        /// <summary>
        /// Отредактировать ссылку на описание
        /// </summary>
        public const string ProductEditUrlCmd = "ProdEditUrl";

        /// <summary>
        /// Изменить фотографию
        /// </summary>
        public const string ProductEditPhotoCmd = "ProdEditPhoto";

        /// <summary>
        /// Обновить категоию в которой находится товар
        /// </summary>
        public const string ProductUpdateCategoryCmd = "ProdUpdCat";

        /// <summary>
        /// ForceReply сообщение с просьбой указать новое название для товара
        /// </summary>
        public const string ProductEditNameRelpy = "Изменить название товара:";

        /// <summary>
        /// ForceReply сообщение с просьбой указать новое описание для товара
        /// </summary>
        public const string ProductEditTextRelpy = "Изменить описание товара:";

        /// <summary>
        /// ForceReply сообщение с просьбой указать новую стоимость для товара
        /// </summary>
        public const string ProductEditPriceRelpy = "Изменить стоимость товара:";

        /// <summary>
        /// ForceReply сообщение с просьбой указать кол-во на сколько имзенить осататок. Прибавить или Убавить
        /// </summary>
        public const string ProductEditStockReply = "Изменить остаток товара:";

        /// <summary>
        /// ForceReply сообщение с просьбой указать новую ссылку на описание товара
        /// </summary>
        public const string ProductEditUrlReply = "Изменить ссылку на заметку для товара:";

        /// <summary>
        /// ForceReply сообщение с просьбой отправить новую фотографию для товара
        /// </summary>
        public const string ProductEditPhotoReply = "Изменить фотографию товара:";

        public const string ProductEditCategoryReply = "Изменить категорию для товара:";

        public const string ProductAdditionallyPhotoReply = "Добавить доп. фото:";

        /// <summary>
        /// Выбрать товар для редактирования
        /// </summary>
        public const string SelectProductCmd = "SelectProd";

        /// <summary>
        /// Показать все товары в категории
        /// </summary>
        public const string AdminProductInCategoryCmd = "AdminProdInCat";

        /// <summary>
        /// Вернуть к выбору категории из которой будем выбирать товар для редактирования
        /// </summary>
        public const string BackToAdminProductInCategoryCmd = "BackToAdminProdInCat";

        /// <summary>
        /// Панель редактирования товара
        /// </summary>
        public const string SetProductCmd = "/adminproduct";

        /// <summary>
        /// КНопка изменить ед. измерения
        /// </summary>
        public const string ProudctUnitCmd = "ProudEditUnit";


        /// <summary>
        /// добавить доп фото
        /// </summary>
        public const string InsertAdditionalPhotosCmd = "InsertAdditionalPhotos";


        /// <summary>
        /// показать доп кнопки для изменения настроек товара
        /// </summary>
        public const string MoreProdFuncCmd = "MoreProdFunc";

        public const string BackToProductEditorCmd = "BackToProductEditor";

        /// <summary>
        /// удалить доп фото
        /// </summary>
        public const string RemoveAdditionalPhotoCmd = "RemoveAdditionalPhoto";

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_update"></param>
        public ProductEditBot(Update _update) : base(_update)
        {

        }

        protected override void Initializer()
        {
            try
            {

                if (base.Argumetns != null && base.Argumetns.Count > 0)
                    ProductId = base.Argumetns[0];

                if (this.ProductId > 0)
                {
                    using (MarketBotDbContext db = new MarketBotDbContext())
                        ProductName = db.Product.Find(ProductId).Name;

                }

            }

            catch
            {

            }
        }

        public async override Task<IActionResult> Response()
        {
            if (IsOwner())
            {
                switch (base.CommandName)
                {

                    ///Назад к сообщению с описание товара и кнопками упралвния
                    case BackToProductEditorCmd:
                        return await SendProductAdminMsg(MessageId);

                    case ProductEditorCmd:
                        return await SendProductAdminMsg();

                    //Пользователь нажан кнопку Показывать/Скрывать от пользователя. 
                    //Данные обновляются в бд. Данные в сообщении обновляются (сообщение редактируется)
                    case ProductEditEnableCmd:
                        return await UpdateProductEnable();

                    ///Пользователь нажал на кнопку Изменить навзание товара. 
                    ///ПОльзователю приходи ForceReply сообщение с просьбой указать новое имя товара
                    case ProductEditNameCmd:
                        return await SendForceReplyMessage(ProductEditNameRelpy);

                    ///Пользователь нажал на кнопку Изменить стоимость товара.
                    ///ПОльзователю приходи ForceReply сообщение с просьбой указать новое значение стоимость для товара
                    case ProductEditPriceCmd:
                        return await SendForceReplyMessage(ProductEditPriceRelpy);

                    ///Пользователь нажал на кнопку Изменить описание товара. 
                    ///ПОльзователю приходи ForceReply сообщение с просьбой указать новое описание товара
                    case ProductEditTextCmd:
                        return await SendForceReplyMessage(ProductEditTextRelpy);

                    ///Пользователь нажал на кнопку Изменить остаток товара.
                    ///ПОльзователю приходи ForceReply сообщение с просьбой указать на какое значение увеличить/уменьшить кол-во товара
                    case ProductEditStockCmd:
                        return await SendForceReplyMessage(ProductEditStockReply);

                    ///Пользователь нажал на кнопку Изменить ссылку на заметку для товара. 
                    ///ПОльзователю приходи ForceReply сообщение с просьбой указать новую ссылку 
                    case ProductEditUrlCmd:
                        return await SendForceReplyMessage(ProductEditUrlReply);

                    ///Пользователь нажал на кнопку Изменить фотографию товара. 
                    ///ПОльзователю приходи ForceReply сообщение с просьбой прислать новую фотографию для товара
                    case ProductEditPhotoCmd:
                        return await SendForceReplyMessage(ProductEditPhotoReply);

                    ///Сообщение меняется на список доступных категорий
                    case ProductEditCategoryCmd:
                        return await SendCategoryList();

                    ///пользваотель выбрал новую категорию для товара. 
                    //Данные обновляются в БД. Сообщение редактируется на Описание товара и кнопки с функциями
                    case ProductUpdateCategoryCmd:
                        return await UpdateProductCategory();


                    ///ПОльзователь нажал на "Ед. изм" появтляется сообщение со списком ед. измерений
                    case ProudctUnitCmd:
                        return await SendUnitList();

                    //Сохраняем новое значение ед. измерения
                    case "UpdateProductUnit":
                        return await UpdateUnit();

                    case MoreProdFuncCmd:
                        return await SendMoreFunctionButton();

                        ///пользователь нажал удалить доп. фото
                    case RemoveAdditionalPhotoCmd:
                        return await RemoveAdditionalPhoto();

                    case InsertAdditionalPhotosCmd:
                        return await SendForceReplyMessage(ProductAdditionallyPhotoReply);

                    default:
                        break;
                }

                //Пользователь прислал новое имя для товара. данные обновляются в БД. Бот присылает сообщение с Описание товара и кнопки с функциями
                if (base.OriginalMessage.Contains(ProductEditNameRelpy))
                    return await UpdateProductName();

                //Пользователь прислал новое значение для стоимость. данные обновляются в БД. Бот присылает сообщение с Описание товара и кнопки с функциями
                if (base.OriginalMessage.Contains(ProductEditPriceRelpy))
                    return await UpdateProductPrice();

                //Пользователь прислал новое описание для товара. данные обновляются в БД. Бот присылает сообщение с Описание товара и кнопки с функциями
                if (base.OriginalMessage.Contains(ProductEditTextRelpy))
                    return await UpdateProductText();

                //Пользователь прислал насколько изменить остатки. данные обновляются в БД. Бот присылает сообщение с Описание товара и кнопки с функциями
                if (base.OriginalMessage.Contains(ProductEditStockReply))
                    return await UpdateProductStock();

                //Пользователь прислал новую ссылку для товара. данные обновляются в БД. Бот присылает сообщение с Описание товара и кнопки с функциями
                if (base.OriginalMessage.Contains(ProductEditUrlReply))
                    return await UpdateProductNoteUrl();

                //Пользователь прислал новую фотографию для товара. данные обновляются в БД. Бот присылает сообщение с Описание товара и кнопки с функциями
                if (base.OriginalMessage.Contains(ProductEditPhotoReply))
                    return await UpdateMainProductPhoto();

                if (base.OriginalMessage.Contains(ProductEditCategoryReply))
                    return await UpdateProductCategory();

                if (base.CommandName.Contains(SetProductCmd)) // админская командка для быстрого отрытия панели упраления товаро
                    return await SendProductFunc(Convert.ToInt32(base.CommandName.Substring(SetProductCmd.Length)));

                if (base.OriginalMessage.Contains(ProductAdditionallyPhotoReply))
                    return await InsertAdditionallyPhoto();

                else
                    return null;
            }

            else
                return null;
        }


        /// <summary>
        /// Показать доп кнопки редактора товара
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendMoreFunctionButton()
        {

            ProductFuncMessage ProductFuncMsg = new ProductFuncMessage(ProductId);

            await EditInlineReplyKeyboard(ProductFuncMsg.MoreBtn());

            return OkResult;
        }


        /// <summary>
        /// Удалить доп.фото у товара
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> RemoveAdditionalPhoto()
        {
            ProductId = Argumetns[0];

            int Attachid = Argumetns[1];
            
            ProductFunction = new ProductFunction();

            if (ProductFunction.RemoveAdditionalPhoto(ProductId, Attachid) > 0)
                await AnswerCallback("Фотография удалена", true);

            else
                await AnswerCallback("Фотографии не существует", true);

            ProductFunction.Dispose();

            return OkResult;
        }

        /// <summary>
        /// Сохрнаяем новоем значение Ед. измерения для товара
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateUnit()
        {
            int ProductId = Argumetns[0];
            int UnitId= Argumetns[1];

            ProductFunction = new ProductFunction();

            var product=ProductFunction.UpdateUnit(ProductId, UnitId);

            ProductFunction.Dispose();

            return await SendProductFunc(product, base.MessageId);

        }


        /// <summary>
        /// Сообщение со списоком ед. измерения
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SendUnitList()
        {
            try
            {
                BotMessage = new UnitListMessage(ProductId);
                if (await EditMessage(BotMessage.BuildMsg()) != null)
                    return OkResult;

                else
                    return OkResult;
            }

            catch
            {
                return OkResult;
            }
        }


        /// <summary>
        /// Показать все категории
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendCategoryList()
        {

            var list = CategoryFunction.GetListCategory();

            string categoryListText = String.Empty;

            foreach (Category cat in list)
                categoryListText += cat.Name + ", ";

            await SendMessage(new BotMessage { TextMessage = "Категории:" + categoryListText });

            await SendForceReplyMessage(ProductEditCategoryReply);

            return OkResult;
            
        }


        /// <summary>
        /// Отправить сообщение с товаром который выбрал Администратор
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendProductAdminMsg(int MessageId = 0)
        {
            BotMessage = new ProductFuncMessage(ProductId);

            await SendMessage(BotMessage.BuildMsg(), MessageId);

            return OkResult;

        }


        /// <summary>
        /// Обновить имя
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateProductName()
        {
            ProductFunction = new ProductFunction();

            string NewName = base.ReplyToMessageText;

            int ProductId = ProductGet(ProductEditNameRelpy);

            var Product = ProductFunction.UpdateName(ProductId, NewName);

            ProductFunction.Dispose();

            return await SendProductFunc(Product);

        }


        /// <summary>
        /// Обновить описание
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateProductText()
        {
            if (base.ReplyToMessageText.Length <= 100)
            {

                ProductFunction = new ProductFunction();

                string Text = base.ReplyToMessageText;

                int ProductId = ProductGet(ProductEditNameRelpy);

                var Product = ProductFunction.UpdateText(ProductId, Text);

                ProductFunction.Dispose();

                return await SendProductFunc(Product);

            }

            else
            {
                await SendMessage(new BotMessage { TextMessage = "Ошибка! Максимум 100 символов" });
                 return OkResult;
            }
        }


        /// <summary>
        /// Обновить ссылку на заметку
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateProductNoteUrl()
        {

            if (WebUrl != null && WebUrl.Length > 0)
            {
                ProductFunction = new ProductFunction();

                string Url = base.ReplyToMessageText;

                int ProductId = ProductGet(ProductEditUrlReply);

                var Product = ProductFunction.UpdateUrl(ProductId, Url);

                ProductFunction.Dispose();

                return await SendProductFunc(Product);
            }

            else
                return await ErrorMessage(ProductEditUrlReply, "Ошибка. Введите Url");
        }


        /// <summary>
        /// Обновить главную фотографию товара
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateMainProductPhoto()
        {

            int fs_id= await base.InsertToAttachmentFs(base.PhotoId);

            int Id = ProductGet(ProductEditPhotoReply);

            ProductFunction = new ProductFunction();

            var product= ProductFunction.UpdateMainPhoto(Id, fs_id);

            AttachmentTelegramFunction.AddAttachmentTelegram(fs_id, BotInfo.Id,base.PhotoId);

            ProductFunction.Dispose();

            return await SendProductFunc(product);
            
        }


        /// <summary>
        /// Обновить стоимость
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateProductPrice()
        {
            double NewPrice = 0.0;

            try
            {
                int ProductId = ProductGet(ProductEditPriceRelpy);

                ProductFunction = new ProductFunction();

                NewPrice = Convert.ToDouble(base.ReplyToMessageText);

                var product = ProductFunction.UpdatePrice(ProductId, NewPrice, Convert.ToInt32(BotInfo.Configuration.CurrencyId));


                ProductFunction.Dispose();

                return await SendProductFunc(product);
            }

            catch
            {
                return await ErrorMessage(ProductEditPriceRelpy);
            }

        }


        /// <summary>
        /// Добавить доп фото
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> InsertAdditionallyPhoto()
        {
            AttachmentTelegram TgAttach = null;

            int ProductId = ProductGet(ProductAdditionallyPhotoReply);

            ProductFunction = new ProductFunction();

            var ProductPhoto = ProductFunction.InsertAdditionallPhoto(ProductId, await base.GetFileAsync(base.PhotoId), base.Caption);

            if(ProductPhoto!=null && base.PhotoId!=null)
             TgAttach = AttachmentTelegramFunction.AddAttachmentTelegram(ProductPhoto.AttachmentFsId, BotInfo.Id, base.PhotoId);

            var product = ProductFunction.GetProduct(ProductId);

            ProductFunction.Dispose();

            if (ProductPhoto!=null && ProductPhoto.AttachmentFsId>0 && TgAttach!=null && TgAttach.Id > 0)
                await SendMessage(new BotMessage { TextMessage = "Добавлено" });

            return await SendProductFunc(product);
        }


        /// <summary>
        /// Обновить остатки
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateProductStock()
        {
            int Value = 0;

            int ProductId = ProductGet(ProductEditStockReply);

            ProductFunction = new ProductFunction();

            try
            {
                Value = Convert.ToInt32(base.ReplyToMessageText);

                var product = ProductFunction.UpdateStock(ProductId, Value, "Значение изменено через панель администратора");

                ProductFunction.Dispose();

                return await SendProductFunc(product);
            }

            catch
            {
                return await ErrorMessage(ProductEditStockReply,"Неверный формат данных!");
            }
        }


        /// <summary>
        /// Обновить состояние 
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateProductEnable()
        {
            int ProductId = Argumetns[0];

            int CurrentEnable = Argumetns[1];

            ProductFunction = new ProductFunction();

            if (CurrentEnable == 1)
                return await SendProductFunc(ProductFunction.UpdateEnable(ProductId, false),base.MessageId);

            else 
                return await SendProductFunc(ProductFunction.UpdateEnable(ProductId, true), base.MessageId);

        }


        /// <summary>
        /// Обновить категорию
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateProductCategory()
        {
            ProductFunction = new ProductFunction();

            int ProductId = ProductGet(ProductEditCategoryReply);

            string CatName = base.ReplyToMessageText;

            var category = CategoryFunction.GetCategory(CatName);

            if (category != null)
            {
                var product= ProductFunction.UpdateCategory(ProductId, category.Id);
                ProductFunction.Dispose();
                return await SendProductFunc(product);
            }
            else
            {
                ProductFunction.Dispose();
                await SendMessage(new BotMessage { TextMessage = "Категория не найдена!" });
                return await SendCategoryList();
            }



        }



        /// <summary>
        /// Информация о товаре
        /// </summary>
        /// <param name="ReplyName"></param>
        /// <returns></returns>
        private int ProductGet(string ReplyName)
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                ProductName = OriginalMessage.Substring(ReplyName.Length);
                var product = db.Product.Where(p => p.Name == ProductName).FirstOrDefault();
                if (product != null)
                {
                    BotMessage = new ProductFuncMessage(product.Id);

                    return product.Id;
                }

                else
                    return 0;
            }
        }


        /// <summary>
        /// Отправить сообщение с админскими функциями для товара
        /// </summary>
        /// <param name="product"></param>
        /// <param name="MessageId"></param>
        /// <returns></returns>
        private async Task<IActionResult> SendProductFunc(int productId, int MessageId=0)
        {
            if (productId >0)
            {
                BotMessage = new ProductFuncMessage(productId);
                var mess = BotMessage.BuildMsg();
                await SendMessage(mess, MessageId);

                return OkResult;

            }

            else
                return OkResult;
        }


        private async Task<IActionResult> SendProductFunc(Product product, int MessageId = 0)
        {
            if (product !=null)
            {
                BotMessage = new ProductFuncMessage(product);
                var mess = BotMessage.BuildMsg();
                await SendMessage(mess, MessageId);

                return OkResult;

            }

            else
                return OkResult;
        }


        /// <summary>
        /// Создать иОтправить ForceReply сообщение
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override async Task<IActionResult> SendForceReplyMessage(string text)
        {
            if (await base.SendForceReplyMessage(text + this.ProductName) != null)
                return base.OkResult;

            else
                return base.OkResult;
        }


        /// <summary>
        /// Сообщение с ошибкой и новое ForceReply сообщение
        /// </summary>
        /// <param name="ForceReplyText"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        private async Task<IActionResult> ErrorMessage (string ForceReplyText,string ErrorMessage = "Ошибка")
        {
            if (await SendMessage(new BotMessage { TextMessage = ErrorMessage }) != null)
                return await SendForceReplyMessage(ForceReplyText);

            else
                return OkResult;
        }
    }
}
