using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class WebHookPort
    {
        /// <summary>
        ///Порты поддерживаемые серверами телеграм 80,443,88,8443
        /// </summary>
        public WebHookPort()
        {
            WebHookUrl = new HashSet<WebHookUrl>();
        }

        public int Id { get; set; }
        public int? PortNumber { get; set; }
        public bool Enable { get; set; }

        public ICollection<WebHookUrl> WebHookUrl { get; set; }
    }
}
