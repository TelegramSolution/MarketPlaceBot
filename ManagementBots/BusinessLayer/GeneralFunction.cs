using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ManagementBots.BusinessLayer
{
    public class GeneralFunction
    {

        public static long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        /// <summary>
        ///  папка в которой нужно создавать сертификат (На прокси сервере)
        /// </summary>
        /// <returns></returns>
        public static string SslPathOnProxyServer()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            return builder.Build().GetSection("CertProxyPath").Value;
        }

        public static string BotName()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            return builder.Build().GetSection("BotName").Value;
        }

        public static string SslPathOnMainServer()
        {
            return Directory.GetCurrentDirectory() + "\\Files\\Cert\\";
        }
    }
}
