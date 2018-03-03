using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Bot
{
    public class HelpDeskBot : BotCore
    {
        public const string ModuleName = "Help";

        private HelpDeskEditorMessage HelpDeskEditorMsg { get; set; }

        const int AttachTypePhoto = 1;

        const int AttachTypeVideo = 2;

        const int AttachTypeAudio = 3;

        const int AttachTypeVoice = 4;

        const int AttachTypeVideoNote = 5;

        public HelpDeskBot(Update _update) : base(_update)
        {

        }

        public async override Task<IActionResult> Response()
        {
            if (base.CommandName == "/help" || base.CommandName == "Help")
            {
                await SendMessage(new BotMessage { TextMessage = "Техническая поддержка." });
                await SendForceReplyMessage("Опишите вашу проблему");

                return OkResult;
            }

            if (base.OriginalMessage == "Опишите вашу проблему")
                return await AddTextToHelpDesk();

            if (base.CommandName == "AddAttachHelpDesk")
                return await SendForceReplyMessage("Прикрепите файл(ы)");

            if (base.OriginalMessage == "Прикрепите файл(ы)")
               return await GetHelpDeskNoSendAndAddAttach();

            if (base.CommandName == "SendHelpDesk")
                return await SaveHelpDesk(Argumetns[0]);


            else
                return NotFoundResult;
        }

        protected override void Constructor()
        {
            
        }



        private async Task<IActionResult> AddTextToHelpDesk()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {

                var NoSendHelp = db.HelpDesk.Where(h => h.Send == false && FollowerId == FollowerId && h.BotInfoId==BotInfo.Id).Include(h=>h.HelpDeskAttachment).FirstOrDefault();

                //У пользователя есть не отправленная заявка. Работаем с ней
                if (NoSendHelp != null && ReplyToMessageText!=null || NoSendHelp != null && ReplyToMessageText != null)
                {
                    NoSendHelp.Text = ReplyToMessageText;
                    db.SaveChanges();
                    AddAttachToHelpDesk(await CheckAttach(), NoSendHelp.Id);
                    
                    if (NoSendHelp.Id > 0)
                    {
                        HelpDeskEditorMsg = new HelpDeskEditorMessage(NoSendHelp);
                        await SendMessage(HelpDeskEditorMsg.BuildMsg());
                    }

                    return OkResult;
                }

                //У пользователя нет не отправленных заявок. Создаем новую, но не даем Номер и делам статус не отправлена
                if (NoSendHelp == null && ReplyToMessageText != null || NoSendHelp == null && ReplyToMessageText != null)
                {
                    HelpDesk help = new HelpDesk
                    {
                        FollowerId = FollowerId,
                        Text = ReplyToMessageText,
                        Send = false,
                        BotInfoId=BotInfo.Id
                    };

                    db.HelpDesk.Add(help);
                    db.SaveChanges();
                    AddAttachToHelpDesk(await CheckAttach(), help.Id);

                    if (help.Id > 0)
                    {
                        HelpDeskEditorMsg = new HelpDeskEditorMessage(help);
                        await SendMessage(HelpDeskEditorMsg.BuildMsg());
                    }

                    return OkResult;
                }

                if (ReplyToMessageText == null && Caption == null)
                {
                    await SendMessage(new BotMessage { TextMessage = "Вы должны описать вашу проблему!" });
                    return OkResult;
                }

                else
                  return NotFoundResult;
                
            }
        }

        /// <summary>
        /// Провереям какие файлы пытается прикрепить к заявке пользователь
        /// </summary>
        /// <returns></returns>
        private async Task<int> CheckAttach()
        {
            AttachmentTelegram attachment = new AttachmentTelegram();

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                if (base.FileId != null && base.FileId != "")
                {
                    await SendMessage(new BotMessage { TextMessage = "Можно прикреплять только Фото, Аудио, Видео" });
                    return -1;
                }

                if (base.PhotoId != null)
                    return await base.InsertToAttachmentFs(base.PhotoId);

                if (base.VideoId != null)
                    return await base.InsertToAttachmentFs(base.VideoId);

                if (base.AudioId != null)
                    return await base.InsertToAttachmentFs(base.AudioId);

                if (base.VoiceId != null)
                    return await base.InsertToAttachmentFs(base.VoiceId);

                if (base.VideoNoteId != null)
                    return await base.InsertToAttachmentFs(base.VideoNoteId);


                else
                    return -1;
            }
        }

        /// <summary>
        /// Прикрепить файлы к заявке
        /// </summary>
        /// <param name="AttachId"></param>
        /// <param name="HelpDeskId"></param>
        /// <returns></returns>
        private HelpDeskAttachment AddAttachToHelpDesk(int AttachFsId, int HelpDeskId)
        {

            if (AttachFsId > 0 && HelpDeskId>0)
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {


                    HelpDeskAttachment attachment = new HelpDeskAttachment
                    {
                        AttachmentFsId = AttachFsId,
                        HelpDeskId = HelpDeskId
                    };
                   

                    db.HelpDeskAttachment.Add(attachment);

                    db.SaveChanges();
                    return attachment;
                }
            }

            else
                return null;
        }

        /// <summary>
        /// Прикрепить еще вложения к заявке
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> GetHelpDeskNoSendAndAddAttach()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var NoSendHelp = db.HelpDesk.Where(h => h.Send == false && FollowerId == FollowerId).Include(h => h.HelpDeskAttachment).FirstOrDefault();
                var attach= AddAttachToHelpDesk(await CheckAttach(), NoSendHelp.Id);

                if(attach!=null)
                    NoSendHelp.HelpDeskAttachment.Add(attach);

                HelpDeskEditorMsg = new HelpDeskEditorMessage(NoSendHelp);
                await SendMessage(HelpDeskEditorMsg.BuildMsg());
                return OkResult;
            }
        }

        private async Task<IActionResult> SaveHelpDesk(int HelpDeskId)
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                var Help = db.HelpDesk.Where(h => h.Id == HelpDeskId && h.Send == false).FirstOrDefault();

                var LastHelp = db.HelpDesk.Where(h=>h.Send==true).OrderByDescending(h=>h.Number).Include(h=>h.HelpDeskAttachment).FirstOrDefault();

                if (Help != null)
                {
                    int number = 1;

                    if (LastHelp != null)
                        number = Convert.ToInt32(LastHelp.Number) + 1;

                    Help.Number = number;
                    Help.Send = true;
                    Help.Timestamp = DateTime.Now;
                    Help.InWork = false;
                    Help.Closed = false;

                    db.SaveChanges();

                    HelpDeskEditorMsg = new HelpDeskEditorMessage(Help);
                    var message = HelpDeskEditorMsg.BuildMsg();
                    await EditMessage(message);
                    AdminHelpDeskMessage adminHelpDesk = new AdminHelpDeskMessage(Help.Id);
                    await base.SendMessageAllBotEmployeess(adminHelpDesk.BuildMsg());
                    return OkResult;
                }

                else
                    return NotFoundResult;
            }
        }
    }
}
