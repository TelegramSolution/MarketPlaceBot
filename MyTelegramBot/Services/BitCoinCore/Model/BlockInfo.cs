using System;
using System.Collections.Generic;
using System.Text;

namespace MyTelegramBot.Services.BitCoinCore
{
    public class Result
    {
        public string hash { get; set; }
        public int confirmations { get; set; }
        public int strippedsize { get; set; }
        public int size { get; set; }
        public int weight { get; set; }
        public int height { get; set; }
        public int version { get; set; }
        public string versionHex { get; set; }
        public string merkleroot { get; set; }
        public IList<string> tx { get; set; }
        public int time { get; set; }
        public int mediantime { get; set; }
        public long nonce { get; set; }
        public string bits { get; set; }
        public double difficulty { get; set; }
        public string chainwork { get; set; }
        public string previousblockhash { get; set; }
        public string nextblockhash { get; set; }
    }

    public class BlockInfo
    {
        public Result result { get; set; }
        public object error { get; set; }
        public string id { get; set; }
    }
}
