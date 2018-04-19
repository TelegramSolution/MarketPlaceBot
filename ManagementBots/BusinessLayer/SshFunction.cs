using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SshNet;
using Renci.SshNet;
using System.IO;

namespace ManagementBots.BusinessLayer
{
    public class SshFunction
    {
        private string UserName { get; set; }

        private string Host { get; set; }

        /// <summary>
        /// путь до сертификата с помощью которого будем подключаться к серверу
        /// </summary>
        private string CertPath { get; set; }

        private string PassPhrase { get; set; }

        private PrivateKeyFile PrivateKeyFile { get; set; }

        private SftpClient client { get; set; }

        public SshFunction(string Host,string CertPath, string UserName = "root", string PassPhrase="")
        {
            this.Host = Host;
            this.CertPath = CertPath;
            this.UserName = UserName;
            this.PassPhrase = PassPhrase;
        }
        
        public bool SCPFile(Stream input,string PathDst)
        {

            if (client.IsConnected)
            {
                client.UploadFile(input, PathDst, null);

                bool result = client.Exists(PathDst);

                return result;
            }

            else
                throw new Exception("Не удалось подключиться к " + Host); 

        }

        public void ConnectToServer()
        {
            client = new SftpClient(GetConnectionInfo());

            client.Connect();
        }

        public void Disconnect()
        {
            client.Disconnect();

            client.Dispose();
        }

        private ConnectionInfo GetConnectionInfo()
        {
            var CertFile = File.Open(CertPath, FileMode.Open);

            if (PassPhrase != null && PassPhrase != "")
                PrivateKeyFile = new PrivateKeyFile(CertFile, PassPhrase);

            else
                PrivateKeyFile = new PrivateKeyFile(CertFile);

            var authenticationMethod = new PrivateKeyAuthenticationMethod(UserName, PrivateKeyFile);

            var connectionInfo = new ConnectionInfo(Host, UserName, authenticationMethod);

            return connectionInfo;
        }
    }
}
