using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.BusinessLayer
{

    public class NotificationFunction
    {
        public static int RemoveNotification(int id)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var notifi = db.Notification.Find(id);

                if (notifi != null)
                {
                    db.Notification.Remove(notifi);
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
        public static List<Notification> NotificationList()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Notification.Where(n=>n.Sended).OrderByDescending(n=>n.Id).ToList();
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

        public static Notification GetNotification(int Id)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Notification.Where(n => n.Id == Id).Include(n => n.Follower).FirstOrDefault();
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

        public static Notification InsertNotification(string Text, int FollowerId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                Notification notification = new Notification
                {
                    FollowerId = FollowerId,
                    Sended = false,
                    Text = Text
                };

                db.Notification.Add(notification);
                db.SaveChanges();
                return notification;
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

        public static Notification NotificationIsSended(int Id)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var notifi = db.Notification.Find(Id);

                if (notifi != null)
                {
                    notifi.DateAdd = DateTime.Now;
                    notifi.Sended = true;
                    db.SaveChanges();
                    
                }

                return notifi;
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
