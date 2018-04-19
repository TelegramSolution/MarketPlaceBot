using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Telegram.Bot;

namespace MyTelegramBot.Bot
{
    public static class GeneralFunction
    {
        public static string GetBotName()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("name.json");
            return builder.Build().GetSection("name").Value;
        }

        public static string BuildPaymentComment(string BotName, string OrderNumber)
        {
            byte[] name = Encoding.ASCII.GetBytes(BotName);
            byte[] number = Encoding.ASCII.GetBytes(OrderNumber);

            byte[] data = new byte[name.Length + number.Length];

            for (int i = 0; i < name.Length; i++)
                data[i] = name[i];

            for (int i = 0; i < number.Length; i++)
                data[name.Length + i] = number[i];

            System.Security.Cryptography.SHA1Managed sHA1 = new System.Security.Cryptography.SHA1Managed();
            string hash= Convert.ToBase64String(sHA1.ComputeHash(data));

            return hash.Replace('+', '0').Replace('=','1').Replace('#','2').Replace('/','3');

        }

        public static string GenerateHash()
        {
            try
            {
                Random random = new Random();
                long nonce = random.Next(1000000, 9000000);
                long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                System.Security.Cryptography.SHA1Managed sHA1 = new System.Security.Cryptography.SHA1Managed();
                string hash = Convert.ToBase64String(sHA1.ComputeHash(BitConverter.GetBytes(nonce)));
                return hash.Replace('+', '0').Replace('=', '1').Replace('#', '2').Replace('/', '3').ToUpper();
            }

            catch
            {
                return null;
            }
        }

        public static Stream ReadFile(string filename)
        {
            try
            {
                var file = File.OpenRead(filename);

                return file;
            }

            catch
            {
                return null;
            }
        }

        public static string FollowerFullName(int? FollowerId)
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                var follower = db.Follower.Where(f => f.Id == FollowerId).FirstOrDefault();

                if (follower != null)
                {
                    string name ="";

                    if(follower.FirstName!=null)
                         name = follower.FirstName;

                    if (follower.LastName != null && follower.LastName != "")
                        name += " " + follower.LastName;

                    return name;
                }

                else
                {
                    return String.Empty;
                }
            }
        }

        public static string FollowerFullName(Follower follower)
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                if (follower != null)
                {
                    string name = "";

                    if (follower.FirstName != null)
                        name = follower.FirstName;

                    if (follower.LastName != null && follower.LastName != "")
                        name += " " + follower.LastName;

                    return name;
                }

                else
                {
                    return String.Empty;
                }
            }
        }

        public static bool CheckAvailableCity(string name)
        {
            try
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    var city_list = db.AvailableСities.ToList();
                    var city = db.AvailableСities.Where(c => c.CityName == name).FirstOrDefault();

                    if (city != null || city_list.Count==0)
                        return true;

                    else
                        return false;
                }
            }

            catch
            {
                return false;
            }
        }

        public static string AvailableCityList()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                var cities= db.AvailableСities.ToList();

                string value = "";

                foreach(AvailableСities ac in cities)
                {
                    value += ac.CityName + ",";
                }

                return value.Substring(0,value.Length-1);
            }
        }

        public static BotInfo GetBotInfo()
        {
            try
            {
                string name = GetBotName();

                BotInfo bot = new BotInfo();
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    bot = db.BotInfo.Where(b => b.Name == name).Include(b => b.Configuration).FirstOrDefault();

                    return db.BotInfo.FirstOrDefault();

                }
            }

            catch (Exception e)
            {
                return null;
            }
        }

        public async static Task<Telegram.Bot.Types.Message> SendTextMsgToOwner(string Text)
        {
            try
            {
                var BotInfo = GetBotInfo();

                TelegramBotClient telegramBotClient = new TelegramBotClient(BotInfo.Token);

                return await telegramBotClient.SendTextMessageAsync(BotInfo.OwnerChatId, Text, Telegram.Bot.Types.Enums.ParseMode.Html);
            }

            catch
            {
                return null;
            }
        }
    }
}
