using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.Core;
using MyTelegramBot.BusinessLayer;

namespace MyTelegramBot.Bot.AdminModule
{
    /// <summary>
    /// Обработка заявок HelpDesk
    /// </summary>
    public class HelpDeskProccessingBot : BotCore
    {
        public const string ModuleName = "HelpAdm";

        public const string ViewAttachCmd = "ViewAttach";

        public const string AddHelpAnswerCmd = "AddHelpAnswer";

        public const string CloseHelpCmd = "CloseHelp";

        public const string TakeToWorkCmd = "TakeToWork";

        public const string ViewContactCmd = "ViewHelpContact";

        public const string FreeHelpCmd = "FreeHelp";

        private const string AddCommentForceReply = "Добавить комментарий к заявке:";

        public const string GetHelpDeskCmd = "GetHelpDesk";

        /// <summary>
        /// Назначить себя исполнитем вместо кого то
        /// </summary>
        public const string CmdOverridePerformerHelp = "OverridePerformerHelp";

        private int HelpDeskId { get; set; }

        private int HelpNumber { get; set; }

        private HelpDesk HelpDesk { get; set; }

        private AdminHelpDeskMessage AdminHelpDeskMsg { get; set; }

        private HelpDeskMiniViewMessage HelpDeskMiniViewMsg { get; set; }

        private OverridePerformerConfirmMessage  OverridePerformerConfirmMsg { get; set; }

        public HelpDeskProccessingBot(Update _update) : base(_update)
        {


        }

        public async override Task<IActionResult> Response()
        {
            if (base.CommandName.Contains("/ticket") && IsOperator() || base.CommandName.Contains("/ticket") && IsOwner())
                return await SendHelpDesk();

            if (IsOwner() || IsOperator())
            {
                switch (CommandName)
                {
                    case TakeToWorkCmd:
                        return await TakeToWork();

                    case ViewContactCmd:
                        return await GetContact();

                    case ViewAttachCmd:
                        return await SendHelpDeskAttach();

                    case GetHelpDeskCmd:
                        return await SendHelpDesk();

                    case AddHelpAnswerCmd:
                        return await SendForceReplyMessage(AddCommentForceReply + HelpNumber);

                    case CloseHelpCmd:
                        return await CloseHelpDesk();

                    case FreeHelpCmd:
                       return await FreeHelpDesk();

                    default:
                        break;
                }

                if (base.OriginalMessage.Contains(AddCommentForceReply))
                    return await AddHelpDeskAnswer();

                else return null;
            }
            else return null;

        }

        protected override void Initializer()
        {

            try
            {
                if (Argumetns.Count > 0)
                    HelpDeskId = Argumetns[0];

                if (Argumetns.Count == 2)
                    HelpNumber = Argumetns[1];
            }

            catch
            {

            }
        }

        private async Task<IActionResult> SendHelpDesk()
        {
            try
            {

                if (HelpDeskId == 0)
                {
                    HelpNumber = Convert.ToInt32(CommandName.Substring(7));
                    HelpDesk = HelpDeskFunction.HelpDeskFindByNumber(HelpNumber);
                }

                if (HelpDeskId > 0)
                    HelpDesk = HelpDeskFunction.GetHelpDesk(HelpDeskId);
                
                if (HelpDesk != null)
                {
                    AdminHelpDeskMessage adminHelpDesk = new AdminHelpDeskMessage(HelpDesk, FollowerId);
                    await SendMessage(adminHelpDesk.BuildMsg());
                }

                return OkResult;
            }

            catch
            {
                return OkResult;
            }
                    

        }

        private async Task<IActionResult> SendHelpDesk(int Id)
        {
            if (Id > 0)
                HelpDesk = HelpDeskFunction.GetHelpDesk(Id);

            if (HelpDesk != null)
            {
                AdminHelpDeskMessage adminHelpDesk = new AdminHelpDeskMessage(HelpDesk, FollowerId);
                await SendMessage(adminHelpDesk.BuildMsg());
            }

            return OkResult;
  
        }

        private async Task<IActionResult> TakeToWork()
        {
           var WhoItWorkNow= HelpDeskFunction.WhoItWorkNow(HelpDeskId);

            HelpDesk = HelpDeskFunction.GetHelpDesk(HelpDeskId);

            if (WhoItWorkNow == null || WhoItWorkNow != null && WhoItWorkNow.Follower.Id == FollowerId)
            {
                HelpDeskFunction.TakeToWork(HelpDeskId, FollowerId);
                return await SendHelpDesk(HelpDeskId);
            }

            if (WhoItWorkNow != null && WhoItWorkNow.Id != FollowerId) // заявка в обработке у другого пользователя. Отправляем сообщение с вопрос о переназначении 
            {
                OverridePerformerConfirmMsg = new OverridePerformerConfirmMessage(HelpDesk, WhoItWorkNow);
                await EditMessage(OverridePerformerConfirmMsg.BuildMsg());
            }

            return OkResult;

        }

        private async Task<IActionResult> GetContact()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {

                var follower = db.Follower.Where(f => f.Id == FollowerId).FirstOrDefault();

                if (follower != null && follower.Telephone != null && follower.Telephone != "")
                {
                    Contact contact = new Contact
                    {
                        FirstName = follower.FirstName,
                        PhoneNumber = follower.Telephone

                    };

                    await SendContact(contact);

                }

                if (follower != null && follower.UserName != null && follower.UserName != "")
                {
                    await SendUrl(BotMessage.HrefUrl("https://t.me/" + follower.UserName, follower.UserName));
                    return OkResult;
                }

                else
                    return base.OkResult;
            }
        }

        private async Task<IActionResult> SendHelpDeskAttach()
        {

            var attach_list = HelpDeskFunction.GetHelpDeskAttachment(HelpDeskId);

            var help = HelpDeskFunction.GetHelpDesk(HelpDeskId);

            if (help != null && attach_list.Count > 0)
            {
                HelpDeskViewAttachMessage viewAttachMessage = new HelpDeskViewAttachMessage(help, attach_list,BotInfo.Id);
                var mess = viewAttachMessage.BuildMessage();

                foreach (var attach in mess)
                    await SendMediaMessage(attach);                        
            }

            else
                await AnswerCallback();

            return OkResult;


        }

        private async Task<IActionResult> AddHelpDeskAnswer()
        {
            try
            {
                HelpNumber =Convert.ToInt32(OriginalMessage.Substring(AddCommentForceReply.Length));

                HelpDesk= HelpDeskFunction.HelpDeskFindByNumber(HelpNumber);

                if (HelpDesk != null)
                {
                    HelpDesk=HelpDeskFunction.AddAnswerComment(HelpDesk.Id, FollowerId, ReplyToMessageText);
                    AdminHelpDeskMsg = new AdminHelpDeskMessage(HelpDesk, FollowerId);
                    await SendMessage(AdminHelpDeskMsg.BuildMsg());
                    
                }

                return OkResult;
            }

            catch
            {
                return OkResult;
            }
        }

        private async Task<IActionResult> CloseHelpDesk()
        {
            var WhoItWorkNow = HelpDeskFunction.WhoItWorkNow(HelpDeskId);

            if (WhoItWorkNow.Follower.Id == FollowerId)
            {
                HelpDeskFunction.HelpDeskClosed(HelpDeskId, FollowerId);
                return await SendHelpDesk(HelpDeskId);
            }

            else
            {
                await SendMessage(new BotMessage { TextMessage = "Заявка в обработке у " + WhoItWorkNow.Follower.FirstName + " " + WhoItWorkNow.Follower.LastName });
                return OkResult;
            }
        }

        private async Task<IActionResult> FreeHelpDesk()
        {
           var WhoItWorkNow = HelpDeskFunction.WhoItWorkNow(HelpDeskId);

           if(WhoItWorkNow.Follower.Id==FollowerId)
            {
                HelpDeskFunction.FreeHelpDesk(HelpDeskId, FollowerId);
                return await SendHelpDesk(HelpDeskId);
            }

            else
            {
                await SendMessage(new BotMessage { TextMessage = "Заявка в обработке у " + WhoItWorkNow.Follower.FirstName + " " + WhoItWorkNow.Follower.LastName });
                return OkResult;
            }
        }
      
    }
}
