using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.BusinessLayer
{
    public class PickUpPointFunction
    {

        public static PickupPoint EnablePickUpPoint(int Id,bool Enable)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var p = db.PickupPoint.Find(Id);

                if (p != null)
                {
                    p.Enable = Enable;
                    db.Update<PickupPoint>(p);
                    db.SaveChanges();
                    return p;
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

        public static List<PickupPoint> PickUpPointList()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.PickupPoint.ToList();
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

        public static PickupPoint InsertPickUpPoint(string Name)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var reapet = db.PickupPoint.Where(p => p.Name == Name).FirstOrDefault();

                if (reapet == null)
                {
                    PickupPoint pickupPoint = new PickupPoint
                    {
                        Enable = true,
                        Name = Name
                    };

                    db.PickupPoint.Add(pickupPoint);
                    db.SaveChanges();
                    return pickupPoint; ;
                }

                else
                    return reapet;
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
