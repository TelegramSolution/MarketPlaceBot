using System;
using System.Collections.Generic;
using System.Text;

namespace MyTelegramBot.Services.BitCoinCore
{
    class Litecoin:BitCoinCore.BitCoin
    {
        public Litecoin (string UserName, string Password, string Host = "127.0.0.1", string Port = "9332") 
            :base(UserName,Password,Host,Port)
        {

        }
    }
}
