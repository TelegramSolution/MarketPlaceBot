using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.Core;
using MyTelegramBot.Bot;

namespace MyTelegramBot.BusinessLayer
{
    public class ConfigurationFunction
    {
        public static int AddFileIdUserNameFaqImage(Configuration Configuration, string FileId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                Configuration.UserNameFaqFileId = FileId;
                db.Update<Configuration>(Configuration);
                return db.SaveChanges();
            }
            catch
            {
                return -1;
            }

            finally
            {
                db.Dispose();
            }
        }

        public static Currency MainCurrencyInSystem()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
               var botInfo= GeneralFunction.GetBotInfo();

                if (botInfo != null)
                    return db.Currency.Find(botInfo.Configuration.CurrencyId);

                else
                    return null;
            }

            catch
            {
                return null;
            }

            finally
            {
                db.Dispose();
            }
        }
    }
}
