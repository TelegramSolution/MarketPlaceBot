using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.Core;
using Telegram.Bot.Types.InlineQueryResults;
using MyTelegramBot.BusinessLayer;
using Telegram.Bot.Types.InputMessageContents;


namespace MyTelegramBot.InlineResult
{
    public class AddressFollowerSearchInline: BotInline
    {
        int FollowerId { get; set; }

        public AddressFollowerSearchInline(string Query):base(Query)
        {
            try
            {
                FollowerId = Convert.ToInt32(Query);
            }

            catch
            {

            }
        }

        public override InlineQueryResult[] GetResult()
        {
            var Address = FollowerFunction.FollowerAddress(FollowerId);

            InlineQueryResultLocation[] location = new InlineQueryResultLocation[Address.Count];
            InlineQueryResult[] result = new InlineQueryResult[Address.Count];

            int i = 0;

            foreach (var adr in Address)
            {
                location[i] = new InlineQueryResultLocation();
                location[i].Id = adr.Id.ToString();
                location[i].Title = adr.House.Street.Name + " " + adr.House.Number;
                result[i] = location[i];

                if (adr.House != null && adr.House.Latitude != null && adr.House.Longitude != null)
                {
                    location[i].Longitude = Convert.ToSingle(adr.House.Longitude);
                    location[i].Latitude = Convert.ToSingle(adr.House.Latitude);
                }

                else
                {
                    location[i].Longitude =0;
                    location[i].Latitude = 0;
                }

                i++;
            }

            return result;
        }
    }
}
