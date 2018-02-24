using System;
using System.Collections.Generic;
using System.Text;

namespace MyTelegramBot.Services.BitCoinCore
{
    public class ResultResponse
    {
        public string result { get; set; }
        public object error { get; set; }
        public string id { get; set; }
    }
}
