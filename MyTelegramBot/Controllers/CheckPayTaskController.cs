using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyTelegramBot.BusinessLayer;
using MyTelegramBot.Bot.Core;
using Telegram.Bot;

namespace MyTelegramBot.Controllers
{
    /// <summary>
    /// проверка поступления новых платежей. Можно создать планироващик задач и вызывать каждые N минут
    /// </summary>
    [Produces("application/json")]
    [Route("api/CheckPayTask")]
    public class CheckPayTaskController : Controller
    {
        TelegramBotClient TelegramBotClient { get; set; }

        BotInfo BotInfo { get; set; }

        PaymentsFunction PaymentsFunction { get; set; }


        /// <summary>
        /// Проверяем поступили ли платежи по не оплаченым инойсам. Если поступили заносим инфу в бд и уведомляем всех
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Run()
        {
            try
            {
                BotInfo = Bot.GeneralFunction.GetBotInfo();

                PaymentsFunction = new PaymentsFunction();

                var InvoiceList = PaymentsFunction.FindNoPaidInvoice(); //Находим все не оплаченые чеки

                TelegramBotClient = new TelegramBotClient(BotInfo.Token);

                foreach (var invoice in InvoiceList)
                {
                    var result = await PaymentsFunction.CheckPaidInvoice(invoice);

                    if (result != null && result.Paid && result.Payment.Count > 0) // владельцу бота
                    {
                        await TelegramBotClient.SendTextMessageAsync(BotInfo.OwnerChatId, "Поступил новый платеж /payment" + result.Payment.LastOrDefault().Id.ToString());
                    }

                    if (result != null && result.Paid && result.Payment.Count > 0) // в чат
                    {
                        await TelegramBotClient.SendTextMessageAsync(BotInfo.Configuration.PrivateGroupChatId, "Поступил новый платеж /payment" + result.Payment.LastOrDefault().Id.ToString());
                    }

                }

                return Ok();
            }

            catch
            {
                return NotFound();
            }
            
        }
       
    }
}