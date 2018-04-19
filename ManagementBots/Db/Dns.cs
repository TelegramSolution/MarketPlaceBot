using ManagementBots.BusinessLayer;
using System;
using System.Collections.Generic;
using System.IO;

namespace ManagementBots.Db
{
    public partial class Dns
    {
        public Dns()
        {
            WebHookUrl = new HashSet<WebHookUrl>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public string SslPathOnProxy { get; set; }
        public DateTime? TimeStamp { get; set; }
        public bool Enable { get; set; }
        public bool IsFree { get; set; }
        public string SslPathOnMainServer { get; set; }
        public ICollection<WebHookUrl> WebHookUrl { get; set; }

        public string PublicKeyPathOnProxy()
        {

            return GeneralFunction.SslPathOnProxyServer() + Name + ".pem";
        }

        public string PublicKeyPathOnMainServer()
        {
            return Directory.GetCurrentDirectory() + "\\Files\\Cert\\"+ Name + ".pem";
        }


        public string PrivateKeyPathOnProxy()
        {
            return GeneralFunction.SslPathOnProxyServer() + Name + ".key";

        }

        /// <summary>
        /// путь до файла закрытого ключа на главном сервере
        /// </summary>
        /// <returns></returns>
        public string PrivateKeyPathOnMainServer()
        {
            return Directory.GetCurrentDirectory() + "\\Files\\Cert\\" + Name + ".key";
        }




    }
}
