using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.BusinessLayer
{
    public class FollowerFunction
    {
        public static async Task<Follower> InsertFollower(int ChatId, string FirstName, string LastName="", string UserName = "")
        {
            MarketBotDbContext db = new MarketBotDbContext();

            var follower =await db.Follower.Where(f => f.ChatId == ChatId).FirstOrDefaultAsync();

            try
            {
                if (follower == null)
                {
                    Follower Follower = new Follower
                    {
                        ChatId = ChatId,
                        FirstName = FirstName,
                        LastName = LastName,
                        UserName = UserName,
                        Blocked = false,
                        DateAdd = DateTime.Now
                    };

                    db.Follower.Add(Follower);
                    db.SaveChanges();
                    return Follower;
                }

                //пользвоатель мог изменить свои данны. Проверяем и обновляем
                if(follower!=null &&follower.UserName!=UserName ||
                    follower != null && follower.FirstName != FirstName ||
                    follower != null && follower.LastName != LastName)
                {
                    follower.UserName = UserName;
                    follower.LastName = LastName;
                    follower.FirstName = FirstName;
                    db.Update<Follower>(follower);
                    await db.SaveChangesAsync();
                    return follower;
                }

                else
                    return follower;
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

        public static Follower GetFollower(int FollowerId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var follower = db.Follower.Find(FollowerId);

                return follower;
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
        /// заблокировать
        /// </summary>
        /// <param name="FollowerId"></param>
        /// <returns></returns>
        public static Follower Block(int FollowerId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var follower= db.Follower.Find(FollowerId);

                if (follower != null && follower.Blocked==false)
                {
                    follower.Blocked = true;
                    db.SaveChanges();
                }

                return follower;
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
        /// Разблокировать
        /// </summary>
        /// <param name="FollowerId"></param>
        /// <returns></returns>
        public static Follower UnBlock(int FollowerId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var follower = db.Follower.Find(FollowerId);

                if (follower != null && follower.Blocked)
                {
                    follower.Blocked = false;
                    db.SaveChanges();
                }

                return follower;
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


        public static List<Follower> GetFollowerList()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Follower.ToList();
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
        /// Все адреса пользователя
        /// </summary>
        /// <param name="FollowerId"></param>
        /// <returns></returns>
        public static List<Orders> FollowerOrder(int FollowerId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Orders.Where(o => o.FollowerId == FollowerId).Include(o => o.CurrentStatusNavigation.Status).OrderByDescending(o => o.Id).Take(20).ToList();
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

        public static List<Address> FollowerAddress(int FollowerId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var list= db.Address.Where(o => o.FollowerId == FollowerId).Include(o => o.OrderAddress).ToList();

                List<Address> addressList = new List<Address>();

                foreach(var address in list)
                {
                    address.House = db.House.Where(h => h.Id == address.Id).Include(h => h.Street.City).FirstOrDefault();

                    if (address.House != null && address.House.Street != null)
                        addressList.Add(address);
                }


                return addressList;
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
