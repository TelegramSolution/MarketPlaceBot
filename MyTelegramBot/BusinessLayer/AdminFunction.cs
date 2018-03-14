using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyTelegramBot.BusinessLayer;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.BusinessLayer
{
    public class AdminFunction
    {
        /// <summary>
        /// Обновить владельца бота
        /// </summary>
        /// <param name="BotId"></param>
        /// <param name="ChatIdOwner"></param>
        /// <returns></returns>
        public static int UpdateOwner (int BotId, int ChatIdOwner)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var Bot= db.BotInfo.Find(BotId);

                if (Bot != null)
                {
                    Bot.OwnerChatId = ChatIdOwner;
                    return db.SaveChanges();
                }

                else
                    return 0;
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

        public static int RemoveOperator(int AdminId) 
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {

                var admin= db.Admin.Find(AdminId);

                if (admin != null)
                {
                    db.Admin.Remove(admin);
                    db.SaveChanges();

                }

                return 0;
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

        public static AdminKey InsertAdminKey(string Hash, bool Enable = false)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                AdminKey key = new AdminKey { DateAdd = DateTime.Now, Enable = false, KeyValue = Hash };
                db.AdminKey.Add(key);
                db.SaveChanges();
                return key;
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

        public static AdminKey FindAdminKey(string Hash, bool Enable = false)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var AdminKey = db.AdminKey.Where(a => a.KeyValue == Hash && a.Enable == Enable).FirstOrDefault();

                return AdminKey;
            }

            catch
            {
                return null;
            }

            finally
            {

            }
        }

        public static Admin InsertAdmin(int FollowerId, AdminKey adminKey)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                Admin NewAdmin = new Admin
                {
                    FollowerId = FollowerId,
                    DateAdd = DateTime.Now,
                    AdminKeyId = adminKey.Id,
                    Enable = true,
                    NotyfiActive = true

                };

                db.Admin.Add(NewAdmin);

                adminKey.Enable = true;

                db.Update<AdminKey>(adminKey);

                db.SaveChanges();

                return NewAdmin;

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

        public static Admin FindAdmin(int FollowerId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
               return db.Admin.Where(a => a.FollowerId == FollowerId && a.Enable).Include(a => a.AdminKey).Include(a => a.Follower).FirstOrDefault();
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
