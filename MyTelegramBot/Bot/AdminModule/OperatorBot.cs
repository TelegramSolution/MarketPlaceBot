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

        public const string GenerateKeyCmd = "GenerateKey";

        public const string ViewOperatosCmd = "ViewOperatos";

        public const string ModuleName = "Operators";


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

                    case "GenerateKey":
                        return await GenerateKey();

                    default:
                        return null;
                }

            }

            else
                 if (base.CommandName.Contains("/key"))
                return await CheckOperatorKey(CommandName.Substring(5));

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

                AdminFunction.RemoveOperator(id);

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
        /// Пользователй хочет получить права оператора. Проверка ключа
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private async Task<IActionResult> CheckOperatorKey(string key)
        {
            var Key = AdminFunction.FindAdminKey(key);

            if (Key != null && Key.Admin.Count == 0)
                return await AddNewOpearator(Key);

            return base.OkResult;
        }

        /// <summary>
        /// Добавить нового оператора
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        private async Task<IActionResult> AddNewOpearator(AdminKey adminKey)
        {

            var admin = AdminFunction.FindAdmin(FollowerId);

            if (admin != null)
                return await SendAdminControlPanelMsg();

            else
            {

                admin = AdminFunction.InsertAdmin(FollowerId, adminKey);

                if (admin != null)
                {
                    string meessage = "Зарегистрирован новый оператор системы: " + GeneralFunction.FollowerFullName(admin.FollowerId)
                    + BotMessage.NewLine() + "Ключ: " + adminKey.KeyValue;
                    await SendMessage(BotOwner, new BotMessage { TextMessage = meessage });
                    return await SendAdminControlPanelMsg();
                }

            }

            return OkResult;

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


        /// <summary>
        /// Генерируем новый ключ для оператора. 
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> GenerateKey()
        {
            string hash = GeneralFunction.GenerateHash();

            if (hash != null && AdminFunction.InsertAdminKey(hash) != null)
            {

                await SendMessage(
                        new BotMessage
                        {
                            TextMessage = "Пользователь который должен получить права оператора должен ввести следующую команду:"
                                                    + BotMessage.NewLine() + BotMessage.Italic("/key " + hash)
                        }
                        );


            }

            return OkResult;

        }
    }
}
