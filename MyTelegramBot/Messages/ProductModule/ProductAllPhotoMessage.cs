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
using Telegram.Bot.Types;
using System.IO;

namespace MyTelegramBot.Messages
{
    public class ProductAllPhotoMessage : Bot.BotMessage
    {
        private int ProductId { get; set; }

        private Product Product { get; set; }

        private List<InputMediaBase> PhotoListMedia { get; set; }

        private int BotId { get; set; }

        private MarketBotDbContext db { get; set; }

        public MediaGroup MediaGroupPhoto { get; set; }

        public ProductAllPhotoMessage(int ProductId, int BotId)
        {
            this.ProductId = ProductId;
            this.BotId = BotId;
        }

        public ProductAllPhotoMessage BuildMessage()
        {
            db = new MarketBotDbContext();

            PhotoListMedia = new List<InputMediaBase>(10);

            MediaGroupPhoto = new MediaGroup();

            MediaGroupPhoto.FsIdTelegramFileId = new Dictionary<int, string>(10);

            GetPhotoList();


            MediaGroupPhoto.ListMedia = PhotoListMedia;

            BackBtn = new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("Назад", BuildCallData(Bot.ProductBot.GetProductCmd, ProductBot.ModuleName, ProductId));

            base.TextMessage = "Вернуться назад";

            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            BackBtn
                        },
                });

            return this;
        }

        private void GetPhotoList()
        {
            //Ищем фотографии для этого бота
            var photo = db.ProductPhoto.Where(p => p.ProductId==ProductId && p.MainPhoto==false).ToList();

            

            //Проверяем загружены ли фотографии на сервер телеграм 
            foreach(ProductPhoto pp in photo)
            {
                var TelegramAttach = db.AttachmentTelegram.Where(a => a.AttachmentFsId == pp.AttachmentFsId && a.BotInfoId == BotId).FirstOrDefault();

                //файл уже загружен на сервер. Вытаскиваем FileID
                if (TelegramAttach != null)
                {
                    string Caption = db.AttachmentFs.Find(TelegramAttach.AttachmentFsId).Caption;
                    
                    InputMediaType inputMediaType = new InputMediaType(TelegramAttach.FileId);

                    InputMediaPhoto mediaPhoto = new InputMediaPhoto { Media = inputMediaType, Caption= Caption };

                    PhotoListMedia.Add(mediaPhoto);

                    //Добавляем в ассоциативный массив данные о том что эта фотография уже загружена на сервер телеграм
                    MediaGroupPhoto.FsIdTelegramFileId.Add(Convert.ToInt32(TelegramAttach.AttachmentFsId), TelegramAttach.FileId);
                }

                //Файл еще не загруже на север телеграм
                else
                {
                    var AttachFs = db.AttachmentFs.Find(Convert.ToInt32(pp.AttachmentFsId));

                    MemoryStream ms = new MemoryStream(AttachFs.Fs);

                    InputMediaType inputMediaType = new InputMediaType(AttachFs.Name, ms);

                    InputMediaPhoto mediaPhoto = new InputMediaPhoto { Media = inputMediaType, Caption = AttachFs.Caption };

                    PhotoListMedia.Add(mediaPhoto);

                    MediaGroupPhoto.FsIdTelegramFileId.Add(Convert.ToInt32(pp.AttachmentFsId), null);
                }
            }
        }
    }
}
