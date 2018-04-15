using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class ProxyServer
    {
        public ProxyServer()
        {
            Bot = new HashSet<Bot>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public bool Enable { get; set; }
        public string CertPath { get; set; }

        public string UserName { get; set; }

        public string PassPhrase { get; set; }

        public ICollection<Bot> Bot { get; set; }
    }
}
