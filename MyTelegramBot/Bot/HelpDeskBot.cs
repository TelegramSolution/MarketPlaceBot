using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.Core;
using MyTelegramBot.BusinessLayer;

namespace MyTelegramBot.Bot
{
    public class HelpDeskBot : BotCore
    {
        public const string ModuleName = "Help";

        //private HelpDeskEditorMessage HelpDeskEditorMsg { get; set; }


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
                return OkResult;
        }

        protected override void Initializer()
        {
            
        }



        private async Task<IActionResult> AddTextToHelpDesk()
        {
            var NoSendHelp=HelpDeskFunction.InsertHelpDesk(FollowerId, BotInfo.Id, ReplyToMessageText);
            BotMessage = new HelpDeskEditorMessage(NoSendHelp);
            await SendMessage(BotMessage.BuildMsg());
            return OkResult;

        }

        /// <summary>
        /// Провереям какие файлы пытается прикрепить к заявке пользователь, вытаскиваем их с сервера телегам и загружает в базу данных бота
        /// </summary>
        /// <returns></returns>
        private async Task<int> CheckAttach()
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
        
        /// <summary>
        /// Прикрепить еще вложения к заявке
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> GetHelpDeskNoSendAndAddAttach()
        {
            int AttachId = await CheckAttach();

            var NoSendHelp = HelpDeskFunction.GetHelpDesk(FollowerId, BotInfo.Id);

            var attach= HelpDeskFunction.AddAttachToHelpDesk(AttachId, NoSendHelp.Id);

            if(attach!=null)
                NoSendHelp.HelpDeskAttachment.Add(attach);

            BotMessage = new HelpDeskEditorMessage(NoSendHelp);
            await SendMessage(BotMessage.BuildMsg());
            return OkResult;
            
        }

        private async Task<IActionResult> SaveHelpDesk(int HelpDeskId)
        {
            if (!FollowerFunction.IsBlocked(FollowerId))
            {
                var Help = HelpDeskFunction.SaveHelpDesk(HelpDeskId);

                BotMessage = new HelpDeskEditorMessage(Help);
                await EditMessage(BotMessage.BuildMsg());

                AdminHelpDeskMessage adminHelpDesk = new AdminHelpDeskMessage(Help);
                await base.SendMessageAllBotEmployeess(adminHelpDesk.BuildMsg());
            }

            else
                await SendMessage(new BotMessage { TextMessage = "Пользователь заблокирован!" });

            return OkResult;
            
        }
    }
}
