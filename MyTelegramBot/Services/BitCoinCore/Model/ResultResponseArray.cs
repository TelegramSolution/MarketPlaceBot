using System;
using System.Collections.Generic;
using System.Text;

namespace MyTelegramBot.Services.BitCoinCore
{
    public class ResultResponseArray
    {
        public IList<string> result { get; set; }
        public object error { get; set; }
        public string id { get; set; }
    }
}
