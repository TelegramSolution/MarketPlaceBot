using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ManagementBots.BusinessLayer
{
    public class GeneralFunction
    {

        /// <summary>
        /// Сетевая папка в которой нужно создавать сертификат (На прокси сервере)
        /// </summary>
        /// <returns></returns>
        public static string SslNetworkFolder()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            return builder.Build().GetSection("CertNetworkFolder").Value;
        }
    }
}
