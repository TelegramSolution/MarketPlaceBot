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

namespace MyTelegramBot.Bot.AdminModule
{
    public class HelpDeskProccessingBot : BotCore
    {
        public const string ModuleName = "HelpAdm";

        public const string ViewAttachCmd = "ViewAttach";

        public const string AddHelpAnswerCmd = "AddHelpAnswer";

        public const string CloseHelpCmd = "CloseHelp";

        public const string TakeHelpCmd = "TakeHelp";

        public const string ViewContactCmd = "ViewHelpContact";

        public const string FreeHelpCmd = "FreeHelp";

        private const string AddCommentForceReply = "Добавить комментарий к заявке:";

        public const string GetHelpDeskCmd = "GetHelpDesk";

        private int HelpDeskId { get; set; }

        private int HelpNumber { get; set; }

        private AdminHelpDeskMessage AdminHelpDeskMsg { get; set; }

        private HelpDeskMiniViewMessage HelpDeskMiniViewMsg { get; set; }

        private IProcessing Processing { get; set; }

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
                    case TakeHelpCmd:
                        return await TakeHelpDesk();

                    case ViewContactCmd:
                        return await GetContact();

                    case ViewAttachCmd:
                        return await SendHelpDeskAttach();

                    case GetHelpDeskCmd:
                        return await SendHelpDesk();

                    case AddHelpAnswerCmd:
                        return await SendForceReplyMessage(AddCommentForceReply + HelpNumber.ToString());

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

        protected override void Constructor()
        {
            Processing = new HelpProcess(Update);

            try
            {
                HelpDeskId = Argumetns[0];                
                using (MarketBotDbContext db = new MarketBotDbContext())
                    HelpNumber =Convert.ToInt32(db.HelpDesk.Where(h => h.Id == HelpDeskId).FirstOrDefault().Number);
            }

            catch
            {

            }
        }

        private async Task<IActionResult> SendHelpDesk()
        {
            try
            {
                int Number = 0;

                HelpDesk Help = new HelpDesk();

                if(HelpDeskId==0)
                    Number = Convert.ToInt32(CommandName.Substring(7));

                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    if(HelpDeskId==0 && Number>0)
                        Help = db.HelpDesk.Where(h => h.Number == Number).Include(h => h.HelpDeskAnswer).Include(h => h.HelpDeskAttachment).Include(h => h.HelpDeskInWork).FirstOrDefault();

                    if(HelpDeskId > 0 && Number==0)
                        Help = db.HelpDesk.Where(h => h.Id == HelpDeskId).Include(h => h.HelpDeskAnswer).Include(h => h.HelpDeskAttachment).Include(h => h.HelpDeskInWork).FirstOrDefault();


                    if (Help != null)
                    {
                        AdminHelpDeskMessage adminHelpDesk = new AdminHelpDeskMessage(Help, FollowerId);
                        await SendMessage(adminHelpDesk.BuildMsg());
                        return OkResult;
                    }

                    return OkResult;
                }
            }

            catch
            {
                return OkResult;
            }
        }

        private async Task<IActionResult> TakeHelpDesk()
        {
            try
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    var Help = db.HelpDesk.Where(h => h.Id == HelpDeskId).Include(h => h.HelpDeskAnswer).
                        Include(h => h.HelpDeskAttachment).Include(h => h.HelpDeskInWork).FirstOrDefault();

                    if (Help != null && Help.Closed == false && Help.InWork == false) // Заявка не закрыта и не находится в обработке. Значит тек. пользвоателй берет ее в обработку
                    {
                        HelpDeskInWork work = new HelpDeskInWork { FollowerId = FollowerId, HelpDeskId = Help.Id, Timestamp = DateTime.Now, InWork = true };
                        Help.InWork = true;
                        db.HelpDeskInWork.Add(work);
                        db.SaveChanges();
                        AdminHelpDeskMsg = new AdminHelpDeskMessage(Help, FollowerId);
                        await EditMessage(AdminHelpDeskMsg.BuildMsg());
                        await Processing.NotifyChanges("Заявку №" + Help.Number.ToString() + " взял в работу пользователей:" + GeneralFunction.FollowerFullName(FollowerId),Help.Id);
                        return OkResult;
                    }

                    if (Help != null && Help.Closed == false && Help.InWork == true &&
                        Help.HelpDeskInWork.FirstOrDefault() != null && Help.HelpDeskInWork.OrderByDescending(h=>h.Id).FirstOrDefault().FollowerId == FollowerId) // Заявка в обработке у пользователя который отправил сообщение
                    {
                        AdminHelpDeskMsg = new AdminHelpDeskMessage(Help, FollowerId);
                        await EditMessage(AdminHelpDeskMsg.BuildMsg());
                        return OkResult;
                    }

                    if (Help != null && Help.Closed == false && Help.InWork == true &&
                        Help.HelpDeskInWork.FirstOrDefault() != null && Help.HelpDeskInWork.OrderByDescending(h => h.Id).FirstOrDefault().FollowerId != FollowerId)
                    {
                        await SendMessage(
                            new BotMessage
                            {     TextMessage = "Заявка в обработке у пользователя: " + 
                                 GeneralFunction.FollowerFullName(Help.HelpDeskInWork.OrderByDescending(h => h.Id).FirstOrDefault().FollowerId)
                            });
                        return OkResult;
                    }

                    if (Help != null && Help.Closed == true)
                    {
                        AdminHelpDeskMsg = new AdminHelpDeskMessage(Help, FollowerId);
                        await EditMessage(AdminHelpDeskMsg.BuildMsg());
                        return OkResult;
                    }

                    else
                        return OkResult;
                }
            }

            catch
            {
                return NotFoundResult;
            }
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
                    string url = BotMessage.HrefUrl("https://t.me/" + follower.UserName, follower.UserName);
                    await SendMessage(new BotMessage { TextMessage = url });
                    return OkResult;
                }

                else
                    return base.OkResult;
            }
        }

        private async Task<IActionResult> SendHelpDeskAttach()
        {
            try
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    var attach_list = db.HelpDeskAttachment.Where(h => h.HelpDeskId == HelpDeskId).ToList();

                    var help = db.HelpDesk.Where(h => h.Id == HelpDeskId).FirstOrDefault();

                    if (help != null && attach_list.Count > 0)
                    {

                        HelpDeskViewAttachMessage viewAttachMessage = new HelpDeskViewAttachMessage(help, attach_list,BotInfo.Id);
                        var mess = viewAttachMessage.BuildMessage();

                        foreach (var attach in mess)
                        {

                            await SendMediaMessage(attach);
                        }
                    }

                    else
                        await AnswerCallback();

                    return OkResult;

                }
            }

            catch
            {
                return NotFoundResult;
            }
        }

        private async Task<IActionResult> AddHelpDeskAnswer()
        {
            try
            {
                int number =Convert.ToInt32(OriginalMessage.Substring(AddCommentForceReply.Length));
                using (MarketBotDbContext db=new MarketBotDbContext())
                {
                    var Help = db.HelpDesk.Where(h => h.Number == number).Include(h => h.HelpDeskInWork)
                        .Include(h=>h.HelpDeskAnswer).FirstOrDefault();

                    //Проверяем находится ли заявка в обработке у этого пользователя и не является ли она закрытой
                    if (Help != null && await Processing.CheckInWork(Help) && !await Processing.CheckIsDone(Help))
                    {
                        HelpDeskAnswer deskAnswer = new HelpDeskAnswer { HelpDeskId = Help.Id, FollowerId = FollowerId, Timestamp = DateTime.Now, Text = ReplyToMessageText };
                        db.HelpDeskAnswer.Add(deskAnswer);
                        if (db.SaveChanges() >0)
                        {
                            Help.HelpDeskAnswer.Add(deskAnswer);
                            AdminHelpDeskMsg = new AdminHelpDeskMessage(Help, FollowerId);
                            await SendMessage(AdminHelpDeskMsg.BuildMsg());
                            await Processing.NotifyChanges("К заявке №" + Help.Number.ToString() + " добавлен комментарий.Пользователь: " + GeneralFunction.FollowerFullName(FollowerId), Help.Id);
                        }
                        return OkResult;
                    }

                    else
                        return NotFoundResult;
                }
            }

            catch
            {
                return NotFoundResult;
            }
        }

        private async Task<IActionResult> CloseHelpDesk()
        {
            try
            {
                using(MarketBotDbContext db=new MarketBotDbContext())
                {
                    var Help = db.HelpDesk.Where(h => h.Id == HelpDeskId).Include(h => h.HelpDeskInWork)
                        .Include(h => h.HelpDeskAnswer).FirstOrDefault();

                    var in_work = Help.HelpDeskInWork.OrderByDescending(h => h.Id).FirstOrDefault();

                    //Проверяем находится ли заявка в обработке у этого пользователя и не является ли она закрытой
                    if (Help != null && await Processing.CheckInWork(Help) && !await Processing.CheckIsDone(Help))
                    {
                        
                        int Save = 0; // 1- данные сохранены.  0-нет
                         
                        //ищем последний комментарий пользователя где поле Closed равно НУЛЛ. 
                        //Т.е мы хотим к изменить этот коммент добавив инф. о том что пользователь заявку закрывает
                        var answer = Help.HelpDeskAnswer.Where(h => h.FollowerId == FollowerId && h.Closed == null).OrderByDescending(h=>h.Id).FirstOrDefault();

                        if (answer != null)
                        {
                            Help.Closed = true;
                            answer.Closed = true;
                            answer.ClosedTimestamp = DateTime.Now;
                            Save = db.SaveChanges();
                            FreeHelpDeskDb(Help, in_work);                         
                        }

                        /// ситуация когда Добавлял комментарии один сотурдник, А потом этоу заявку "Отжал" и Взял себе другой,
                        /// что бы просто закрыть ее без добавления комментариеВ
                        else
                        {
                            answer = new HelpDeskAnswer { Text=String.Empty,FollowerId = FollowerId, HelpDeskId = Help.Id, Timestamp = DateTime.Now, ClosedTimestamp = DateTime.Now, Closed = true };
                            db.HelpDeskAnswer.Add(answer);
                            Help.Closed = true;
                            Save = db.SaveChanges();
                            // освобождаем заявку
                            FreeHelpDeskDb(Help, in_work);

                            
                            if (Save>0)
                                Help.HelpDeskAnswer.Add(answer);
                        }

                        if (Save > 0) // Если новые данные сохранилсь в БД то создаем сообещние и отправляем
                        {
                            AdminHelpDeskMsg = new AdminHelpDeskMessage(Help, FollowerId);
                            await EditMessage(AdminHelpDeskMsg.BuildMsg());
                            await Processing.NotifyChanges("Заявка №" + Help.Number.ToString() + " закрыта. Пользователь: " + GeneralFunction.FollowerFullName(FollowerId), Help.Id);
                            //Отправляем сообщение пользователю о том что его заявка закрыта
                            long ChatId = db.Follower.Where(f => f.Id == Help.FollowerId).FirstOrDefault().ChatId;
                            string text = "Заявка №" + Help.Number.ToString() + " Закрыта"+BotMessage.NewLine()+"Комментарий:"+answer.Text;
                            await SendMessage(ChatId, new BotMessage { TextMessage = text });
                        }

                        return OkResult;
                    }


                    else
                        return NotFoundResult;
                }
            }

            catch
            {
                return NotFoundResult;
            }
        }

        private async Task<IActionResult> FreeHelpDesk()
        {
            try
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    var Help = db.HelpDesk.Where(h => h.Id == HelpDeskId).Include(h => h.HelpDeskInWork)
                        .Include(h => h.HelpDeskAnswer).FirstOrDefault();

                    var in_work = Help.HelpDeskInWork.OrderByDescending(h => h.Id).FirstOrDefault();

                    if (!await Processing.CheckIsDone(Help) && await Processing.CheckInWork(Help))
                    {
                        Help.InWork = false;
                        int a= db.SaveChanges();
                        AdminHelpDeskMsg = new AdminHelpDeskMessage(Help, FollowerId);
                        await EditMessage(AdminHelpDeskMsg.BuildMsg());
                        await Processing.NotifyChanges("Пользователь: " + GeneralFunction.FollowerFullName(FollowerId) +" освободил заявку №" + Help.Number.ToString(), Help.Id);
                        return OkResult;
                    }

                    else
                        return NotFoundResult;
                            
                }
                }

            catch
            {
                return NotFoundResult;
            }
        }

        private int FreeHelpDeskDb(HelpDesk help, HelpDeskInWork in_work)
        {
            try
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    if (help != null && in_work != null)
                    {
                        // освобождаем заявку
                        help.InWork = false;
                        //HelpDeskInWork inWork = new HelpDeskInWork { FollowerId = in_work.FollowerId, HelpDeskId = in_work.HelpDeskId, InWork = false, Timestamp = DateTime.Now };                        
                       // db.HelpDeskInWork.Add(inWork);
                        int save= db.SaveChanges();
                        return save;
                    }

                    else return -1;
                }
            }

            catch (Exception exp)
            {
                return -1;
            }
        }

      
    }
}
