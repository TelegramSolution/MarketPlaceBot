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

        /// <summary>
        /// Список операторов системы
        /// </summary>
        /// <returns></returns>
        public static List<Admin> GetOperatorList()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Admin.Where(a => a.Enable).Include(a => a.Follower).ToList(); 
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

        public static Configuration TelephoneVerify(Configuration configuration)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                if(configuration!=null && configuration.VerifyTelephone)
                {
                    configuration.VerifyTelephone = false;

                    db.Update<Configuration>(configuration);

                    db.SaveChanges();

                    return configuration;
                }

                if (configuration != null && !configuration.VerifyTelephone)
                {
                    configuration.VerifyTelephone = true;

                    db.Update<Configuration>(configuration);

                    db.SaveChanges();

                    return configuration;
                }

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
