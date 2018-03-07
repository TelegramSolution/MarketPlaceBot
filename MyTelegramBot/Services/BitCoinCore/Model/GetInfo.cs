using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot.Services.BitCoinCore
{
    public class GetInfo
    {
        public GetInfoResult result { get; set; }
        public object error { get; set; }
        public string id { get; set; }
    }

    public class GetInfoResult
    {
        public int version { get; set; }
        public int protocolversion { get; set; }
        public int walletversion { get; set; }
        public double balance { get; set; }
        public int blocks { get; set; }
        public int timeoffset { get; set; }
        public int connections { get; set; }
        public string proxy { get; set; }
        public double difficulty { get; set; }
        public bool testnet { get; set; }
        public int keypoololdest { get; set; }
        public int keypoolsize { get; set; }
        public double paytxfee { get; set; }
        public double relayfee { get; set; }

        public string errors { get; set; }
    }
}
