using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.BusinessLayer
{
    public class BasketFunction
    {
        /// <summary>
        /// Посчитать стоимость корзины без учета доставки
        /// </summary>
        /// <param name="FollowerId"></param>
        /// <param name="BotId"></param>
        /// <returns></returns>
        public static double BasketTotalPrice(int FollowerId, int BotId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            var basket = db.Basket.Where(b => b.FollowerId == FollowerId && b.Enable && b.BotInfoId == BotId)
                .Include(p => p.Product.CurrentPrice).ToList();

            double total = 0.0;

            try
            {
                foreach (var position in basket)
                    total += position.Amount * position.Product.CurrentPrice.Value;

                return total;
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
        /// Общая стоимость все позиций в коризне
        /// </summary>
        /// <param name="Basket"></param>
        /// <returns></returns>
        public static double BasketTotalPrice(List<Basket> Basket)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                double total = 0.0;
                foreach (var position in Basket) // Общая стоимость корзины
                    total += position.Product.CurrentPrice.Value * position.Amount;

                return total;
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
        /// Перенести позиции из корзины в заказ
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="basket"></param>
        /// <returns></returns>
        public static List<OrderProduct> FromBasketToOrderPosition(int OrderId, List<Basket> basket)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                List<OrderProduct> list = new List<OrderProduct>();
                foreach (var position in basket)
                {
                    OrderProduct orderProduct = new OrderProduct
                    {
                        ProductId = position.ProductId,
                        OrderId = OrderId,
                        DateAdd = DateTime.Now,
                        Count = 1,
                        PriceId = Convert.ToInt32(position.Product.CurrentPriceId),

                    };

                    db.Basket.Remove(position); // удаляем из корзины

                    db.OrderProduct.Add(orderProduct); // вставляем в заказ

                    db.SaveChanges();

                    orderProduct.Price = position.Product.CurrentPrice;

                    list.Add(orderProduct);
                }

                return list;
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
        /// удалить все позиции из корзины пользвателя
        /// </summary>
        /// <param name="FollowerId"></param>
        /// <param name="BotId"></param>
        /// <returns></returns>
        public static int ClearBasket(int FollowerId, int BotId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var basket = db.Basket.Where(b => b.FollowerId == FollowerId);

                foreach (var product in basket)
                    db.Remove(product);

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
        /// Посчитвать сколько этого товара в корзине у пользователя
        /// </summary>
        /// <param name="FollowerId"></param>
        /// <param name="ProductId"></param>
        /// <param name="BotId"></param>
        /// <returns></returns>
        public static int ProductBasketCount(int FollowerId, int ProductId,  int BotId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var list = db.Basket.Where(b => b.ProductId == ProductId && b.BotInfoId == BotId 
                && b.FollowerId == FollowerId).ToList();

                return list.Count;

            }
            catch
            {
                return 0;

            }

            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Добавить одну позицию товара в корзину пользователя
        /// </summary>
        /// <param name="FollowerId"></param>
        /// <param name="ProductId"></param>
        /// <param name="BotId"></param>
        /// <returns></returns>
        public static Basket AddPositionToBasker(int FollowerId, int ProductId, int BotId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                Basket basket = new Basket
                {
                    FollowerId = FollowerId,
                    ProductId = ProductId,
                    Enable = true,
                    DateAdd = DateTime.Now,
                    Amount = 1,
                    BotInfoId = BotId
                };

                db.Basket.Add(basket);
                db.SaveChanges();
                return basket;
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
        /// Удалить товар из коризны у пользовател
        /// </summary>
        /// <param name="FollowerId"></param>
        /// <param name="ProductId"></param>
        /// <param name="BotId"></param>
        /// <returns>Вернет кол-во этого товара в корзине у польвозователя</returns>
        public static int RemovePositionFromBasket(int FollowerId, int ProductId, int BotId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var basket = db.Basket.Where(b => b.ProductId == ProductId && 
                                                  b.FollowerId == FollowerId && 
                                                  b.BotInfoId == BotId)
                                                  .LastOrDefault();
                if (basket != null)
                {
                    db.Basket.Remove(basket);
                    db.SaveChanges();
                }
                var list = db.Basket.Where(b => b.ProductId == ProductId  &&
                                                b.BotInfoId == BotId &&
                                                b.FollowerId == FollowerId).ToList();

                return list.Count;
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
            

    }
}