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
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообщение с описание товара
    /// </summary>
    public class ProductViewMessage : BotMessage
    {
        private InlineKeyboardCallbackButton AddToBasketBtn { get; set; }

        private InlineKeyboardCallbackButton RemoveFromBasketBtn { get; set; }

        private InlineKeyboardCallbackButton NextProductBtn { get; set; }

        private InlineKeyboardCallbackButton PreviousProductBtn { get; set; }

        private InlineKeyboardCallbackButton ReturnToCatalogListBtn { get; set; }

        private InlineKeyboardCallbackButton UrlProductBtn { get; set; }

        private InlineKeyboardCallbackButton ViewBasketBtn { get; set; }

        private InlineKeyboardCallbackButton ViewAllPhotoBtn { get; set; }

        /// <summary>
        /// кнопка показать отзывы к товару
        /// </summary>
        private InlineKeyboardCallbackButton ViewFeedBackBtn { get; set; }

        private InlineKeyboardButton PhotoCatalogBtn { get; set; }

        private InlineKeyboardButton SearchProductBtn { get; set; }

        /// <summary>
        /// Кнопка для отображения доп. кнопок (Отзыв, Фотографии, Подробное описание, Назад)
        /// </summary>
        private InlineKeyboardCallbackButton Page2Btn { get; set; }

        /// <summary>
        /// вернуться назад к основным кнопкам
        /// </summary>
        private InlineKeyboardCallbackButton BackToMainPageBtn { get; set; }

        private InlineKeyboardCallbackButton QuestionBtn { get; set; }

        private int CategoryId { get; set; }

        private int ProductId { get; set; }

        private Product Product { get; set; }

        private string ProductName { get; set; }

        private int BotId { get; set; }


        public ProductViewMessage (Product product)
        {
            Product = product;
        }

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


        public override BotMessage BuildMsg()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {

                if (CategoryId > 0)
                    Product = db.Product.Where(p => p.CategoryId == CategoryId && p.Enable == true)
                        .Include(p => p.CurrentPrice).Include(p=>p.ProductPhoto).Include(p => p.Stock).Include(p => p.Unit).FirstOrDefault();

                if (ProductId > 0)
                    Product = db.Product.Where(p => p.Id == ProductId && p.Enable == true)
                        .Include(p => p.CurrentPrice).Include(p => p.ProductPhoto).Include(p => p.Stock).Include(p=>p.Unit).FirstOrDefault();

                if (Product != null && Product.Id > 0)
                {

                    Url = Product.TelegraphUrl;                                     

                    base.TextMessage = Product.ToString();

                    base.CallBackTitleText = Product.Name;

                    GetMainPhoto(db, base.TextMessage);

                    base.MessageReplyMarkup = MainPageButtons();


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


        public InlineKeyboardMarkup MainPageButtons()
        {
            NextProductBtn = ListingProduct(GetNextProductId(Product.Id, Convert.ToInt32(Product.CategoryId)), base.NextEmodji);

            PhotoCatalogBtn = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(base.PictureEmodji, InlineFind.PhotoCatalog + "|");

            SearchProductBtn = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(base.SearchEmodji, InlineFind.SearchProduct + "|");

            PreviousProductBtn = ListingProduct(GetPreviousId(Product.Id, Convert.ToInt32(Product.CategoryId)), base.PreviuosEmodji);

            ReturnToCatalogListBtn = BuildInlineBtn("\u2934\ufe0f", BuildCallData("ReturnToCatalogList", CategoryBot.ModuleName));

            ViewBasketBtn = base.BuildInlineBtn(base.BasketEmodji, base.BuildCallData(BasketBot.ViewBasketCmd, BasketBot.ModuleName));

            QuestionBtn= base.BuildInlineBtn(base.QuestionMarkEmodji, base.BuildCallData(ProductBot.ProductQuestionCmd, ProductBot.ModuleName,Product.Id));

            Page2Btn = BuildInlineBtn(base.Next2Emodji, BuildCallData(ProductBot.CmdPage2Buttons, ProductBot.ModuleName, Product.Id));


            if (Product.Stock.Count > 0 && Product.Stock.LastOrDefault().Balance > 0) // если есть в наличии то Добавляем кнопки +/-
            {
                AddToBasketBtn = BuildInlineBtn(base.Plus, base.BuildCallData(ProductBot.AddToBasketCmd, ProductBot.ModuleName, Product.Id));
                RemoveFromBasketBtn = BuildInlineBtn(base.Minus, base.BuildCallData(ProductBot.RemoveFromBasketCmd, ProductBot.ModuleName, Product.Id));
            }


            if (AddToBasketBtn != null)
                return new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            PreviousProductBtn,
                            ReturnToCatalogListBtn,
                            NextProductBtn
                        },
                new[]
                        {
                            RemoveFromBasketBtn, AddToBasketBtn

                        },
                new[]
                        {
                            PhotoCatalogBtn, SearchProductBtn,QuestionBtn,ViewBasketBtn,Page2Btn
                        },
                


                 });



            if (AddToBasketBtn == null)
                return new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            PreviousProductBtn,
                            ReturnToCatalogListBtn,
                            NextProductBtn
                        },
                new[]
                        {
                            PhotoCatalogBtn, SearchProductBtn,QuestionBtn,ViewBasketBtn,Page2Btn
                        },


                });

            else
                return null;
        }

        public InlineKeyboardMarkup SecondPageButtons()
        {
            if (Product == null && ProductId > 0)
                Product = BusinessLayer.ProductFunction.GetProductById(ProductId);

            ViewAllPhotoBtn = base.BuildInlineBtn("Все фотографии", BuildCallData(ProductBot.ViewAllPhotoProductCmd, ProductBot.ModuleName, Product.Id), base.PictureEmodji);

            ViewFeedBackBtn = base.BuildInlineBtn("Отзывы", BuildCallData(ProductBot.CmdViewFeedBack, ProductBot.ModuleName, Product.Id), base.StartEmodji);

            BackToMainPageBtn = base.BuildInlineBtn(base.Previuos2Emodji, BuildCallData(ProductBot.CmdBackToMainPageButtons, ProductBot.ModuleName, Product.Id));

            if (Product.TelegraphUrl != null && Product.TelegraphUrl.Length > 0) // Если есть ссылка на заметку то делаем кнопку "Подробнее"
                UrlProductBtn = BuildInlineBtn("Подробное описание", BuildCallData(ProductBot.MoreInfoProductCmd, ProductBot.ModuleName, Product.Id));

            if(UrlProductBtn!=null)
                return new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                           ViewAllPhotoBtn,ViewFeedBackBtn
                        },
                new[]
                        {
                            BackToMainPageBtn,UrlProductBtn
                        },


                });

            else
                return new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                           ViewAllPhotoBtn,ViewFeedBackBtn
                        },
                new[]
                        {
                            BackToMainPageBtn
                        },


                });
        }

        private InlineKeyboardCallbackButton ListingProduct(int ProductId, string BtnText)
        {
            string data = base.BuildCallData(Bot.ProductBot.GetProductCmd, ProductBot.ModuleName, ProductId);     
            InlineKeyboardCallbackButton button = new InlineKeyboardCallbackButton(BtnText, data);
            return button;
        }


        
        private void GetMainPhoto(MarketBotDbContext db,string Caption)
        {
            //Ищем фотографии для этого бота
            var photo = db.AttachmentFs.Find(Convert.ToInt32(this.Product.MainPhoto));

            if (photo != null) 
            {
                //Берем последнюю фотографию
                var attach = db.AttachmentTelegram.Where(a => a.AttachmentFsId == photo.Id && a.BotInfoId==BotId).OrderByDescending(a=>a.AttachmentFsId).FirstOrDefault();

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
                    ///для бота фотография не загружена. Вытаскиваем файл фотографии из бд
                    if (photo != null && photo.Fs.Length > 0)
                    {
                        base.MediaFile = new MediaFile
                        {
                            Caption = Caption,
                            FileTo = new Telegram.Bot.Types.FileToSend { Content = new System.IO.MemoryStream(photo.Fs), Filename = "Photo.jpg" },
                            
                            AttachmentFsId= photo.Id,
                            FileTypeId=Bot.Core.ConstantVariable.MediaTypeVariable.Photo
                        };
                    }

                }
            }
        }

    }
}
