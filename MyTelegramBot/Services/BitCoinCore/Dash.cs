using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot.Services.BitCoinCore
{
    class Dash:BitCoin
    {
        public Dash(string UserName,string Password, string Host= "127.0.0.1", string Port= "9999") : base(UserName,Password,Host,Port)
        {

        }
    }
}
