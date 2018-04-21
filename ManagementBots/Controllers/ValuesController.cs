using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using ManagementBots.Bot;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using ManagementBots.Bot.Core;

namespace ManagementBots.Controllers
{
    /// <summary>
    /// ВЕБХУК
    /// </summary>

    [Produces("application/json")]

   
    public class ValuesController : Controller
    {
        
       
        private OkResult OkResult { get; set; }

        private ManagementBots.Bot.Core.BotCore BotCore { get; set; }

        private string ModuleName { get; set; }

        IActionResult Result { get; set; }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {

            //if (Result == null && update.CallbackQuery == null && update.InlineQuery==null)
            //{
            //    BotCore = new ProductAddBot(update);
            //    Result = await BotCore.Response();

            //}


            if (update != null && update.CallbackQuery != null && update.CallbackQuery.Data != null && update.InlineQuery == null)
            {
                ModuleName = JsonConvert.DeserializeObject<BotCommand>(update.CallbackQuery.Data).M;
                
            }

            if(update.InlineQuery == null)
            {
                if (Result == null && ModuleName != null && ModuleName == MainMenuBot.ModuleName || Result == null && ModuleName == null)
                {
                    BotCore = new MainMenuBot(update);
                    Result = await BotCore.Response();
                }

            }

            //делаем так что бы наше приложние всегда отвечало телеграму ОК. 
            //В противном случаем телеграм так и будет слать нам это сообщения в ожиданиее ответа ОК
            if (Result == null || Result != null)
                Result = Ok();


                return Result;
            }


        }
    }
