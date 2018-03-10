using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyTelegramBot.BusinessLayer;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.BusinessLayer
{
    public class FeedbackFunction
    {
        public static FeedBack EnableFeedback(int FeedBackId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var feedback = db.FeedBack.Where(f=>f.Id==FeedBackId).Include(f=>f.Product).FirstOrDefault();
                feedback.DateAdd = DateTime.Now;
                feedback.Enable = true;
                db.Update<FeedBack>(feedback);
                db.SaveChanges();
                return feedback;
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

        public static FeedBack InsertFeedBack(int Raiting, int ProductId, int OrderId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            var feed = db.FeedBack.Where(f => f.ProductId == ProductId && f.OrderId == OrderId).LastOrDefault();

            var product = db.Product.Find(ProductId);

            try
            {
                if (feed != null)
                {
                    feed.RaitingValue = Raiting;
                    feed.OrderId = OrderId;
                    db.Update<FeedBack>(feed);
                    db.SaveChanges();
                    feed.Product = product;
                    return feed;

                }

                else
                {
                    FeedBack feedBack = new FeedBack
                    {
                        OrderId = OrderId,
                        ProductId = ProductId,
                        RaitingValue = Raiting,
                        Enable = false
                    };

                    db.FeedBack.Add(feedBack);
                    db.SaveChanges();
                    feedBack.Product = product;
                    return feedBack;
                }

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

        public static FeedBack GetFeedBack(int Id)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.FeedBack.Where(f => f.Id == Id).Include(f => f.Product).FirstOrDefault();

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

        public static int RemoveFeedBack(int Id)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var feedback= db.FeedBack.Find(Id);

                db.FeedBack.Remove(feedback);

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

        /// <summary>
        /// Отзывы для заказа
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public static List<FeedBack> GetFeedBackByOrderId(int OrderId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.FeedBack.Where(f => f.OrderId == OrderId && f.Enable).Include(f=>f.Product).ToList();
            }

            catch
            {
                return null;
            }

            finally
            {

            }
        }

        public static List<FeedBack> GetFeedBackByOrderNumber(int OrderNumber)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var order = db.Orders.Where(o => o.Number == OrderNumber).Include(o => o.FeedBack).FirstOrDefault();

                return order.FeedBack.Where(f => f.Enable).ToList();
            }

            catch
            {
                return null;
            }

            finally
            {

            }
        }
    }
}
