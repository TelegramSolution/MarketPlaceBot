using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.BusinessLayer;
using MyTelegramBot.Bot.Core;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InlineKeyboardButtons;
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot;
using MyTelegramBot.Model;


namespace MyTelegramBot.InlineResult
{
    public class AdditionalPhotoSearchInline:BotInline
    {
        List<AdditionalPhoto> AdditionalPhotos { get; set; }

        int ProductId { get; set; }

        private int BotId { get; set; }

        private InlineKeyboardCallbackButton RemoveAdditionalPhotoBtn { get; set; }

        private InlineKeyboardCallbackButton BackToProductEditorBtn { get; set; }

        private InlineKeyboardButton ViewAdditionalPhotoBtn { get; set; }

        public AdditionalPhotoSearchInline(string Query):base(Query)
        {

            try
            {
                ProductId = Convert.ToInt32(Query);
            }
            catch
            {

            }
        }

        public override InlineQueryResult[] GetResult()
        {
            BotId = GeneralFunction.GetBotInfo().Id;

            AdditionalPhotos = ProductFunction.GetAdditionalPhoto(ProductId, BotId);

            InlineQueryResult [] result = new InlineQueryResult[AdditionalPhotos.Count];

            InlineQueryResultCachedPhoto[] cachedPhoto = new InlineQueryResultCachedPhoto[AdditionalPhotos.Count];

            int i = 0;

            foreach(var photo in AdditionalPhotos)
            {
                RemoveAdditionalPhotoBtn = BuildInlineBtn("Удалить", BuildCallData(ProductEditBot.RemoveAdditionalPhotoCmd, ProductEditBot.ModuleName, photo.ProductId,photo.AttachFsId));
                BackToProductEditorBtn = BuildInlineBtn("Вернуться к товару", BuildCallData(ProductEditBot.ProductEditorCmd, ProductEditBot.ModuleName, photo.ProductId));
                ViewAdditionalPhotoBtn= InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Другие фотографии", InlineFind.AdditionalProduct + "|" + ProductId);
                cachedPhoto[i] = new InlineQueryResultCachedPhoto();
                cachedPhoto[i].FileId = photo.FileId;
                cachedPhoto[i].Caption = photo.Caption;
                cachedPhoto[i].Id = photo.AttachFsId.ToString();
                cachedPhoto[i].ReplyMarkup = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(
                                            new[]
                                            {
                                                        new[]
                                                        {
                                                            RemoveAdditionalPhotoBtn
                                                        },
                                                        new[]
                                                        {
                                                            BackToProductEditorBtn
                                                        },
                                                        new[]
                                                        {
                                                            ViewAdditionalPhotoBtn
                                                        }
                                            });

                result[i] = cachedPhoto[i];
                i++;
            }
            return result;
        }
    }
}
