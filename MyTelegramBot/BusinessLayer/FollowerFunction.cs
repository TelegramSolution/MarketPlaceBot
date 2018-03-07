using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.BusinessLayer
{
    public class FollowerFunction
    {
        public static bool ExistTelephone(int FollowerId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var follower = db.Follower.Find(FollowerId);

                if (follower != null && follower.Telephone != null && follower.Telephone != "")
                    return true;

                else
                    return false;
            }

            catch
            {
                return false;
            }

            finally
            {
                db.Dispose();
            }
        }

        public static bool ExistUserName(int FollowerId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var follower = db.Follower.Find(FollowerId);

                if (follower != null && follower.UserName != null && follower.UserName != "")
                    return true;

                else
                    return false;
            }

            catch
            {
                return false;
            }

            finally
            {
                db.Dispose();
            }
        }

        public static Follower AddTelephoneNumber(int FollowerId, string TelephoneNumber)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var follower = db.Follower.Find(FollowerId);

                if (follower != null && follower.Telephone == null ||  follower.Telephone == "")
                {
                    follower.Telephone = TelephoneNumber;
                    db.Update<Follower>(follower);
                    db.SaveChanges();
                    return follower;
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

        public static Follower AddUserName(int FollowerId, string UserName)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var follower = db.Follower.Find(FollowerId);

                if (follower != null && follower.UserName == null || follower.UserName == "")
                {
                    follower.Telephone = UserName;
                    db.Update<Follower>(follower);
                    db.SaveChanges();
                    return follower;
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

        /// <summary>
        /// Проверяем заблокирован ли юзер
        /// </summary>
        /// <param name="FollowerId"></param>
        /// <returns></returns>
        public static bool IsBlocked(int FollowerId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var follower = db.Follower.Find(FollowerId);

                if (follower!=null)
                    return follower.Blocked;

                else
                    return false;
            }

            catch
            {
                return false;
            }

            finally
            {
                db.Dispose();
            }
        }
    }
}
