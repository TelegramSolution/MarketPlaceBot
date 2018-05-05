using System;
using System.Collections.Generic;
using System.IO;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DomainName">например mydomain.com</param>
        /// <param name="ProxyPass">куда перенаправлять например http://192.168.1.4:8086</param>
        /// <param name="PortListen">Порт для прослушки</param>
        /// <returns></returns>
        public bool CreateConfigFile(string DomainName,string ProxyPass, int PortListen = 443)
        {
            string file_name = PortListen + DomainName + BusinessLayer.GeneralFunction.UnixTimeNow().ToString();

           string cfg= "server {listen "+ PortListen.ToString()+ ";server_name "+DomainName+";access_log /var/log/nginx/"+DomainName+".log;ssl on;ssl_protocols SSLv3 TLSv1;"
                + "ssl_certificate /etc/nginx/ssl/"+DomainName+".pem;ssl_certificate_key /etc/nginx/ssl/"+DomainName+".key;"
                + "location / {proxy_pass " + ProxyPass + ";}}";



            BusinessLayer.SshFunction ssh = new BusinessLayer.SshFunction(Ip, CertPath, UserName);

            ssh.SftpConnectToServer();

            ssh.SCPFile(WriteFile(Directory.GetCurrentDirectory()+"\\Files\\" +file_name, cfg), "/etc/nginx/sites-available/" +file_name);

            ssh.Disconnect();


            ssh.SshConnectToServer();

            ssh.Command(String.Format("ln -s /etc/nginx/sites-available/{0} /etc/nginx/sites-enabled/{0}", file_name));

            ssh.Command("sudo service nginx configtest");

            ssh.Command("sudo nginx -s reload");

            ssh.Disconnect();

            return true;

        }

        private Stream WriteFile(string path, string text)
        {

            using(StreamWriter sw=new StreamWriter(path))
            {
                sw.Write(text);
                sw.Flush();
            }

            return System.IO.File.OpenRead(path);
        }
    }
}
