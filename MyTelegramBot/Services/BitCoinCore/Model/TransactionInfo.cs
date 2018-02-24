using System;
using System.Collections.Generic;
using System.Text;

namespace MyTelegramBot.Services.BitCoinCore
{
    class TransactionInfo
    {
        public Result result { get; set; }
        public object error { get; set; }
        public string id { get; set; }

        public class Detail
        {
            public string account { get; set; }
            public string address { get; set; }
            public string category { get; set; }
            public double amount { get; set; }
            public string label { get; set; }
            public int vout { get; set; }
        }

        public class Result
        {
            public double amount { get; set; }
            public int confirmations { get; set; }
            public string blockhash { get; set; }
            public int blockindex { get; set; }
            public int blocktime { get; set; }
            public string txid { get; set; }
            public IList<object> walletconflicts { get; set; }
            public int time { get; set; }
            public int timereceived { get; set; }
            public string bip125 { get; set; }
            public IList<Detail> details { get; set; }
            public string hex { get; set; }
    }
}

    class TransactionInfoList
    {
        public IList<Listtransactions> result { get; set; }
        public object error { get; set; }
        public string id { get; set; }

    }

    class Listtransactions
    {
        public string account { get; set; }
        public string address { get; set; }
        public string category { get; set; }
        public double amount { get; set; }
        public string label { get; set; }
        public int vout { get; set; }
        public int confirmations { get; set; }
        public string blockhash { get; set; }
        public int blockindex { get; set; }
        public int blocktime { get; set; }
        public string txid { get; set; }
        public IList<object> walletconflicts { get; set; }
        public int time { get; set; }
        public int timereceived { get; set; }
    }
}
