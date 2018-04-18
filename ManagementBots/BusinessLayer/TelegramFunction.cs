using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementBots.BusinessLayer
{
    public class TelegramFunction
    {

        public static bool SendTextMessage(string Text, int ChatId, string BotToken)
        {
            try
            {
                return true;
            }

            catch
            {
                return false;
            }
        }
    }
}
