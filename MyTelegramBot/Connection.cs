using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MyTelegramBot
{
    class Connection
    {
        private static MarketBotDbContext dbContext;
        private Connection()
        {

        }

        public static MarketBotDbContext getConnection()
        {
            try
            {
                if (dbContext == null)
                {
                    dbContext = new MarketBotDbContext();

                }

                //if (dbContext != null)
                //{
                //    dbContext.Dispose();
                //    dbContext = new MarketBotDbContext();
                //}
                return dbContext;
            }

            catch
            {
                if (dbContext != null)
                    dbContext.Dispose();

                return dbContext = new MarketBotDbContext(); 
            }
        }

        public static int Save()
        {
            try
            {
                if (dbContext == null)
                    getConnection();

                return dbContext.SaveChanges();
            }

            catch (Exception exp)
            {
                dbContext.Dispose();
                return -1;
            }
        }

        public static void Close()
        {
            try
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }

            catch
            {

            }
        }
    }
}