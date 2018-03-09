﻿using System;
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
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Сообщение с админскими функциями Товара
    /// </summary>
    public class ProductFuncMessage:BotMessage
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

        private InlineKeyboardCallbackButton MoreSettingsBtn { get; set; }

        private InlineKeyboardCallbackButton UnitBtn { get; set; }

        private InlineKeyboardButton ViewAdditionalPhotosBtn { get; set; }

        private InlineKeyboardCallbackButton InsertAdditionalPhotosBtn { get; set; }

        private InlineKeyboardButton EditorProductBtn { get; set; }

        private Product Product { get; set; }

        public ProductFuncMessage(int ProductId)
        {
            this.ProductId = ProductId;
            
        }

        public ProductFuncMessage (Product product)
        {
            this.Product = product;
        }

        public override BotMessage BuildMsg()
        {
            int? Quantity = 0;

            if (Product == null)
                Product = BusinessLayer.ProductFunction.GetProductById(ProductId);
            

            if (Product != null && Product.Stock != null && Product.Stock.Count > 0)
                Quantity = Product.Stock.OrderByDescending(s => s.Id).FirstOrDefault().Balance;

            if (Product != null)
            {
                // base.TextMessage = product.ToString()+ " - "+ Quantity.ToString()+" шт.";

                AdminPanelBtn = BackToAdminPanelBtn();

                EditorProductBtn = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Редактор"+base.SearchEmodji, InlineFind.EditProduct + "|");

                ProductEditNameBtn = BuildInlineBtn("Название", BuildCallData(ProductEditBot.ProductEditNameCmd, ProductEditBot.ModuleName, Product.Id),base.PenEmodji);

                ProductEditCategoryBtn = BuildInlineBtn("Категория", BuildCallData(ProductEditBot.ProductEditCategoryCmd, ProductEditBot.ModuleName, Product.Id), base.PenEmodji);

                ProductEditPriceBtn = BuildInlineBtn("Стоимость", BuildCallData(ProductEditBot.ProductEditPriceCmd, ProductEditBot.ModuleName, Product.Id), base.CashEmodji);

                ProductEditStockBtn = BuildInlineBtn("Остаток", BuildCallData(ProductEditBot.ProductEditStockCmd, ProductEditBot.ModuleName, Product.Id),base.DepthEmodji);

                ProductEditTextBtn = BuildInlineBtn("Описание", BuildCallData(ProductEditBot.ProductEditTextCmd, ProductEditBot.ModuleName, Product.Id),base.NoteBookEmodji);

                ProductEditPhotoBtn = BuildInlineBtn("Фотография", BuildCallData(ProductEditBot.ProductEditPhotoCmd, ProductEditBot.ModuleName, Product.Id),base.PictureEmodji);

                MoreSettingsBtn = BuildInlineBtn("Дополнительно", BuildCallData(ProductEditBot.MoreProdFuncCmd, ProductEditBot.ModuleName, Product.Id), base.Next2Emodji);

                base.TextMessage = Product.AdminMessage();

                if (Product.Enable == true)
                    ProductEditEnableBtn = BuildInlineBtn("Активно", BuildCallData(ProductEditBot.ProductEditEnableCmd, ProductEditBot.ModuleName, Product.Id,1),base.CheckEmodji);

                else
                    ProductEditEnableBtn = BuildInlineBtn("Скрыто", BuildCallData(ProductEditBot.ProductEditEnableCmd, ProductEditBot.ModuleName, Product.Id,0),base.UnCheckEmodji);

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
                            EditorProductBtn
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
                            ProductEditTextBtn, ProductEditPhotoBtn
                        },

                new[]
                        {
                            ProductEditEnableBtn
                        },

                new[]
                         {
                            AdminPanelBtn,MoreSettingsBtn
                         }
                ,

                });

            else
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]   {
                            OpenProductBtn,EditorProductBtn
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
                            ProductEditTextBtn, ProductEditPhotoBtn
                        },

                new[]
                        {
                            ProductEditEnableBtn
                        },

                new[]
                         {
                            AdminPanelBtn,EditorProductBtn,MoreSettingsBtn
                         }
                ,

                });

        }

        public InlineKeyboardMarkup MoreBtn()
        {
            ViewAdditionalPhotosBtn = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Доп. фото" + base.PictureEmodji, InlineFind.AdditionalProduct + "|"+ ProductId);

            ProductEditUrlBtn = BuildInlineBtn("Заметка", BuildCallData(ProductEditBot.ProductEditUrlCmd, ProductEditBot.ModuleName, ProductId), base.PenEmodji);

            AdminPanelBtn = BackToAdminPanelBtn();

            UnitBtn = BuildInlineBtn("Ед.изм.", BuildCallData(ProductEditBot.ProudctUnitCmd, ProductEditBot.ModuleName, ProductId), base.WeigherEmodji);

            BackBtn = BuildInlineBtn("Назад", BuildCallData(ProductEditBot.BackToProductEditorCmd, ProductEditBot.ModuleName, ProductId), base.Previuos2Emodji, false);


            InsertAdditionalPhotosBtn = BuildInlineBtn("Добавить доп. фото", BuildCallData(ProductEditBot.InsertAdditionalPhotosCmd, ProductEditBot.ModuleName, ProductId), base.PictureEmodji);


            return new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            ProductEditUrlBtn, UnitBtn
                        },
                new[]
                        {
                             ViewAdditionalPhotosBtn,InsertAdditionalPhotosBtn
                        },

                new[]
                         {
                            BackBtn,AdminPanelBtn
                         }
                ,

                });
        }
    }
}
