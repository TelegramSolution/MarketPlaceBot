using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class BotInfo
    {
        public BotInfo()
        {
            AttachmentTelegram = new HashSet<AttachmentTelegram>();
            Basket = new HashSet<Basket>();
            HelpDesk = new HashSet<HelpDesk>();
            OrderTemp = new HashSet<OrderTemp>();
            Orders = new HashSet<Orders>();
            TelegramMessage = new HashSet<TelegramMessage>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? ChatId { get; set; }
        public string Token { get; set; }
        public DateTime? Timestamp { get; set; }
        public int? OwnerChatId { get; set; }
        public string WebHookUrl { get; set; }
        public bool ServerVersion { get; set; }

        public bool HomeVersion { get; set; }

        public Configuration Configuration { get; set; }
        public ICollection<AttachmentTelegram> AttachmentTelegram { get; set; }
        public ICollection<Basket> Basket { get; set; }
        public ICollection<HelpDesk> HelpDesk { get; set; }
        public ICollection<OrderTemp> OrderTemp { get; set; }
        public ICollection<Orders> Orders { get; set; }
        public ICollection<TelegramMessage> TelegramMessage { get; set; }
    }
}
