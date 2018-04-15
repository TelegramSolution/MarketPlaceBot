using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class WebHookUrl
    {
        public WebHookUrl()
        {
            Bot = new HashSet<Bot>();
        }

        public int Id { get; set; }
        public int? DnsId { get; set; }
        public int? PortId { get; set; }
        public bool IsFree { get; set; }
        public string Controller { get; set; }

        public Dns Dns { get; set; }
        public WebHookPort Port { get; set; }
        public WebHookUrlHistory WebHookUrlHistory { get; set; }
        public ICollection<Bot> Bot { get; set; }

        public override string ToString()
        {
            if (Dns != null && Port != null)
                return "https://" + Dns.Name + ":" + Port.PortNumber.ToString() + "/" +Controller;

            else
                return "";
        }


    }
}
