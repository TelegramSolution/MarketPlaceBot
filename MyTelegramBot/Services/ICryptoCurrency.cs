using System;
using System.Collections.Generic;
using System.Text;

namespace MyTelegramBot.Services
{
    interface ICryptoCurrency
    {
        string GetNewAddress(string account = null);

        T GetTxnInfo<T>(string TxId);

        T GetBlockInfo<T>(string BlockHash);

        IList<string> GetAddressByAccount(string Account=null);

        double GetBalance(string Account);
    }
}
