using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace MyTelegramBot.Bot
{
    public abstract class BotCore
    {

        private TelegramBotClient TelegramClient { get; set; }

        /// <summary>
        /// Объект апдейт
        /// </summary>
        protected Update Update { get; set; }

        /// <summary>
        /// Кем послано сообщение боту
        /// </summary>
        protected long ChatId { get; set; }

        /// <summary>
        /// Колл бэк id. При нажатии на кнопку
        /// </summary>
        private string CallBackQueryId { get; set; }

        /// <summary>
        /// Объект команды которая содержится в теле кнопки
        /// </summary>
        private BotCommand BotCommand { get; set; }

        /// <summary>
        /// Название команды 
        /// </summary>
        protected string CommandName { get; set; }

        /// <summary>
        /// Аргументы 
        /// </summary>
        protected List<int> Argumetns { get; set; }

        /// <summary>
        /// id пользователя который в базе
        /// </summary>
        protected int FollowerId { get; set; }

        /// <summary>
        /// id отправленно сообщения
        /// </summary>
        protected int MessageId { get; set; }

        /// <summary>
        /// 200 ответ
        /// </summary>
        protected OkResult OkResult { get; set; }

        /// <summary>
        /// 404 ответ
        /// </summary>
        protected NotFoundResult NotFoundResult { get; set; }

        /// <summary>
        /// Объект описывающий событие нажатой Inline кнопки
        /// </summary>
        private CallbackQuery CallbackQuery { get; set; }

        /// <summary>
        /// Цитата
        /// </summary>
        protected string ReplyToMessageText { get; set; }

        /// <summary>
        /// Оригинальное сообщение которое процитировали
        /// </summary>
        protected string OriginalMessage { get; set; }

        /// <summary>
        /// id фотографии на сервере телеграм
        /// </summary>
        protected string PhotoId { get; set; }

        /// <summary>
        /// ссылка
        /// </summary>
        protected string WebUrl { get; set; }

        /// <summary>
        /// id файл на сервере телеграм
        /// </summary>
        protected string FileId { get; set; }

        /// <summary>
        /// Подпись к фотографии
        /// </summary>
        protected string Caption { get; set; }


        /// <summary>
        /// chatid владельца бота
        /// </summary>
        protected long BotOwner { get; set; }

        protected string VideoId { get; set; }

        protected string AudioId { get; set; }

        protected string VideoNoteId { get; set; }

        protected string VoiceId { get; set; }

        /// <summary>
        /// id чата куда отсылаются уведомления о заявках и заказах
        /// </summary>
        protected long GroupChatId { get; set; }

        /// <summary>
        /// Инф о боте. Токен, название
        /// </summary>
        protected BotInfo BotInfo { get; set; }

        /// <summary>
        /// конфигурация бота из бд
        /// </summary>
        protected Configuration ConfigurationBot { get; set; }


        protected int MediaFileTypeId { get; set; }

        public BotCore(Update update)
        {
            try
            {
                this.Update = update;
                BotInfo = GetBotInfo();
                TelegramClient = new TelegramBotClient(BotInfo.Token);

                this.Argumetns = new List<int>();
                this.OkResult = new OkResult();
                this.NotFoundResult = new NotFoundResult();
                OriginalMessage = String.Empty;
                ReplyToMessageText = String.Empty;
                this.CommandName = String.Empty;

                if (this.Update.CallbackQuery != null) // Если пользователь нажал на Inline кнопку
                {
                    this.CallbackQuery = update.CallbackQuery;
                    UpdateParser(this.CallbackQuery);

                }


                if (this.Update.Message != null) // Пришло текстовое сообщение
                    UpdateParser(update.Message);

                Constructor();

            }

            catch (Exception exp)
            {

            }

        }

        protected abstract void Constructor();

        /// <summary>
        /// id пользователя в в БД
        /// </summary>
        /// <param name="ChatId"></param>
        /// <returns></returns>
        private int GetFollowerID(long ChatId)
        {
            int id = 0;
            try
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    var follower = db.Follower.Where(f => f.ChatId == Convert.ToInt32(ChatId)).FirstOrDefault();

                    if (follower != null)
                        id = follower.Id;
                }


            }
            catch (Exception exp)
            {

            }
            return id;
        }

        /// <summary>
        /// Проверяем явлется ли оператором
        /// </summary>
        /// <returns></returns>
        protected bool IsOperator(int FollowerID=0)
        {
            try
            {
                int id = 0;

                if (FollowerID > 0)
                    id = FollowerID;

                else
                    id = FollowerId;

                using (MarketBotDbContext db = new MarketBotDbContext())
                {

                   var admin = db.Admin.Where(a => a.FollowerId == id && a.Enable).FirstOrDefault();

                    if (admin != null)
                        return true;

                    else
                        return false;

                }
            }

            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Является ли тот кто отправил сообщение боту, владельцем данного бота
        /// </summary>
        /// <returns></returns>
        protected bool IsOwner()
        {
            if (BotInfo.OwnerChatId == ChatId)
                return true;

            else
                return false;
        }

        /// <summary>
        /// Информация о боте
        /// </summary>
        /// <returns></returns>
        private BotInfo GetBotInfo()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("name.json");
                string name = builder.Build().GetSection("name").Value;

                BotInfo bot = new BotInfo();
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    bot = db.BotInfo.Where(b=>b.Name==name).Include(b=>b.Configuration).FirstOrDefault();

                    if (bot != null)
                    {
                        this.BotOwner =Convert.ToInt32(bot.OwnerChatId);
                        return bot;
                    }

                    else
                        return db.BotInfo.FirstOrDefault();

                }
            }

            catch (Exception e)
            {
                return null;
            }
        }

        protected Configuration GetConfigurationBot(int BotId)
        {
            try
            {
                using (MarketBotDbContext db=new MarketBotDbContext())
                {
                   return db.Configuration.Where(c => c.BotInfoId == BotId).Include(c=>c.Currency).FirstOrDefault();
                }
            }

            catch (Exception exp)
            {
                return null;
            }
        }


        private void UpdateParser(CallbackQuery callbackQuery)
        {
            try
            {
                this.ChatId = callbackQuery.From.Id;
                this.CallBackQueryId = callbackQuery.Id;
                BotCommand = JsonConvert.DeserializeObject<BotCommand>(callbackQuery.Data);
                FollowerId = GetFollowerID(this.ChatId);

                if(callbackQuery.Message!=null)
                    MessageId = callbackQuery.Message.MessageId;
                //если пришло сообщение из группового чата
                if (callbackQuery.Message!=null && callbackQuery.Message.Chat != null)
                    this.GroupChatId = callbackQuery.Message.Chat.Id;


                if (BotCommand.Cmd != null)
                {
                    this.CommandName = BotCommand.Cmd;
                    this.Argumetns = BotCommand.Arg;
                }

                else
                    BotCommand.Cmd = String.Empty;
            }

            catch
            {

            }
        }

        private void UpdateParser(Message message)
        {
            this.ChatId = message.From.Id;
            FollowerId = GetFollowerID(this.ChatId);

            this.MessageId = message.MessageId;
            ///Пришла ссылка
            if (message.Entities != null && message.Entities.Count > 0 && message.Entities[0].Type == MessageEntityType.Url)
                WebUrl = message.Text;

            //если пришло сообщение из группового чата
            if (message.Chat != null)
                this.GroupChatId = message.Chat.Id;


            ///Пришло сообщение с текстом, например команда
            if (message.Text != null)
                this.CommandName = message.Text;

            //если пришла команда вида /cmd@Botname
            if (this.CommandName!=null && this.CommandName.Contains('@'))
                this.CommandName = this.CommandName.Substring(0, this.CommandName.IndexOf('@'));

            //пользователь процитировал сообщение от бота
            if (message.ReplyToMessage != null && message.ReplyToMessage.From != null 
                && message.ReplyToMessage.From.Username == GeneralFunction.GetBotName())
            {
                OriginalMessage = message.ReplyToMessage.Text;
                ReplyToMessageText = message.Text;
            }

            else // нет процитированного сообещния
            {
                OriginalMessage = String.Empty;
                ReplyToMessageText = String.Empty;
            }

            ///пользователь процитировал сообщение бота, сообещнием фотографией
            if (message.Photo != null && message.ReplyToMessage != null)
            {
                OriginalMessage = message.ReplyToMessage.Text;
                ReplyToMessageText = message.Text;
                Caption = message.Caption;
                PhotoId = message.Photo[this.Update.Message.Photo.Length - 1].FileId;
            }

            if (message.Caption != null)
                Caption = message.Caption;

            //пришла фотография
            if (message.Photo != null)
            {
                this.PhotoId = message.Photo[message.Photo.Length - 1].FileId;
                this.MediaFileTypeId = Core.ConstantVariable.MediaTypeVariable.Photo;
            }

            if (message.Video != null)
            {
                this.VideoId = message.Video.FileId;
                this.MediaFileTypeId = Core.ConstantVariable.MediaTypeVariable.Video;
            }

            if (message.Audio != null)
            {
                this.AudioId = message.Audio.FileId;
                this.MediaFileTypeId = Core.ConstantVariable.MediaTypeVariable.Audio;
            }

            if (message.Document != null)
            {
                this.FileId = message.Document.FileId;
                this.MediaFileTypeId = Core.ConstantVariable.MediaTypeVariable.Document;
            }

            if (message.VideoNote != null)
            {
                this.VideoNoteId = message.VideoNote.FileId;
                this.MediaFileTypeId = Core.ConstantVariable.MediaTypeVariable.VideoNote;
                
            }

            if (message.Voice != null)
            {
                this.VoiceId = message.Voice.FileId;
                this.MediaFileTypeId = Core.ConstantVariable.MediaTypeVariable.Voice;
            }
            
        }

        public abstract Task<IActionResult> Response();

        /// <summary>
        /// Отправить сообщение 
        /// </summary>
        /// <param name="botMessage"></param>
        /// <param name="EditMessageId"></param>
        /// <param name="ReplyToMessageId"></param>
        /// <returns></returns>
        protected async Task<Message> SendMessage(BotMessage botMessage, int EditMessageId = 0, int ReplyToMessageId = 0)
        {
            IReplyMarkup replyMarkup;
            try
            {
                replyMarkup = botMessage.MessageReplyMarkup;


                if (botMessage != null&& this.Update.CallbackQuery != null && this.CallBackQueryId != null)
                    await AnswerCallback(botMessage.CallBackTitleText);

                if (botMessage != null && EditMessageId != 0)
                    return await TelegramClient.EditMessageTextAsync(this.ChatId, EditMessageId, botMessage.TextMessage, ParseMode.Html, true, replyMarkup);

                if (botMessage != null && botMessage.TextMessage != null)
                    return await TelegramClient.SendTextMessageAsync(this.ChatId, botMessage.TextMessage, ParseMode.Html, true, false, ReplyToMessageId, replyMarkup);

                else
                    return null;
            }

            catch 
            {
                //await telegram.SendTextMessageAsync(this.ChatId, botMessage.Text, ParseMode.Html, false, false, ReplyToMessageId, botMessage.InlineKeyboard);

                return null;
            }
        }

        /// <summary>
        /// Отправить несолько фотографии альбомом
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        protected async Task<bool> SendMediaPhotoGroup(MediaGroup mediaGroup)
        {
            try
            {
                if (mediaGroup!=null && mediaGroup.ListMedia != null && mediaGroup.ListMedia.Count > 0)
                {
                   var res= await TelegramClient.SendMediaGroupAsync(ChatId, mediaGroup.ListMedia);

                    int counter = 0;
                    foreach (Message mess in res) // проверяем есть ли FileId у отправленных фотографий. Если нет, то заносим в БД
                    {
                        if (mediaGroup.FsIdTelegramFileId.ElementAt(counter).Value == null)
                            InsertToAttachmentTelegram(mess.Photo[mess.Photo.Length-1].FileId, mediaGroup.FsIdTelegramFileId.ElementAt(counter).Key);

                        counter++;
                    }
                }

                else
                    await this.AnswerCallback("Данные отсутствуют");

                return true;
            }

            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Изменить сообщение
        /// </summary>
        /// <param name="botMessage"></param>
        /// <returns></returns>
        protected async Task<Message> EditMessage(BotMessage botMessage)
        {
            IReplyMarkup replyMarkup;
            replyMarkup = botMessage.MessageReplyMarkup;
           
            try
            {

                if (botMessage != null&& this.Update.CallbackQuery != null && this.CallBackQueryId != null)
                    await AnswerCallback(botMessage.CallBackTitleText);

                if (botMessage != null && botMessage.TextMessage!=null)
                    return await TelegramClient.EditMessageTextAsync(this.ChatId, this.MessageId, botMessage.TextMessage, ParseMode.Html, true, replyMarkup);



                else
                    return null;

            }

            catch
            {
                return await TelegramClient.SendTextMessageAsync(this.ChatId, botMessage.TextMessage, ParseMode.Html, false, false, 0, replyMarkup);
            }

        }

        /// <summary>
        /// отправить фото
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected async Task<Message> SendPhoto(BotMessage message)
        {
            Message mess = new Message();

            try
            {

                if (this.Update.CallbackQuery != null && this.CallBackQueryId != null)
                    await AnswerCallback(message.CallBackTitleText);

                //максимальная длина подписи для фотографии 200 символов
                if (message.MediaFile!=null && message.MediaFile.Caption != null 
                    && message.MediaFile.Caption != "" && message.MediaFile.Caption.Length < 200)
                    mess= await TelegramClient.SendPhotoAsync(ChatId, message.MediaFile.FileTo, message.MediaFile.Caption, false, 0, message.MessageReplyMarkup);

                if (message.MediaFile != null && message.MediaFile.Caption == null)
                    mess= await TelegramClient.SendPhotoAsync(ChatId, message.MediaFile.FileTo, "", false, 0, message.MessageReplyMarkup);

                //если подпись для фотографии больше 200 символом, то разибваем на два сообщения 1) Фото 2) Текст
                if (message.MediaFile != null && message.MediaFile.Caption != null && message.MediaFile.Caption != "" && message.MediaFile.Caption.Length >= 200)
                {
                    mess= await TelegramClient.SendPhotoAsync(this.ChatId, message.MediaFile.FileTo, "");
                    await SendMessage(this.ChatId, new BotMessage { TextMessage = message.MediaFile.Caption, MessageReplyMarkup = message.MessageReplyMarkup });
                }

                // если фотки нет
                if (message.MediaFile != null && message.MediaFile.Caption!=null && message.MediaFile.Caption!="" && message.MediaFile.FileTo.Content==null
                    && message.MediaFile.FileTo.FileId==null)
                    mess= await TelegramClient.SendTextMessageAsync(ChatId, message.TextMessage, ParseMode.Html, false, false, 0, message.MessageReplyMarkup);

                if(message.MediaFile ==null && message.TextMessage!=null)
                    mess = await TelegramClient.SendTextMessageAsync(ChatId, message.TextMessage, ParseMode.Html, false, false, 0, message.MessageReplyMarkup);

                //Если мы отрпавляем файл для этого бота первый раз, то Записываем FileId в базу для этог бота, что бы в следующий раз не отслылать целый файл
                //а только FileId на сервере телеграм
                if (mess != null && mess.Photo!=null && message.MediaFile.AttachmentFsId>0 && message.MediaFile.FileTo.FileId==null)
                    InsertToAttachmentTelegram(message.MediaFile, mess.Photo[mess.Photo.Length - 1].FileId);

                return mess;
            }

            catch (Exception exp)
            {
                return null;
            }
        }

        /// <summary>
        /// Вставить данные о файл в таблицу AttachmentTelegram.
        /// </summary>
        /// <param name="mediaFile">Класс описывающий файл. FileId там должен быть пустой, это значит что для этого бота файл отпралвяется впервые. 
        /// Для этого мы и записываем информацию в таблицу Attachment, что бы постоянно не отправлять этот файл целиков а только FileId на серевере телеграм</param>
        /// <param name="FileId"></param>
        /// <returns></returns>
        protected int InsertToAttachmentTelegram(MediaFile mediaFile, string FileId)
        {
            try
            {
                if (mediaFile.AttachmentFsId > 0 && FileId !="")
                    using (MarketBotDbContext db = new MarketBotDbContext())
                    {
                       var Attach=db.AttachmentTelegram.Where(a => a.AttachmentFsId == mediaFile.AttachmentFsId && a.BotInfoId == BotInfo.Id).FirstOrDefault();

                        if (Attach == null)
                        {
                            AttachmentTelegram attachment = new AttachmentTelegram
                            {
                                AttachmentFsId = mediaFile.AttachmentFsId,
                                FileId = FileId,
                                BotInfoId = BotInfo.Id,
                            };

                            db.AttachmentTelegram.Add(attachment);
                            db.SaveChanges();
                            return attachment.Id;
                        }

                        if(Attach!=null && Attach.FileId == null)
                        {
                            Attach.FileId = FileId;
                            db.SaveChanges();
                            return Attach.Id;
                        }

                        else
                            return -1;
                    }

                else
                    return -1;
            }

            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Вставить данные о файл в таблицу AttachmentTelegram.
        /// Что бы постоянно не отправлять этот файл целиком а только FileId на серевере телеграм
        /// </summary>
        /// <param name="FileId"></param>
        /// <param name="AttachmentFsId"></param>
        /// <returns></returns>
        protected int InsertToAttachmentTelegram (string FileId,int AttachmentFsId)
        {
            using(MarketBotDbContext db=new MarketBotDbContext())
            {
                AttachmentTelegram attachmentTelegram = new AttachmentTelegram
                {
                    AttachmentFsId = AttachmentFsId,
                    FileId = FileId,
                    BotInfoId = BotInfo.Id
                };

                db.AttachmentTelegram.Add(attachmentTelegram);

                return db.SaveChanges();                   
            }
        }

        protected async Task<bool> AnswerCallback(string text = null, bool ShowAlert=false)
        {
            try
            {
                if (this.Update.CallbackQuery != null && this.CallBackQueryId != null && text != null)
                    return await TelegramClient.AnswerCallbackQueryAsync(this.CallBackQueryId, text, ShowAlert);

                if (this.Update.CallbackQuery != null && this.CallBackQueryId != null && text == null)
                    return await TelegramClient.AnswerCallbackQueryAsync(this.CallBackQueryId);

                else
                    return false;
            }

            catch
            {
                //if (text != null)
                //    await Telegram.SendTextMessageAsync(this.ChatId, text);

                return true;
            }
        }

        private async Task<Message> SendForceReply(string text)
        {
            try
            {
                ForceReply forceReply = new ForceReply
                {
                    Force = true,
                    Selective = true
                };
        
                return await TelegramClient.SendTextMessageAsync(ChatId, text, ParseMode.Html, false, false, 0, forceReply);

            }

            catch
            {
                return null;
            }
        }

        protected async Task<Message> SendContact(Contact contact)
        {
            try
            {

                await AnswerCallback();
                return await TelegramClient.SendContactAsync(ChatId, contact.PhoneNumber, contact.FirstName);

            }

            catch (Exception e)
            {
                return null;
            }
        }

        protected async Task<Message> SendLocation(Location location)
        {
            try
            {
                if (this.Update.CallbackQuery != null && this.CallBackQueryId != null)
                    await AnswerCallback();

                   return await TelegramClient.SendLocationAsync(ChatId, location.Latitude, location.Longitude);

            }

            catch
            {
                return null;
            }
        }

        protected async Task<Message> SendMessage(long ChatId, BotMessage botMessage, bool DisableNotifi=false)
        {
            try
            {

                if (botMessage != null && this.Update.CallbackQuery != null && this.CallBackQueryId != null)
                    await AnswerCallback(botMessage.CallBackTitleText);

                if (botMessage != null && botMessage.TextMessage != null)
                    return await TelegramClient.SendTextMessageAsync(ChatId, botMessage.TextMessage, ParseMode.Html, false, DisableNotifi, 0, botMessage.MessageReplyMarkup);

                else
                    return null;
            }

            catch (Exception exp)
            {
                return null;
            }
        }

        /// <summary>
        /// Вытаскивам с сервера телеграм этот файл и записываем его в БД бота. Нам нужно хранить эти файлы, т.к если бота заблокируют или
        /// мы подключим допольнительных ботов, то мы не сможем отправить FileId другого бота. Поэтому мы храним в бд эти файлы
        /// </summary>
        /// <param name="id">FileId файл на сервере телеграм</param>
        /// <returns>Возращает id записи из таблицы AttachmentFS</returns>
        protected async Task<int> InsertToAttachmentFs(string id=null)
        {
            if(id==null)
                id = FileId;

            try
            {
                var file = await TelegramClient.GetFileAsync(id);

                System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

                await file.FileStream.CopyToAsync(memoryStream);

                // Довавбляем в БД
                AttachmentFs attachmentFs = new AttachmentFs
                {
                    Fs = memoryStream.ToArray(),
                    GuId=Guid.NewGuid(),
                    Caption= Caption,
                    AttachmentTypeId=MediaFileTypeId

                };

                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    db.AttachmentFs.Add(attachmentFs);

                    int type = HowMediaType(Update.Message); // узнаем какой типа файла. Фото, Аудио и тд

                    //Когда оператора будет смотреть заявку через того же бота, через коготого пользователь
                    //оформлял заявку, мы отправим ему ID файла на сервере телеграм вместо целой картинки. Это будет быстрее.
                    //А если оператор будет смотреть заявку из другого бота (например старого удалят), то мы сможем отрпавить файл картинки


                    //максимальный размер файла 15 мб
                    if (file!=null && file.FileSize<=15*1024*1024 && memoryStream!=null && await db.SaveChangesAsync() > 0 && type > 0)
                    {
                        AttachmentTelegram attachment = new AttachmentTelegram();
                        attachment.FileId = id;
                        attachment.AttachmentFsId = attachmentFs.Id;
                        attachment.BotInfoId = BotInfo.Id;

                        db.AttachmentTelegram.Add(attachment);

                        if (await db.SaveChangesAsync() > 0)
                            return Convert.ToInt32(attachment.AttachmentFsId);

                        else
                            return -1;
                    }

                    if (file.FileSize > 15 * 1024 * 1024)
                    {
                        await SendMessage(new BotMessage { TextMessage = "Ошибка. Файл не может быть больше 15 мб!" });
                        return -1;
                    }

                    else
                        return -1;
                }
            }

            catch (Exception exp)
            {
                return -1;
            }
            
        }

        /// <summary>
        /// Вытащить файл с сервера телеграм по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task<Telegram.Bot.Types.File> GetFileAsync (string id)
        {
            try
            {
               return await TelegramClient.GetFileAsync(id);
            }

            catch
            {
                return null;
            }
        }

        private int HowMediaType(Message message)
        {

            if (message.Photo != null)
                return Core.ConstantVariable.MediaTypeVariable.Photo; 

            if (message.Video != null)
                return Core.ConstantVariable.MediaTypeVariable.Video;

            if (message.Audio != null)
                return Core.ConstantVariable.MediaTypeVariable.Audio;

            if (message.Voice != null)
                return Core.ConstantVariable.MediaTypeVariable.Voice;

            if (message.VideoNote != null)
                return Core.ConstantVariable.MediaTypeVariable.VideoNote;

            else return -1;

        }

        private async Task<Message> SendVoice (BotMessage message)
        {
            try
            {
                if (message.MediaFile.AttachmentFsId > 0 && message.MediaFile.FileTo.FileId != null)
                    InsertToAttachmentTelegram(message.MediaFile, this.VoiceId);

                return await TelegramClient.SendVoiceAsync(this.ChatId, message.MediaFile.FileTo, Caption, 0, false, 0, message.MessageReplyMarkup);
            }

            catch
            {
                return null;
            }
        }

        private async Task<Message> SendAudio(BotMessage message)
        {
            try
            {

                if (message.MediaFile.AttachmentFsId > 0 && message.MediaFile.FileTo.FileId != null)
                    InsertToAttachmentTelegram(message.MediaFile, this.AudioId);

                return await TelegramClient.SendVoiceAsync(this.ChatId, message.MediaFile.FileTo, Caption, 0, false, 0, message.MessageReplyMarkup);
            }

            catch
            {
                return null;
            }
        }

        private async Task<Message> SendVideo(BotMessage message)
        {
            try
            {
                if (message.MediaFile.AttachmentFsId > 0 && message.MediaFile.FileTo.FileId != null)
                    InsertToAttachmentTelegram(message.MediaFile, this.VideoId);

                return await TelegramClient.SendVoiceAsync(this.ChatId, message.MediaFile.FileTo, Caption, 0, false, 0, message.MessageReplyMarkup);
            }

            catch
            {
                return null;
            }
        }

        private async Task<Message> SendVideoNote(BotMessage message)
        {
            try
            {
                if (message.MediaFile.AttachmentFsId > 0 && message.MediaFile.FileTo.FileId != null)
                    InsertToAttachmentTelegram(message.MediaFile, this.VideoNoteId);

                return await TelegramClient.SendVoiceAsync(this.ChatId, message.MediaFile.FileTo, Caption, 0, false, 0, message.MessageReplyMarkup);
            }

            catch
            {
                return null;
            }
        }

        protected async Task<Message> SendMediaMessage(BotMessage message)
        {
            try
            {

                if (message.MediaFile.FileTypeId== Core.ConstantVariable.MediaTypeVariable.Audio)
                    return await SendAudio(message);

                if (message.MediaFile.FileTypeId == Core.ConstantVariable.MediaTypeVariable.Video)
                    return await SendVideo(message);

                if (message.MediaFile.FileTypeId == Core.ConstantVariable.MediaTypeVariable.Voice)
                    return await SendVoice(message);

                if (message.MediaFile.FileTypeId == Core.ConstantVariable.MediaTypeVariable.VideoNote)
                    return await SendVideoNote(message);

                if (message.MediaFile.FileTypeId == Core.ConstantVariable.MediaTypeVariable.Photo)
                    return await SendPhoto(message);

                else
                    return null;


            }

            catch
            {
                return null;
            }
        }


        protected async Task<Message> SendDocument(FileToSend FiletoSend, string Caption = "")
        {
            try
            {
                if (this.Update.CallbackQuery != null && this.CallBackQueryId != null)
                    await AnswerCallback();

                return await TelegramClient.SendDocumentAsync(this.ChatId, FiletoSend, Caption);
            }

            catch (Exception exp)
            {
                return null;
            }
        }

        protected async Task<Message> SendDocument(BotMessage message)
        {
            try
            {
                if (this.Update.CallbackQuery != null && this.CallBackQueryId != null)
                    await AnswerCallback();

                return await TelegramClient.SendDocumentAsync(this.ChatId, message.MediaFile.FileTo, message.MediaFile.Caption);
            }

            catch (Exception exp)
            {
                return null;
            }
        }

        protected async Task<bool> AnswerInlineQueryAsync(InlineQueryResult[] results)
        {
            try
            {
                return await TelegramClient.AnswerInlineQueryAsync(Update.InlineQuery.Id, results);
            }

            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Создать иОтправить ForceReply сообщение
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected virtual async Task<IActionResult> ForceReplyBuilder(string text)
        {
            if (this.Update.CallbackQuery != null)
                await AnswerCallback();

            if (await SendForceReply(text) != null)
                return OkResult;

            else
                return NotFoundResult;
        }


        /// <summary>
        /// Рассылка сообщений всем сотудникам в лс + в чат
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected async Task<bool> SendMessageAllBotEmployeess(BotMessage message)
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var list = db.Admin.Where(a => a.Enable && a.FollowerId != FollowerId).Include(a=>a.Follower).ToList();
                
                try
                {
                    
                    if(db.Configuration.Where(o => o.BotInfoId == BotOwner).FirstOrDefault()!=null &&
                        db.Configuration.Where(o=>o.BotInfoId==BotOwner).FirstOrDefault().OwnerPrivateNotify)
                        await SendMessage(BotOwner, message);

                    if (list != null)
                    {
                        foreach (var admin in list)
                        {
                            if (admin.NotyfiActive)
                            {
                                await SendMessage(admin.Follower.ChatId, message,true);
                                System.Threading.Thread.Sleep(300);
                            }
                        }

                        await SendMessageToGroupChat(message);

                        return 
                            true;
                    }

                    else
                        return false;
                }

                catch
                {
                    return 
                        false;
                }
            }
        }

        /// <summary>
        /// Отправить сообщение в групповой чат в котором находятся все операторы
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<Message> SendMessageToGroupChat(BotMessage message)
        {
            try
            {
                using(MarketBotDbContext db=new MarketBotDbContext())
                {
                    this.GroupChatId = Convert.ToInt64(db.Configuration.FirstOrDefault().PrivateGroupChatId);
                     return await SendMessage(this.GroupChatId, message);
                }
            }

            catch
            {
                return null;
            }
        }
    }
}

