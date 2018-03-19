using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.Core;
using MyTelegramBot.BusinessLayer;

namespace MyTelegramBot.Bot.AdminModule
{
    public class OperatorBot:BotCore
    {
        private const string RemoveOperatorCmd = "/removeoperator";

        public const string ViewOperatosCmd = "ViewOperatos";

        public const string ModuleName = "Operators";

        public const string AddOperatorRulesCmd = "AddOperatorRules";

        public OperatorBot(Update update):base(update)
        {

        }

        protected override void Initializer()
        {

        }

        public async override Task<IActionResult> Response()
        {
            if (IsOwner())
            {
                if (base.CommandName.Contains(RemoveOperatorCmd))
                    return await RemoveOperator();

                switch (base.CommandName)
                {
                    case ViewOperatosCmd:
                        return await SendOperatorList(MessageId);


                    case AddOperatorRulesCmd:
                        return await AddOperatorRule();

                    default:
                        return null;
                }

            }

            else
                return null;
        }

        /// <summary>
        /// удалить оператора
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> RemoveOperator()
        {
            try
            {
                int id = Convert.ToInt32(base.CommandName.Substring(RemoveOperatorCmd.Length));

                var admin = AdminFunction.GetAdmin(id);

                AdminFunction.RemoveOperator(id);
                //если есть общий чат, кикаем от туда этого оператора
                if (admin!=null && BotInfo.Configuration.PrivateGroupChatId!=null) 
                {
                   await base.KickMember(Convert.ToInt64(BotInfo.Configuration.PrivateGroupChatId), admin.Follower.ChatId);
                }

                return await SendOperatorList();

            }

            catch
            {
                return await SendOperatorList();
            }
        }

        /// <summary>
        /// Отправить сообщение со списком всех операторов
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendOperatorList(int MessageId = 0)
        {
            try
            {
                BotMessage = new OperatosListMessage();
                await SendMessage(BotMessage.BuildMsg(), MessageId);
                return OkResult;
            }

            catch
            {
                return OkResult;
            }
        }



        /// <summary>
        /// Сообщение с панелью администратора
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendAdminControlPanelMsg()
        {
            BotMessage = new ControlPanelMessage(base.FollowerId);

            await SendMessage(BotMessage.BuildMsg());

            return base.OkResult;

        }


        private async Task<IActionResult> AddOperatorRule()
        {
            int followerid = Argumetns[0];
            var admin= AdminFunction.FindAdmin(followerid);

            var followerinfo = FollowerFunction.GetFollower(followerid);

            if(followerinfo.ChatId == BotInfo.OwnerChatId)
            {
                await AnswerCallback("Владелец бота не может обладать правами оператора", true);
                return OkResult;
            }

            //даем пользователю права оператора. И если есть общий чат то даем пользователю приглашение
            if (admin == null && followerinfo!=null && followerinfo.ChatId!=BotInfo.ChatId)
            {
               var key= AdminFunction.InsertAdminKey(GeneralFunction.GenerateHash());

                admin = AdminFunction.InsertAdmin(followerid, key);

                if (admin != null && admin.Follower!=null)
                {
                    await AnswerCallback("Сохранено!", true);

                    await base.SendMessage(admin.Follower.ChatId, 
                        new BotMessage { TextMessage = "Вы получили права оператора. Нажмите сюда /admin" });

                    if (BotInfo.Configuration != null && BotInfo.Configuration.PrivateGroupChatId!=null && BotInfo.Configuration.PrivateGroupChatId != "")
                        await base.SendMessage(admin.Follower.ChatId,
                            new BotMessage {
                                TextMessage = "Что бы подключиться в общий чат, перейдите по ссылке " 
                                +await CreateInviteToGroupChat(Convert.ToInt64(BotInfo.Configuration.PrivateGroupChatId),admin.Follower.ChatId)
                             });
                }


                return OkResult;
            }


            if (admin != null)
            {
                await AnswerCallback("Этот пользователь уже обладает правами оператора", true);
                return OkResult;
            }

            else
                return OkResult;
        }
    }
}
