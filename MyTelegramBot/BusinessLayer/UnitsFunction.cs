using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot.BusinessLayer
{
    public class UnitsFunction
    {

        public static Units GetUnits(string name)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Units.Where(u => u.Name == name).FirstOrDefault();
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

        public static List<Units> UnitsList()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Units.ToList();
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
