using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.ServiceModel;
using action_gw.Properties;
using System.Drawing;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using action_gw.ProxyCrm;

namespace action_gw
{
    class Request
    {
        private string Method;
        private string[] RequestArr;
        private string data_type = "xml";
        public string ContentType = "xml";
        private Log Log = Log.getInstance();

        HttpListenerRequest r;

        public byte[] WebRequest(HttpListenerRequest request)
        {
            if (request.RawUrl == "" || request.RawUrl == "/")
            {
                return sendAnswer("<error>Method does not support</error>");
            }

            r = request;

            string[] Req = request.RawUrl.Split("?".ToCharArray());
            Req[0] = Req[0].Replace("/", "");

            Method = Req[0];
            int count = Req.Length;
            if (count > 1)
            {
                RequestArr = Req[1].Split("&".ToCharArray());
            }

            Log.add(string.Format("Получен http запрос метода {0}", Method));

            string xml = "";

            try
            {
                if (Method == "RegisterLicense.xml")
                {
                    this.data_type = "xml";
                    return sendAnswer(RegisterLicense());
                }
                if (Method == "favicon.ico")
                {
                    return sendAnswer("");
                }
                else
                {
                    xml = "<error>Method does not support</error>";
                    Log.add(string.Format("Метод {0} не поддерживается", Method));
                }
            }
            catch(Exception e)
            {
                Log.add(String.Format("[ERR] Системная ошибка: {0}", e.Message));
                return sendAnswer("<error>System error</error>");
            }

            return sendAnswer(xml);
        }

        private string RegisterLicense()
        {
            try
            {
                string PartnerNumber = "976625";
                string SellerLogin = @"ACTION-CRM\MCFR_Ukraine";
                string ProductNumber = r.QueryString["ProductNumber"];
                string PriceListNumber = r.QueryString["PriceListNumber"];
                DateTime ServiceActivateOn = DateTime.Now;
                DateTime ServiceExpiresOn = DateTime.Now;
                DateTime DateConnectionNow = DateTime.Now;
                string ContactEmail = r.QueryString["ContactEmail"];
                string ContactPin = r.QueryString["ContactPin"];
                string ContactLastName = r.QueryString["ContactLastName"];
                string ContactFirstName = r.QueryString["ContactFirstName"];
                string ContactMiddleName = r.QueryString["ContactMiddleName"];
                string ContactPhone = r.QueryString["ContactPhone"];
                string ContactPhoneCode = r.QueryString["ContactPhoneCode"];
                string AccountPin = r.QueryString["AccountPin"];
                string AccountInn = r.QueryString["AccountInn"];
                string AccountKpp = r.QueryString["AccountKpp"];
                string AccountName = r.QueryString["AccountName"];
                int RegCodeUkr = 0;

                string xml = "";
                DateTime dateValue;
                int intValue;

                //check int

                if (int.TryParse(r.QueryString["RegCodeUkr"], out intValue))
                    RegCodeUkr = int.Parse(r.QueryString["RegCodeUkr"]);

                //check dates

                if (DateTime.TryParse(r.QueryString["ServiceActivateOn"], out dateValue))
                    ServiceActivateOn = DateTime.Parse(r.QueryString["ServiceActivateOn"]);
                else
                    return "<error>Invalid ServiceActivateOn parameter</error>";

                if (DateTime.TryParse(r.QueryString["ServiceExpiresOn"], out dateValue))
                    ServiceExpiresOn = DateTime.Parse(r.QueryString["ServiceExpiresOn"]);
                else
                    return "<error>Invalid ServiceExpiresOn parameter</error>";

                if (DateTime.TryParse(r.QueryString["DateConnectionNow"], out dateValue))
                    DateConnectionNow = DateTime.Parse(r.QueryString["DateConnectionNow"]);
                else
                    DateConnectionNow = ServiceActivateOn;

                //encode xml to utf8
                
                byte[] bytes_1 = Encoding.Default.GetBytes(ProductNumber);
                ProductNumber = Encoding.UTF8.GetString(bytes_1);

                byte[] bytes_2 = Encoding.Default.GetBytes(PriceListNumber);
                PriceListNumber = Encoding.UTF8.GetString(bytes_2);

                byte[] bytes_3 = Encoding.Default.GetBytes(ContactEmail);
                ContactEmail = Encoding.UTF8.GetString(bytes_3);

                byte[] bytes_4 = Encoding.Default.GetBytes(ContactFirstName);
                ContactFirstName = Encoding.UTF8.GetString(bytes_4);

                byte[] bytes_5 = Encoding.Default.GetBytes(ContactLastName);
                ContactLastName = Encoding.UTF8.GetString(bytes_5);

                byte[] bytes_6 = Encoding.Default.GetBytes(ContactMiddleName);
                ContactMiddleName = Encoding.UTF8.GetString(bytes_6);

                byte[] bytes_7 = Encoding.Default.GetBytes(AccountName);
                AccountName = Encoding.UTF8.GetString(bytes_7);

                //answer

                Log.add("Отправка запроса");

                PublicCrmProxyClient client = new PublicCrmProxyClient();
                RegisterLicenseDto registerLicenseDto = new RegisterLicenseDto();

                registerLicenseDto.PartnerNumber = PartnerNumber;
                registerLicenseDto.SellerLogin = SellerLogin;

                registerLicenseDto.ProductNumber = ProductNumber;
                registerLicenseDto.PriceListNumber = PriceListNumber;

                registerLicenseDto.DateСonnectionNow = DateConnectionNow;
                registerLicenseDto.ServiceActivateOn = ServiceActivateOn;
                registerLicenseDto.ServiceExpiresOn = ServiceExpiresOn;

                registerLicenseDto.ContactEmail = ContactEmail;
                registerLicenseDto.ContactFirstName = ContactFirstName;
                registerLicenseDto.ContactLastName = ContactLastName;
                registerLicenseDto.ContactMiddleName = ContactMiddleName;
                registerLicenseDto.ContactPhone = ContactPhone;
                registerLicenseDto.ContactPhoneCode = ContactPhoneCode;
                registerLicenseDto.ContactPin = ContactPin;

                registerLicenseDto.AccountInn = AccountInn;
                registerLicenseDto.AccountKpp = AccountKpp;
                registerLicenseDto.AccountName = AccountName;
                registerLicenseDto.AccountPin = AccountPin;
                
                registerLicenseDto.RegCodeUkr = RegCodeUkr;

                RegisterLicenseDto result = client.RegisterLicense(registerLicenseDto);
                string AuthCode = result.AuthCode;
                Guid LicenseId = result.LicenseId;
                String ErrorMessage = result.ErrorMessage;
                int LicenseCountBySalesContactByProduct = result.LicenseCountBySalesContactByProduct;

                Log.add("Ответ получен. Формируем xml");

                if (String.IsNullOrEmpty(ErrorMessage))
                {
                    xml += String.Format("<status>1</status>");
                    xml += String.Format("<AuthCode>{0}</AuthCode>", AuthCode);
                    xml += String.Format("<LicenseId>{0}</LicenseId>", AuthCode);
                    xml += String.Format("<LicenseCountBySalesContactByProduct>{0}</LicenseCountBySalesContactByProduct>", LicenseCountBySalesContactByProduct);
                }
                else
                {
                    xml += String.Format("<status>0</status>");
                    xml += String.Format("<error>{0}</error>", ErrorMessage);
                }

                return xml;
            }
            catch (Exception e)
            {
                return String.Format("<status>0</status><error>{0}</error>", e.Message);
            }
        }

        protected byte[] sendAnswer(string Answer)
        {
            Log.add("Отдаем данные клиенту");

            if (ContentType == "xls")
            {
                return Encoding.UTF8.GetBytes(Answer);
            }
            if (ContentType == "xml")
            {
                return Encoding.UTF8.GetBytes(string.Format(
                    "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                    "<response>" +
                    "   <method>{0}</method>" +
                    "   {1}" +
                    "</response>"
                , Method, Answer));
            }

            return Encoding.UTF8.GetBytes(string.Format(
                    "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                    "<response>" +
                    "   <method>{0}</method>" +
                    "   {1}" +
                    "</response>"
                , Method, Answer));
        }

        protected byte[] sendAnswer(byte[] Answer)
        {
            Log.add("Отдаем данные клиенту");
            return Answer;
        }
    }
}
