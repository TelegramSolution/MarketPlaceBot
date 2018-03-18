using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot.BusinessLayer
{
    public class RequestLogFunction
    {
        public static ReportsRequestLog GetLasRecordFromFollower(int FollowerId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var log = db.ReportsRequestLog.Where(r => r.FollowerId == FollowerId).LastOrDefault();

                return log;
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

        public static ReportsRequestLog Insert(int FollowerId, DateTime dateTime)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                ReportsRequestLog log = new ReportsRequestLog
                {
                    DateAdd = dateTime,
                    FollowerId = FollowerId
                };

                db.ReportsRequestLog.Add(log);
                db.SaveChanges();
                return log;
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
