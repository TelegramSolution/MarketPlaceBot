using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.InlineQueryResults;
using MyTelegramBot.Bot;
using Newtonsoft.Json;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообщение с описание товара
    /// </summary>
    public class ProductViewMessage : Bot.BotMessage
    {
        private InlineKeyboardCallbackButton AddToBasketBtn { get; set; }

        private InlineKeyboardCallbackButton RemoveFromBasketBtn { get; set; }

        private InlineKeyboardCallbackButton NextProductBtn { get; set; }

        private InlineKeyboardCallbackButton PreviousProductBtn { get; set; }

        private InlineKeyboardCallbackButton ReturnToCatalogListBtn { get; set; }

        private InlineKeyboardCallbackButton InfoProductBtn { get; set; }

        private InlineKeyboardCallbackButton ViewBasketBtn { get; set; }

        private InlineKeyboardCallbackButton ViewAllPhotoBtn { get; set; }

        /// <summary>
        /// кнопка показать отзывы к товару
        /// </summary>
        private InlineKeyboardCallbackButton ViewFeedBackBtn { get; set; }

        private int NextProductId { get; set; }

        private int PreviousProductId { get; set; }

        private int CategoryId { get; set; }

        private int ProductId { get; set; }

        private Product Product { get; set; }

        private string ProductName { get; set; }

        private int BotId { get; set; }


        public ProductViewMessage(Category category, int BotId)
        {
            if(category!=null)
                CategoryId = category.Id;

            this.BotId = BotId;
        }

        public ProductViewMessage (int ProductId, int BotId)
        {
            this.ProductId = ProductId;
            this.BotId = BotId;
        }

        public ProductViewMessage (string ProductName)
        {
            try
            {
                this.ProductName = ProductName;
                using (MarketBotDbContext db = new MarketBotDbContext())
                    Product = db.Product.Where(p => p.Name == ProductName && p.Enable == true).Include(p => p.ProductPrice).Include(p => p.Stock).FirstOrDefault();
            }

            catch
            {

            }
        }

        public override BotMessage BuildMsg()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {

                if (CategoryId > 0)
                    Product = db.Product.Where(p => p.CategoryId == CategoryId && p.Enable == true)
                        .Include(p => p.ProductPrice).Include(p=>p.ProductPhoto).Include(p => p.Stock).Include(p => p.Unit).FirstOrDefault();

                if (ProductId > 0)
                    Product = db.Product.Where(p => p.Id == ProductId && p.Enable == true)
                        .Include(p => p.ProductPrice).Include(p => p.ProductPhoto).Include(p => p.Stock).Include(p=>p.Unit).FirstOrDefault();

                if (Product != null && Product.Id > 0)
                {
                    NextProductId = GetNextProductId(Product.Id, Product.CategoryId);

                    PreviousProductId = GetPreviousId(Product.Id, Product.CategoryId);

                    Url = Product.TelegraphUrl;

                    NextProductBtn = ListingProduct(NextProductId, "\u27a1\ufe0f");

                    PreviousProductBtn = ListingProduct(PreviousProductId, "\u2b05\ufe0f");

                    ReturnToCatalogListBtn = ReturnToCatalogList();

                    ViewBasketBtn = base.BuildInlineBtn("Перейти в корзину", base.BuildCallData(Bot.BasketBot.ViewBasketCmd, BasketBot.ModuleName), base.BasketEmodji);

                    ViewAllPhotoBtn = base.BuildInlineBtn("Все фотографии", BuildCallData("ViewAllPhotoProduct", ProductBot.ModuleName, Product.Id),base.PictureEmodji);

                    ViewFeedBackBtn = BuildInlineBtn("Отзывы", BuildCallData(ProductBot.CmdViewFeedBack, ProductBot.ModuleName, Product.Id), base.StartEmodji);


                    if (Product.TelegraphUrl!=null && Product.TelegraphUrl.Length > 0) // Если есть ссылка на заметку то делаем кнопку "Подробнее"
                        InfoProductBtn = MoreInfoProduct(Product.Id);

                    if (Product.Stock.Count > 0 && Product.Stock.OrderByDescending(s => s.Id).FirstOrDefault().Balance > 0) // если есть в наличии то Добавляем кнопки +/-
                    {
                        AddToBasketBtn = AddProductToBasket(Product.Id);
                        RemoveFromBasketBtn = RemoveFromBasket(Product.Id);
                    }

               
                                       

                    base.TextMessage = Product.ToString();

                    base.CallBackTitleText = Product.Name;

                    GetMainPhoto(db, base.TextMessage);

                    SetInlineKeyBoard();


                }

                return this;
            }
        }


        private int GetNextProductId (int ProductId, int CategoryId)
        {
            int Id = 0;
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                Product product = db.Product.Where(p => p.CategoryId == CategoryId && p.Id > ProductId && p.Enable == true).FirstOrDefault();

                if (product == null)
                {
                    var pr = db.Product.Where(p => p.CategoryId == CategoryId && p.Enable == true).OrderBy(p => p.Id).FirstOrDefault();
                    Id = pr.Id;
                }
                else
                    Id = product.Id;

                return Id;
            }
        }

        private int GetPreviousId (int ProductId, int CategoryId)
        {
            int Id = 0;
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                Product product = db.Product.Where(p => p.CategoryId == CategoryId && p.Id < ProductId && p.Enable == true).OrderByDescending(p => p.Id).FirstOrDefault();

                if (product == null)
                    product = db.Product.Where(p => p.CategoryId == CategoryId && p.Enable == true && p.Id > ProductId).OrderByDescending(p => p.Id).FirstOrDefault();

                if (product != null)
                    Id = product.Id;

                else
                    Id = ProductId;

                return Id;
            }
        }

        private void SetInlineKeyBoard()
        {
            if(InfoProductBtn!=null&&AddToBasketBtn!=null)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            PreviousProductBtn,
                            ReturnToCatalogListBtn,
                            NextProductBtn
                        },
                new[]
                        {
                            ViewAllPhotoBtn,InfoProductBtn,ViewFeedBackBtn
                        }
                ,
                new[]
                        {
                            RemoveFromBasketBtn,
                            AddToBasketBtn
                        },
                new[]
                        {
                            ViewBasketBtn
                        }

                 });

            if(InfoProductBtn==null&&AddToBasketBtn!=null)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            PreviousProductBtn,
                            ReturnToCatalogListBtn,
                            NextProductBtn
                        },
                new[]
                        {
                            ViewAllPhotoBtn,ViewFeedBackBtn
                        }
                ,

                new[]
                        {
                            RemoveFromBasketBtn,
                            AddToBasketBtn
                        },
                new[]
                        {
                            ViewBasketBtn
                        }

                });

            if (InfoProductBtn != null && AddToBasketBtn == null)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            PreviousProductBtn,
                            ReturnToCatalogListBtn,
                            NextProductBtn
                        },
                new[]
                        {
                            ViewAllPhotoBtn,InfoProductBtn,ViewFeedBackBtn
                        }
                ,

                new[]
                        {
                            ViewBasketBtn
                        }

                });


            if (InfoProductBtn == null && AddToBasketBtn == null)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            PreviousProductBtn,
                            ReturnToCatalogListBtn,
                            NextProductBtn
                        },
                new[]
                        {
                            ViewAllPhotoBtn,ViewFeedBackBtn
                        }
                ,

                new[]
                        {
                            ViewBasketBtn
                        }

                });
        }

        private InlineKeyboardCallbackButton AddProductToBasket(int ProductId)
        {
            string data= base.BuildCallData(Bot.ProductBot.AddToBasketCmd,ProductBot.ModuleName ,ProductId);
            InlineKeyboardCallbackButton button = new InlineKeyboardCallbackButton("+" , data);
            return button;
        }

        private InlineKeyboardCallbackButton RemoveFromBasket (int ProductId)
        {
            string data = base.BuildCallData(Bot.ProductBot.RemoveFromBasketCmd,ProductBot.ModuleName ,ProductId);
            InlineKeyboardCallbackButton button = new InlineKeyboardCallbackButton("-", data);
            return button;
        }

        private InlineKeyboardCallbackButton ListingProduct(int ProductId, string BtnText)
        {
            string data = base.BuildCallData(Bot.ProductBot.GetProductCmd, ProductBot.ModuleName, ProductId);     
            InlineKeyboardCallbackButton button = new InlineKeyboardCallbackButton(BtnText, data);
            return button;
        }

        private InlineKeyboardCallbackButton MoreInfoProduct (int ProductId)
        {
            string data = base.BuildCallData(Bot.ProductBot.MoreInfoProductCmd, ProductBot.ModuleName,ProductId);
            InlineKeyboardCallbackButton button = new InlineKeyboardCallbackButton("Подробнее "+ base.NoteBookEmodji, data);
            return button;
        }

        private InlineKeyboardCallbackButton ReturnToCatalogList()
        {
            string data = base.BuildCallData("ReturnToCatalogList",CategoryBot.ModuleName);
            InlineKeyboardCallbackButton button = new InlineKeyboardCallbackButton("\u2934\ufe0f", data);
            return button;
        }

        private InlineKeyboardCallbackButton ViewBasket()
        {
            string data = base.BuildCallData(Bot.BasketBot.ViewBasketCmd, BasketBot.ModuleName);
            InlineKeyboardCallbackButton button = new InlineKeyboardCallbackButton("Посмотреть корзину"+ "\u2139\ufe0f", data);
            return button;
        }

        
        private void GetMainPhoto(MarketBotDbContext db,string Caption)
        {
            //Ищем фотографии для этого бота
            var photo = Product.ProductPhoto.Where(p=>p.MainPhoto).OrderByDescending(a => a.AttachmentFsId).FirstOrDefault();
            if (photo != null) 
            {
                //Берем последнюю фотографию
                var attach = db.AttachmentTelegram.Where(a => a.AttachmentFsId == photo.AttachmentFsId && a.BotInfoId==BotId).OrderByDescending(a=>a.AttachmentFsId).FirstOrDefault();

                // для бота уже загружена фотография. Вытаскиваем id файла
                if (attach != null && attach.FileId != null)
                {
                    base.MediaFile = new MediaFile
                    {
                        AttachmentFsId = Convert.ToInt32(attach.AttachmentFsId),
                        Caption = Caption,
                        FileTo = new Telegram.Bot.Types.FileToSend { FileId = attach.FileId },
                        
                        FileTypeId =Convert.ToInt32(db.AttachmentFs.Where(a => a.Id == attach.AttachmentFsId).FirstOrDefault().AttachmentTypeId)
                    };
                }

                else
                {
                    var attach_fs = db.AttachmentFs.Where(a => a.Id == photo.AttachmentFsId).OrderByDescending(a=>a.Id).FirstOrDefault();

                    ///для бота фотография не загружена. Вытаскиваем файл фотографии из бд
                    if (attach_fs != null && attach_fs.Fs.Length > 0)
                    {
                        base.MediaFile = new MediaFile
                        {
                            Caption = Caption,
                            FileTo = new Telegram.Bot.Types.FileToSend { Content = new System.IO.MemoryStream(attach_fs.Fs), Filename = "Photo.jpg" },
                            
                            AttachmentFsId=attach_fs.Id,
                            FileTypeId=Bot.Core.ConstantVariable.MediaTypeVariable.Photo
                        };
                    }

                }
            }
        }

    }
}
