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

namespace MyTelegramBot.Bot
{
    /// <summary>
    /// Редактирование товара
    /// </summary>
    public class ProductEditBot: BotCore
    {
        public const string ModuleName = "ProdEdit";

        /// <summary>
        /// Соообщение с категориями
        /// </summary>
        private CategoryListMessage CategoryListMsg { get; set; }

        /// <summary>
        /// Сообщение с товарами в выбранной категории
        /// </summary>
        private AdminProductListMessage AdminProductListMsg { get; set; }

        /// <summary>
        /// Сообщение с админскими функциями для товара
        /// </summary>
        private ProductFuncMessage AdminProductFuncMsg { get; set; }

        private UnitListMessage UnitListMsg { get; set; }

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
        /// Кнопка изменить Валюту
        /// </summary>
        public const string ProudctCurrencyCmd = "ProdCurrency";

        /// <summary>
        /// кнопка изменить Inline фото
        /// </summary>
        public const string ProductInlineImageCmd = "ProdEditInlineImg";

        private const string InlineForceReply = "Изменить Inline фотографию:";

        /// <summary>
        /// показать доп кнопки для изменения настроек товара
        /// </summary>
        public const string MoreProdFuncCmd = "MoreProdFunc";

        public const string BackToProductEditorCmd = "BackToProductEditor";

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_update"></param>
        public ProductEditBot(Update _update) : base(_update)
        {
        
        }

        protected override void Constructor()
        {
            try
            {

                if (base.Argumetns != null && base.Argumetns.Count > 0)
                    ProductId = base.Argumetns[0];

                if (this.ProductId > 0)
                {
                    using (MarketBotDbContext db = new MarketBotDbContext())
                        ProductName = db.Product.Find(ProductId).Name;

                    UnitListMsg = new UnitListMessage(this.ProductId);


                    AdminProductListMsg = new AdminProductListMessage(this.ProductId);

                    AdminProductFuncMsg = new ProductFuncMessage(this.ProductId);
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
                    case AdminProductListMessage.NextPageCmd:
                        return await SendProductPage(Argumetns[0], Argumetns[1]);

                    case AdminProductListMessage.PreviuousPageCmd:
                        return await SendProductPage(Argumetns[0], Argumetns[1]);

                    //следеющая стр. с категориями при выборе товара для редактирования
                    case CategoryListMessage.NextPageCmd:
                        return await SendCategoryPage(Argumetns[0]);

                    // пред. стр с категориями при выборе товара для редактирования
                    case CategoryListMessage.PreviuousPageCmd:
                        return await SendCategoryPage(Argumetns[0]);

                    case ProductEditorCmd:
                        return await SendCategoryPage();

                    //Пользователь выбрал категорию.  Далее ему показываются все товары в этой категории
                    case AdminProductInCategoryCmd:
                        return await AdminProductInCategory();

                        ///Назад к сообщению с описание товара и кнопками упралвния
                    case BackToProductEditorCmd:
                        return await SendProductAdminMsg(MessageId);


                    // Пользователь выбрал товар который будет редактировать. 
                    //Присылается сообщение с описание товара и кнопками ф-ций
                    case SelectProductCmd:
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

                    ///Пользователь нажал на кнопку Изменить навзание товара.
                    ///Сообщение меняется на список доступных категорий
                    case ProductEditCategoryCmd:
                        return await SendCategoryList();

                    ///пользваотель выбрал новую категорию для товара. 
                    //Данные обновляются в БД. Сообщение редактируется на Описание товара и кнопки с функциями
                    case ProductUpdateCategoryCmd:
                        return await UpdateProductCategory();

                        //Пользователь нажана на кнопку Inline Фотография.
                    case ProductInlineImageCmd:
                        return await SendForceReplyMessage(InlineForceReply);

                        ///ПОльзователь нажал на "Ед. изм" появтляется сообщение со списком ед. измерений
                    case ProudctUnitCmd:
                        return await SendUnitList();

                     //Сохраняем новое значение ед. измерения
                    case "UpdateProductUnit":
                        return await UpdateUnit();

                    case MoreProdFuncCmd:
                        return await SendMoreFunctionButton();

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

                //пользователь прислал новую ссылку на Inline фотографию
                if (base.OriginalMessage.Contains(InlineForceReply))
                    return await UpdateInlinePhotoUrl();

                if (base.OriginalMessage.Contains(ProductEditCategoryReply))
                    return await UpdateProductCategory();

                if (base.CommandName.Contains(SetProductCmd)) // админская командка для быстрого отрытия панели упраления товаро
                    return await SendProductFunc(Convert.ToInt32(base.CommandName.Substring(SetProductCmd.Length)));


                else
                    return null;
            }

            else
                return null;
        }


        private async Task<IActionResult> SendMoreFunctionButton()
        {
            AdminProductFuncMsg = new ProductFuncMessage(Argumetns[0]);

            await EditInlineReplyKeyboard(AdminProductFuncMsg.MoreBtn());

            return OkResult;
        }

        private async Task<IActionResult> SendProductPage(int CategoryId, int PageNumber = 1)
        {
            AdminProductListMsg = new AdminProductListMessage(CategoryId, PageNumber);

            var mess = AdminProductListMsg.BuildMsg();

            if (mess != null)
                await EditMessage(mess);

            else
                await AnswerCallback("Данные отсутсвуют");

            return OkResult;
        }

        /// <summary>
        /// Пользователь хочет изменить товар. Для начала он выбирает категорию в которой этот товар находится.
        /// </summary>
        /// <param name="PageNumber">Номер страницы. Т.к товаров может быть много они бьются на страницы</param>
        /// <returns></returns>
        private async Task<IActionResult> SendCategoryPage(int PageNumber = 1)
        {
            CategoryListMsg = new CategoryListMessage(ModuleName, AdminProductInCategoryCmd,PageNumber);

            await EditMessage(CategoryListMsg.BuildCategoryAdminPage());


            return OkResult;
        }

        /// <summary>
        /// Сохрнаяем новоем значение Ед. измерения для товара
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateUnit()
        {
            try
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    var product = db.Product.Where(p => p.Id == ProductId).Include(p => p.Category).Include(p => p.CurrentPrice).Include(p => p.Unit).Include(p => p.Stock).Include(p => p.Category).FirstOrDefault();

                    product.UnitId = Argumetns[1];

                    db.SaveChanges();

                    return await SendProductFunc(product, base.MessageId);
                }
            }

            catch
            {
                return NotFoundResult;
            }
        }

        /// <summary>
        /// Сообщение со списоком ед. измерения
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SendUnitList()
        {
            try
            {
                if (UnitListMsg != null &&await EditMessage(UnitListMsg.BuildMsg()) != null)
                    return OkResult;

                else
                    return NotFoundResult;
            }

            catch
            {
                return NotFoundResult;
            }
        }

        private async Task<IActionResult> UpdateInlinePhotoUrl()
        {
            try
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    var product = db.Product.Where(p => p.Id == ProductGet(InlineForceReply)).Include(p => p.Category).Include(p => p.CurrentPrice).Include(p => p.Unit).Include(p => p.Stock).Include(p=>p.Category).FirstOrDefault();

                    string Url = base.ReplyToMessageText;

                    if (product != null)
                    {
                        product.PhotoUrl = Url;
                        db.SaveChanges();
                    }

                    return await SendProductFunc(product);
                }
                    
            }

            catch
            {
                return  NotFoundResult;
            }
        }


        /// <summary>
        /// Показать все категории
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendCategoryList()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                var list = db.Category.ToList();

                string categoryListText = String.Empty;

                foreach (Category cat in list)
                    categoryListText += cat.Name+", ";

                await SendMessage(new BotMessage { TextMessage ="Категории:"+ categoryListText });

                await SendForceReplyMessage(ProductEditCategoryReply);

                return OkResult;
            }
        }

        /// <summary>
        /// Все товары в категории
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> AdminProductInCategory()
        {
            AdminProductListMsg = new AdminProductListMessage(base.Argumetns[0]);

            var mess= AdminProductListMsg.BuildMsg();

            if (mess != null)
                await EditMessage(mess);

            else
                await AnswerCallback("Данные отсутствуют");

            return OkResult;
        }

        /// <summary>
        /// Отправить сообщение с товаром который выбрал Администратор
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendProductAdminMsg(int MessageId=0)
        {

            if (await SendMessage(AdminProductFuncMsg.BuildMsg(), MessageId) != null)
                return base.OkResult;

            if (MessageId > 0 && await EditMessage(AdminProductFuncMsg.BuildMsg()) != null)
                return OkResult;

            else
                return base.NotFoundResult;
        }

        /// <summary>
        /// Обновить имя
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateProductName()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var product = db.Product.Where(p => p.Id == ProductGet(ProductEditNameRelpy)).Include(p => p.Category).Include(p => p.CurrentPrice).Include(p => p.Unit).Include(p => p.Stock).Include(p => p.Category).FirstOrDefault();

                string NewName = base.ReplyToMessageText;

                product.Name = NewName;

                db.SaveChanges();

                return await SendProductFunc(product);
            }
        }

        /// <summary>
        /// Обновить описание
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateProductText()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var product = db.Product.Where(p => p.Id == ProductGet(ProductEditNameRelpy)).Include(p => p.Category).Include(p => p.CurrentPrice).Include(p => p.Unit).Include(p => p.Stock).Include(p => p.Category).FirstOrDefault();

                string NewText = base.ReplyToMessageText;

                product.Text = NewText;

                db.SaveChanges();

                return await SendProductFunc(product);
            }
        }

        /// <summary>
        /// Обновить ссылку на заметку
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateProductNoteUrl()
        {

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                Product product = new Product();
                if (WebUrl != null && WebUrl.Length > 0)
                {
                     product =db.Product.Where(p=>p.Id==ProductGet(ProductEditUrlReply)).Include(p => p.Category).Include(p => p.CurrentPrice).Include(p => p.Unit).Include(p => p.Stock).Include(p => p.Category).FirstOrDefault();

                    if (product != null)
                    {
                        string NewUrl = base.WebUrl;

                        product.TelegraphUrl = NewUrl;

                        db.SaveChanges();

                        return await SendProductFunc(product);
                    }

                    else return base.NotFoundResult;
                }

                else
                    return await ErrorMessage(ProductEditUrlReply,"Ошибка. Введите Url");
            }
        }


        /// <summary>
        /// Обновить главную фотографию товара
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateMainProductPhoto()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var product = db.Product.Where(p => p.Id == ProductGet(ProductEditPhotoReply)).Include(p => p.Category).Include(p => p.CurrentPrice).Include(p => p.Unit).Include(p => p.Stock).Include(p => p.Category).FirstOrDefault();
                string NewPhoto = base.PhotoId;

                int fs_id= await base.InsertToAttachmentFs(base.PhotoId);

                AttachmentTelegram attachment = new AttachmentTelegram
                {
                    AttachmentFsId = fs_id,
                    FileId = base.PhotoId,
                    BotInfoId = BotInfo.Id,

                };

                product.MainPhoto = fs_id;
                db.Update<Product>(product);
                db.AttachmentTelegram.Add(attachment);
                db.SaveChanges();

                return await SendProductFunc(product);
            }
        }

        /// <summary>
        /// Обновить стоимость
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateProductPrice()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                double NewPrice = 0.0;
                try
                {
                    var product = db.Product.Where(p => p.Id == ProductGet(ProductEditPriceRelpy)).Include(p=>p.CurrentPrice).Include(p => p.Category).FirstOrDefault();
                    NewPrice = Convert.ToDouble(base.ReplyToMessageText);

                    ProductPrice productPrice = new ProductPrice
                    {
                        ProductId = product.Id,
                        DateAdd = DateTime.Now,
                        Enabled = true,
                        Value = NewPrice
                    };

                    var OldPrice = product.CurrentPrice;

                    if (OldPrice != null)
                        OldPrice.Enabled = false;

                    db.ProductPrice.Add(productPrice);
                    db.SaveChanges();
                    return await SendProductFunc(product);
                }

                catch
                {
                    return await ErrorMessage(ProductEditPriceRelpy);
                }
            }

        }

        /// <summary>
        /// Обновить остатки
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateProductStock()
        {
            int NewStock = 0;
            int? OldBalance = 0;

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var product = db.Product.Where(p => p.Id == ProductGet(ProductEditStockReply)).Include(p => p.Category).Include(p => p.CurrentPrice).Include(p => p.Unit).Include(p => p.Stock).Include(p => p.Category).FirstOrDefault();

                try
                {
                    NewStock = Convert.ToInt32(base.ReplyToMessageText);

                    if (product.Stock.Count > 0)
                        OldBalance = db.Stock.Where(s => s.ProductId == product.Id).OrderByDescending(s => s.Id).FirstOrDefault().Balance;

                    if (OldBalance + NewStock < 0) // Если в наличии 50, а отнимают 100. Новый баланс должен быть равен 0
                        NewStock =-1* Convert.ToInt32(OldBalance);

                    Stock stock = new Stock
                    {
                        ProductId = product.Id,
                        DateAdd = DateTime.Now,
                        Quantity = NewStock,
                        Balance = OldBalance + NewStock,
                        Text="Добавлено через панель администратора"
                    };

                    db.Stock.Add(stock);
                    db.SaveChanges();
                    product.Stock.Add(stock);
                    return await SendProductFunc(product);
                }

                catch
                {
                    return await ErrorMessage(ProductEditStockReply);
                }
            }

        }

        /// <summary>
        /// Обновить состояние 
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateProductEnable()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var product =db.Product.Where(p => p.Id == ProductId).Include(p => p.Category).Include(p => p.CurrentPrice).Include(p => p.Unit).Include(p => p.Stock).Include(p => p.Category).FirstOrDefault();

                if (product != null && product.Enable == true || product!=null && product.Enable==false)
                {
                    product.Enable = true;
                    db.SaveChanges();
                    return await SendProductFunc(product, base.MessageId);
                }

                if (product != null && product.Enable == false || product != null && product.Enable == true)
                {
                    product.Enable = false;
                    db.SaveChanges();
                    return await SendProductFunc(product, base.MessageId);
                }

                else
                    return await SendProductFunc(product, base.MessageId);
            }
        }

        /// <summary>
        /// Обновить категорию
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateProductCategory()
        {

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var product = db.Product.Where(p => p.Id == ProductGet(ProductEditCategoryReply)).Include(p => p.Category).Include(p => p.CurrentPrice).Include(p => p.Unit).Include(p => p.Stock).Include(p => p.Category).FirstOrDefault();

                string CatName = base.ReplyToMessageText;

                var cat = db.Category.Where(c => c.Name == CatName).FirstOrDefault();

                if (cat != null)
                {
                    product.CategoryId = cat.Id;
                    db.Update(product);
                    db.SaveChanges();
                    return await SendProductFunc(product);
                }

                else
                {
                    await SendMessage(new BotMessage { TextMessage = "Категория не найдена!" });
                    return await SendCategoryList();
                }
            }

        }


        private int ToInt(string value)
        {
            try
            {
                return Convert.ToInt32(value);
            }

            catch
            {
                return -1;
            }
        }

        private double ToDouble(string value)
        {
            try
            {
                return Convert.ToDouble(value);
            }

            catch
            {
                return -1;
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
                    AdminProductFuncMsg = new ProductFuncMessage(product.Id);

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
                AdminProductFuncMsg = new ProductFuncMessage(productId);
                var mess = AdminProductFuncMsg.BuildMsg();
                await SendMessage(mess, MessageId);

                return OkResult;

            }

            else
                return NotFoundResult;
        }

        private async Task<IActionResult> SendProductFunc(Product product, int MessageId = 0)
        {
            if (product !=null)
            {
                AdminProductFuncMsg = new ProductFuncMessage(product);
                var mess = AdminProductFuncMsg.BuildMsg();
                await SendMessage(mess, MessageId);

                return OkResult;

            }

            else
                return NotFoundResult;
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
                return base.NotFoundResult;
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
                return NotFoundResult;
        }
    }
}
