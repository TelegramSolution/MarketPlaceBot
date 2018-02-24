using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace MyTelegramBot.Services
{
    public static class CryptonatorConvert
    {
        public static Cryptonator GetCryptonator(string bases, string target)
        {
            try
            {
                WebClient client = new WebClient();
                string reply = client.DownloadString("https://api.cryptonator.com/api/ticker/"+bases+"-"+target);
                return JsonConvert.DeserializeObject<Cryptonator>(reply);
            }

            catch
            {
                return null;
            }
        }
    }

    public class Ticker
    {
        public string target { get; set; }
        public string price { get; set; }
        public string volume { get; set; }
        public string change { get; set; }
    }

    public class Cryptonator
    {
        public Ticker ticker { get; set; }
        public int timestamp { get; set; }
        public bool success { get; set; }
        public string error { get; set; }
    }
}
