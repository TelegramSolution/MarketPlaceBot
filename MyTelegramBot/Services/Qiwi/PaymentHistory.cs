using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot.Services.Qiwi
{
    public class PaymentHistory
    {
        public class Sum
        {
            /// <summary>
            /// 
            /// </summary>
            public double amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int currency { get; set; }
        }

        public class Commission
        {
            /// <summary>
            /// 
            /// </summary>
            public double amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int currency { get; set; }
        }

        public class Total
        {
            /// <summary>
            /// 
            /// </summary>
            public double amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int currency { get; set; }
        }

        public class ExtrasItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string key { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string value { get; set; }
        }

        public class Provider
        {
            /// <summary>
            /// 
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string shortName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string longName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string logoUrl { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string description { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string keys { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string siteUrl { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<ExtrasItem> extras { get; set; }
        }

        public class DataItem
        {
            /// <summary>
            /// 
            /// </summary>
            public long txnId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public long personId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string date { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int errorCode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string error { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string statusText { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string trmTxnId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string account { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Sum sum { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Commission commission { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Total total { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Provider provider { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string source { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string comment { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int currencyRate { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> extras { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string chequeReady { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string bankDocumentAvailable { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string bankDocumentReady { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string repeatPaymentEnabled { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string favoritePaymentEnabled { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string regularPaymentEnabled { get; set; }
        }

            /// <summary>
            /// 
            /// </summary>
            public List<DataItem> data { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string nextTxnId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string nextTxnDate { get; set; }
        
    }
}
