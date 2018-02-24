using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using System.Web;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.AdminModule;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Сообщение с админскими функциями Товара
    /// </summary>
    public class AdminProductFuncMessage:BotMessage
    {
        private int ProductId { get; set; }

        private InlineKeyboardCallbackButton ProductEditNameBtn { get; set; }

        private InlineKeyboardCallbackButton ProductEditCategoryBtn { get; set; }

        private InlineKeyboardCallbackButton ProductEditPriceBtn { get; set; }

        private InlineKeyboardCallbackButton ProductEditStockBtn { get; set; }

        private InlineKeyboardCallbackButton ProductEditEnableBtn { get; set; }

        private InlineKeyboardCallbackButton ProductEditTextBtn { get; set; }

        private InlineKeyboardCallbackButton ProductEditPhotoBtn { get; set; }

        private InlineKeyboardCallbackButton ProductEditUrlBtn { get; set; }

        private InlineKeyboardCallbackButton AdminPanelBtn { get; set; }

        private InlineKeyboardCallbackButton OpenProductBtn { get; set; }

        //private InlineKeyboardCallbackButton CurrencyBtn { get; set; }

        private InlineKeyboardCallbackButton UnitBtn { get; set; }

        private InlineKeyboardCallbackButton InlineImageBtn { get; set; }

        private Product Product { get; set; }

        public AdminProductFuncMessage(int ProductId)
        {
            this.ProductId = ProductId;
            
        }

        public override BotMessage BuildMsg()
        {
            int? Quantity = 0;

            using(MarketBotDbContext db=new MarketBotDbContext())
                Product=db.Product.Where(p => p.Id == ProductId).Include(p => p.Category).Include(p => p.ProductPrice).Include(p=>p.Unit).Include(p => p.Stock).FirstOrDefault();
            

            if (Product != null && Product.Stock != null && Product.Stock.Count > 0)
                Quantity = Product.Stock.OrderByDescending(s => s.Id).FirstOrDefault().Balance;

            if (Product != null)
            {
                // base.TextMessage = product.ToString()+ " - "+ Quantity.ToString()+" шт.";

              

                base.BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData(AdminBot.AdminProductInCategoryCmd, AdminBot.ModuleName, Product.CategoryId));

                ProductEditNameBtn = BuildInlineBtn("Название", BuildCallData(ProductEditBot.ProductEditNameCmd, ProductEditBot.ModuleName, ProductId),base.PenEmodji);

                ProductEditCategoryBtn = BuildInlineBtn("Категория", BuildCallData(ProductEditBot.ProductEditCategoryCmd, ProductEditBot.ModuleName, ProductId), base.PenEmodji);

                ProductEditPriceBtn = BuildInlineBtn("Стоимость", BuildCallData(ProductEditBot.ProductEditPriceCmd, ProductEditBot.ModuleName, ProductId), base.CashEmodji);

                ProductEditStockBtn = BuildInlineBtn("Остаток", BuildCallData(ProductEditBot.ProductEditStockCmd, ProductEditBot.ModuleName, ProductId),base.DepthEmodji);

                ProductEditTextBtn = BuildInlineBtn("Описание", BuildCallData(ProductEditBot.ProductEditTextCmd, ProductEditBot.ModuleName, ProductId),base.NoteBookEmodji);

                ProductEditPhotoBtn = BuildInlineBtn("Фотография", BuildCallData(ProductEditBot.ProductEditPhotoCmd, ProductEditBot.ModuleName, ProductId),base.PictureEmodji);

                ProductEditUrlBtn = BuildInlineBtn("Заметка", BuildCallData(ProductEditBot.ProductEditUrlCmd, ProductEditBot.ModuleName, ProductId),base.PenEmodji);

                AdminPanelBtn = BuildInlineBtn("Панель администратора", BuildCallData(AdminBot.BackToAdminPanelCmd,AdminBot.ModuleName),base.CogwheelEmodji);

                UnitBtn = BuildInlineBtn("Ед.изм.", BuildCallData(ProductEditBot.ProudctUnitCmd, ProductEditBot.ModuleName, ProductId),base.WeigherEmodji);

                //CurrencyBtn = new InlineKeyboardCallbackButton("Валюта", BuildCallData(ProductEditBot.ProudctCurrencyCmd, ProductEditBot.ModuleName, ProductId));

                InlineImageBtn = BuildInlineBtn("Фото в Inline", BuildCallData(ProductEditBot.ProductInlineImageCmd, ProductEditBot.ModuleName, ProductId),base.PictureEmodji);

                base.TextMessage = Product.AdminMessage();

                if (Product.Enable == true)
                    ProductEditEnableBtn = BuildInlineBtn("Активно", BuildCallData(ProductEditBot.ProductEditEnableCmd, ProductEditBot.ModuleName, ProductId),base.CheckEmodji);

                else
                    ProductEditEnableBtn = BuildInlineBtn("Активно", BuildCallData(ProductEditBot.ProductEditEnableCmd, ProductEditBot.ModuleName, ProductId),base.UnCheckEmodji);

                if (Product.Enable == true)
                    OpenProductBtn = BuildInlineBtn("Открыть", BuildCallData(ProductBot.GetProductCmd, ProductBot.ModuleName, Product.Id),base.SenderEmodji);

                       

                SetInlineKeyBoard();
            }

            return this;
        }

        private void SetInlineKeyBoard()
        {
            if(OpenProductBtn==null)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            ProductEditNameBtn, ProductEditCategoryBtn
                        },
                new[]
                        {
                            ProductEditPriceBtn, ProductEditStockBtn
                        },

                new[]
                        {
                            ProductEditTextBtn, ProductEditPhotoBtn,UnitBtn
                        },

                new[]
                        {
                            ProductEditEnableBtn, ProductEditUrlBtn,InlineImageBtn
                        },

                new[]
                         {
                            AdminPanelBtn
                         }
                ,
                new[]
                        {
                            BackBtn
                        }

                });

            else
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]   {
                            OpenProductBtn
                        },
                new[]
                        {
                            ProductEditNameBtn, ProductEditCategoryBtn
                        },
                new[]
                        {
                            ProductEditPriceBtn, ProductEditStockBtn
                        },

                new[]
                        {
                            ProductEditTextBtn, ProductEditPhotoBtn,UnitBtn
                        },

                new[]
                        {
                            ProductEditEnableBtn, ProductEditUrlBtn,InlineImageBtn
                        },

                new[]
                         {
                            AdminPanelBtn
                         }
                ,
                new[]
                        {
                            BackBtn
                        }

                });

        }
    }
}
