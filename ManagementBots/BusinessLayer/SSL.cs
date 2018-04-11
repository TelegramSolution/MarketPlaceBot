using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace ManagementBots.BusinessLayer
{
    public class SSL
    {
        public static string GenerateSSL(string Path, string DomainName)
        {
            //папка где будет хранится сертификат
            string CertDirectory = Path + "\\" + DomainName+"\\";

            //путь до файл openssl.exe
            string openssl = Directory.GetCurrentDirectory() + "\\Files\\OpenSsl\\bin\\openssl.exe";

            //путь где будет хранится бат файл
            string bat = Directory.GetCurrentDirectory() + "\\Files\\OpenSsl\\bin\\ssl.bat";

            //путь где будет хранится приватный ключ
            string privateCert = CertDirectory + DomainName + ".key";

            //путь где будет хранится публичный ключ
            string publicCert = CertDirectory + DomainName + ".pem";

            //аргументы для бат файла
            string cmd = String.Format("req -newkey rsa:2048 -sha256 -nodes -keyout {1} -x509 -days 365 -out {2}"
            + " -subj " + @"""/C=US/ST=Russia/L=Russia/O=Telegram Solution/CN={0}""" + " -config " +@"""openssl.cnf""", DomainName,privateCert,publicCert);

            //проверяем есть ли папка где будет хранится сертификат, если нет то создаем

            if (!System.IO.Directory.Exists(CertDirectory))
                System.IO.Directory.CreateDirectory(CertDirectory);

            //создаем батник который будет генерировать сертификат
            StreamWriter streamWriter = new StreamWriter(bat);
            streamWriter.Write(openssl + " " + cmd);
            streamWriter.Dispose();

            if (File.Exists(bat))
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = bat;
                proc.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory() + "\\Files\\OpenSsl\\bin\\";
                proc.Start();
                proc.WaitForExit();
            }

            else
                throw new Exception("Не удалось начать процесс генерации сертификата");

            if (File.Exists(privateCert) &&File.Exists(publicCert))
                return CertDirectory;

            else
                throw new Exception("Не удалось сгенерировать сертификат");
        }
    }
}
