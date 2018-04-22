using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using ManagementBots.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

namespace ManagementBots.Db
{
    public partial class WebApp
    {
        public WebApp()
        {
            Bot = new HashSet<Bot>();
            WebAppHistory = new HashSet<WebAppHistory>();
        }

        public int Id { get; set; }
        public int? ServerWebAppId { get; set; }
        public string Port { get; set; }
        public bool Enable { get; set; }
        public bool IsFree { get; set; }

        public ServerWebApp ServerWebApp { get; set; }
        public ReserveWebApp ReserveWebApp { get; set; }
        public ICollection<Bot> Bot { get; set; }
        public ICollection<WebAppHistory> WebAppHistory { get; set; }

        public override string ToString()
        {
            if (ServerWebApp != null)
                return "http://" + ServerWebApp.Ip + ":" + Port;

            else
                return "";
        }

        public HostInfo GetInfo()
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp(this.ToString() + "//HostingVersion//GetInfo");

            var response= webRequest.GetResponse().GetResponseStream();

            using (StreamReader s = new StreamReader(response))
            {
                return JsonConvert.DeserializeObject<Models.HostInfo>(s.ReadToEnd());
            }
        }

        public async Task<string> Install (HostInfo hostInfo)
        {

            var body = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(hostInfo));
            var request = (HttpWebRequest)WebRequest.Create(this.ToString() + "//HostingVersion//Install//");

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = body.Length;

            using (Stream stream =await request.GetRequestStreamAsync())
            {
                stream.Write(body, 0, body.Length);
                stream.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }


            else
                throw new Exception(response.StatusDescription);
        }

        public Models.HostInfo Unistall()
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp(this.ToString() + "//HostingVersion//Unistall");

            var response = webRequest.GetResponse().GetResponseStream();

            return GetInfo();
        }
    }
}

