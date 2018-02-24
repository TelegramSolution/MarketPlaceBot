using Microsoft.AspNetCore.Authorization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    namespace MyTelegramBot
    {
    /// <summary>
    /// Белый список ip адресов с которыми работает приложение. Это localhost и ip адреса серверов Телеграма
    /// </summary>
        public class AdminWhiteListMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<AdminWhiteListMiddleware> _logger;

            public AdminWhiteListMiddleware(RequestDelegate next, ILogger<AdminWhiteListMiddleware> logger)
            {
                _next = next;
                _logger = logger;
            }

        public async Task Invoke(HttpContext context)
        {

            var remoteIp = context.Connection.RemoteIpAddress;

            string Ip =remoteIp.ToString();

            //telegram 149.154.167.197 - 149.154.167.233

            if (Ip != "::1" && Ip!="127.0.0.1" && TelegramIpCheck(Ip)==false)
                    {
                        _logger.LogInformation($"Forbidden Request from Remote IP address: {remoteIp}");
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return;
                    }
                
    
                await _next.Invoke(context);

            }

        private bool TelegramIpCheck(string Ip)
        {
            string[] spl = Ip.Split('.');
            //telegram 149.154.167.197 - 149.154.167.233
            if (spl != null && spl.Length == 4)
            {
                if ( spl[0] == "149" && spl[1] == "154" && spl[2] == "167" &&
                    Convert.ToInt32(spl[3]) >= 197 && Convert.ToInt32(spl[3]) <= 233)
                    return true;

                else
                    return false;
            }

            else
                return false;
        }
        }
    }

