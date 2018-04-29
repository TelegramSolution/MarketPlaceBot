using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementBots.MyExeption
{
    public class BotIsLaunchedExeption:Exception
    {
         public BotIsLaunchedExeption() : base("Боту уже запущен!")
        {

        }
    }
}
