﻿using System;
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

        private InlineKeyboardCallbackButton InfoProductBtn { get; set; }

        private InlineKeyboardCallbackButton ViewBasketBtn { get; set; }

        private InlineKeyboardCallbackButton ViewAllPhotoBtn { get; set; }

        /// <summary>
        /// кнопка показать отзывы к товару
        /// </summary>
        private InlineKeyboardCallbackButton ViewFeedBackBtn { get; set; }

        private InlineKeyboardButton PhotoCatalogBtn { get; set; }

        private InlineKeyboardButton SearchProductBtn { get; set; }


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

        public ProductViewMessage (string ProductName)
        {
            try
            {
                this.ProductName = ProductName;
                using (MarketBotDbContext db = new MarketBotDbContext())
                    Product = db.Product.Where(p => p.Name == ProductName && p.Enable == true).Include(p => p.CurrentPrice).Include(p => p.Stock).FirstOrDefault();
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

                    base.MessageReplyMarkup = SetInlineKeyBoard();


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


        public InlineKeyboardMarkup SetInlineKeyBoard()
        {
            NextProductBtn = ListingProduct(GetNextProductId(Product.Id, Convert.ToInt32(Product.CategoryId)), base.Next2Emodji);

            PhotoCatalogBtn = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Фотокаталог" + base.PictureEmodji, InlineFind.PhotoCatalog + "|");

            SearchProductBtn = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Поиск" + base.SearchEmodji, InlineFind.SearchProduct + "|");

            PreviousProductBtn = ListingProduct(GetPreviousId(Product.Id, Convert.ToInt32(Product.CategoryId)), base.Previuos2Emodji);

            ReturnToCatalogListBtn = ReturnToCatalogList();

            ViewBasketBtn = base.BuildInlineBtn("Перейти в корзину", base.BuildCallData(Bot.BasketBot.ViewBasketCmd, BasketBot.ModuleName), base.BasketEmodji);

            ViewAllPhotoBtn = base.BuildInlineBtn("Все фотографии", BuildCallData("ViewAllPhotoProduct", ProductBot.ModuleName, Product.Id), base.PictureEmodji);

            ViewFeedBackBtn = BuildInlineBtn("Отзывы", BuildCallData(ProductBot.CmdViewFeedBack, ProductBot.ModuleName, Product.Id), base.StartEmodji);

            if (Product.TelegraphUrl != null && Product.TelegraphUrl.Length > 0) // Если есть ссылка на заметку то делаем кнопку "Подробнее"
                InfoProductBtn = MoreInfoProduct(Product.Id);

            if (Product.Stock.Count > 0 && Product.Stock.LastOrDefault().Balance > 0) // если есть в наличии то Добавляем кнопки +/-
            {
                AddToBasketBtn = BuildInlineBtn("+", base.BuildCallData(Bot.ProductBot.AddToBasketCmd, ProductBot.ModuleName, Product.Id));
                RemoveFromBasketBtn = BuildInlineBtn("-", base.BuildCallData(Bot.ProductBot.RemoveFromBasketCmd, ProductBot.ModuleName, Product.Id));
            }


            if (InfoProductBtn != null && AddToBasketBtn != null)
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
                            PhotoCatalogBtn, SearchProductBtn
                        },
                new[]
                        {
                            ViewAllPhotoBtn,InfoProductBtn,ViewFeedBackBtn
                        },
                
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

            if (InfoProductBtn == null && AddToBasketBtn != null)
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
                            PhotoCatalogBtn, SearchProductBtn
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
                            PhotoCatalogBtn, SearchProductBtn
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
                            PhotoCatalogBtn, SearchProductBtn
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

            else
                return null;
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
