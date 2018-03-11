using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.BusinessLayer
{
    public class CategoryFunction
    {
        public static Category GetCategory(string name)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Category.Where(c => c.Name == name).LastOrDefault();
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

        public static List<Category> GetListCategory()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Category.ToList(); 
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

        public static Category InsertCategory(string name)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                if (db.Category.Where(c => c.Name == name).FirstOrDefault() != null)
                {
                    Category category = new Category
                    {
                        Name = name,
                        Enable = true
                    };

                    db.Category.Add(category);
                    db.SaveChanges();
                    return category;
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
    }
}
