using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot.BusinessLayer
{
    public class CurrencyFunction
    {
        public static List<Currency> CurrencyList()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Currency.ToList();
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
