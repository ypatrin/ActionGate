using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Windows.Forms;

namespace SendMail
{
    class Program
    {
        static void Main(string[] args)
        {
            string logFile = String.Format("logs\\calls\\{0:dd.MM.yy}.log", DateTime.Now);

            if (File.Exists(logFile))
            {
                string logCalls = File.ReadAllText(logFile, Encoding.UTF8);
                string text = String.Format("Дата: {0:dd.MM.yy} {1}Сегодня получено от Актиона: {2} Лидов", DateTime.Now, Environment.NewLine, logCalls);

                SendMail("smtp.gmail.com", "yury.patrin@gmail.com", "ClearMeaN732", "ypatrin@mcfr.ua", "Action PhoneCalls Service", "Action PhoneCalls Service" + Environment.NewLine + Environment.NewLine + text);
                SendMail("smtp.gmail.com", "yury.patrin@gmail.com", "ClearMeaN732", "ykulgeyko@mcfr.ua", "Action PhoneCalls Service", "Action PhoneCalls Service" + Environment.NewLine + Environment.NewLine + text);
            }
        }

        /// <summary>
        /// Отправка письма на почтовый ящик
        /// </summary>
        /// <param name="smtpServer">Имя SMTP-сервера</param>
        /// <param name="from">Адрес отправителя</param>
        /// <param name="password">пароль к почтовому ящику отправителя</param>
        /// <param name="mailto">Адрес получателя</param>
        /// <param name="caption">Тема письма</param>
        /// <param name="message">Сообщение</param>
        /// <param name="attachFile">Присоединенный файл</param>
        public static void SendMail(string smtpServer, string from, string password, string mailto, string subject, string message)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from);
                mail.To.Add(new MailAddress(mailto));
                mail.Subject = subject;
                mail.Body = message;

                SmtpClient client = new SmtpClient();
                client.Host = smtpServer;
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(from, password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);

                mail.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception("SendMail: " + e.Message);
            }
        }
    }
}
