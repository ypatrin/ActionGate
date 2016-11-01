using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using action_gw.PhoneCalls;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.ServiceModel;
using action_gw.Properties;
using System.Drawing;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace action_gw
{
    class Request
    {
        private string Method;
        private string[] RequestArr;
        private string data_type = "xml";

        Log Log = Log.getInstance();

        public string ContentType = "xml";

        public byte[] WebRequest(HttpListenerRequest request)
        {
            if (request.RawUrl == "" || request.RawUrl == "/")
            {
                return sendAnswer("<error>Method does not support</error>");
            }

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
                if (Method == "getPhoneCalls.xml")
                {
                    this.data_type = "xml";
                    return sendAnswer(getPhoneCalls());
                }
                if (Method == "getPhoneCalls.xls")
                {
                    this.data_type = "xls";
                    return sendAnswer(getPhoneCalls());
                }
                if (Method == "getActionStatuses.xml")
                {
                    return sendAnswer(getActionStatuses());
                }
                if (Method == "completePhoneCall")
                {
                    return sendAnswer(completePhoneCall());
                }
                else if (Method == "favicon.ico")
                {
                    return sendAnswer("");
                }
                else
                {
                    xml = "<result><status>-1</status><message>Method does not support</message></result>";
                    Log.add(string.Format("Метод {0} не поддерживается", Method));
                }
            }
            catch(Exception e)
            {
                Log.add(String.Format("[ERR] Системная ошибка: {0} Stack: {1}", e.Message, e.StackTrace));
                return sendAnswer("<result><status>-1</status><message>System error</message></result>");
            }

            return sendAnswer(xml);
        }

        protected byte[] getPhoneCalls()
        {
            string date = "";

            if (RequestArr != null && RequestArr.Length > 0)
            {
                foreach (string req_str in RequestArr)
                {
                    string[] p = req_str.Split("=".ToCharArray());

                    if (p[0] == "date")
                    {
                        date = p[1].Replace("%20", " ");
                    }
                }
            }

            if (date == "") date = DateTime.Today.ToString();

            //DateTime dt = Convert.ToDateTime(date);

            DB db = new DB();
            List<GetPhoneCallsData> phoneCalls = db.getPhoneCalls(date);

            if (data_type == "xml")
            {
                string answer = "";

                Log.add("Формируем xml файл GetPhoneCalls");
                ContentType = "xml";

                // Create XML
                answer += "<phone_calls>";

                foreach (GetPhoneCallsData phoneCall in phoneCalls)
                {
                    answer += "<phone_call>";

                    if (!String.IsNullOrEmpty(phoneCall.CompanyId.ToString()))
                        answer += "<CompanyId>" + phoneCall.CompanyId.ToString() + "</CompanyId>";
                    if (!String.IsNullOrEmpty(phoneCall.CompanyName))
                        answer += "<CompanyName>" + phoneCall.CompanyName + "</CompanyName>";
                    if (!String.IsNullOrEmpty(phoneCall.PhoneCallId.ToString()))
                        answer += "<PhoneCallId>" + phoneCall.PhoneCallId.ToString() + "</PhoneCallId>";
                    if (!String.IsNullOrEmpty(phoneCall.PhoneCallNr))
                        answer += "<PhoneCallNr>" + phoneCall.PhoneCallNr + "</PhoneCallNr>";
                    if (!String.IsNullOrEmpty(phoneCall.Pin))
                        answer += "<Pin>" + phoneCall.Pin + "</Pin>";
                    if (!String.IsNullOrEmpty(phoneCall.CustomerName))
                        answer += "<CustomerName>" + phoneCall.CustomerName + "</CustomerName>";
                    if (!String.IsNullOrEmpty(phoneCall.Inn))
                        answer += "<Inn>" + phoneCall.Inn + "</Inn>";
                    if (!String.IsNullOrEmpty(phoneCall.Kpp))
                        answer += "<Kpp>" + phoneCall.Kpp + "</Kpp>";
                    if (!String.IsNullOrEmpty(phoneCall.Email))
                        answer += "<Email>" + phoneCall.Email + "</Email>";
                    if (!String.IsNullOrEmpty(phoneCall.BitrixId))
                        answer += "<BitrixId>" + phoneCall.BitrixId + "</BitrixId>";
                    if (!String.IsNullOrEmpty(phoneCall.ScheduledStart.ToString()))
                        answer += "<ScheduledStart>" + phoneCall.ScheduledStart.ToString() + "</ScheduledStart>";
                    if (!String.IsNullOrEmpty(phoneCall.Overview))
                        answer += "<Overview>" + phoneCall.Overview + "</Overview>";
                    if (!String.IsNullOrEmpty(phoneCall.Priority.ToString()))
                        answer += "<Priority>" + phoneCall.Priority.ToString() + "</Priority>";
                    if (!String.IsNullOrEmpty(phoneCall.EventNr.ToString()))
                        answer += "<EventNr>" + phoneCall.EventNr.ToString() + "</EventNr>";
                    if (!String.IsNullOrEmpty(phoneCall.DateEvent.ToString()))
                        answer += "<DateEvent>" + phoneCall.DateEvent.ToString() + "</DateEvent>";
                    if (!String.IsNullOrEmpty(phoneCall.ProductEvent))
                        answer += "<ProductEvent>" + phoneCall.ProductEvent + "</ProductEvent>";
                    if (!String.IsNullOrEmpty(phoneCall.Phone))
                        answer += "<Phone>" + phoneCall.Phone + "</Phone>";
                    if (!String.IsNullOrEmpty(phoneCall.EventName))
                        answer += "<EventName>" + phoneCall.EventName + "</EventName>";
                    if (!String.IsNullOrEmpty(phoneCall.TimeDifference.ToString()))
                        answer += "<TimeDifference>" + phoneCall.TimeDifference.ToString() + "</TimeDifference>";

                    answer += "</phone_call>";
                }
                answer += "</phone_calls>";

                return Encoding.UTF8.GetBytes(answer);
            }
            if (data_type == "xls")
            {
                var excelApp = new Excel.Application();
                excelApp.Visible = false;
                Excel._Workbook workBook = excelApp.Workbooks.Add();
                Excel._Worksheet workSheet = (Excel.Worksheet)workBook.Worksheets.get_Item(1);

                byte[] answer;
                Log.add("Формируем xls файл GetPhoneCalls");
                ContentType = "xls";

                //delete old xls file
                if (File.Exists("phone_calls.xls")) File.Delete("phone_calls.xls");

                workSheet.Cells[1, "A"] = "ID Компании";
                workSheet.Cells[1, "B"] = "Наименование кампании";
                workSheet.Cells[1, "C"] = "ИД звонка";
                workSheet.Cells[1, "D"] = "№ звонка";
                workSheet.Cells[1, "E"] = "Пин клиента";
                workSheet.Cells[1, "F"] = "Наименование клиента";
                workSheet.Cells[1, "G"] = "ИНН";
                workSheet.Cells[1, "H"] = "КПП";
                workSheet.Cells[1, "I"] = "E-mail";
                workSheet.Cells[1, "J"] = "BitrixId";
                workSheet.Cells[1, "K"] = "Назначенная дата звонка";
                workSheet.Cells[1, "L"] = "Презентация";
                workSheet.Cells[1, "M"] = "Приоритет";
                workSheet.Cells[1, "N"] = "№ события";
                workSheet.Cells[1, "O"] = "Дата события";
                workSheet.Cells[1, "P"] = "Продукт по событию";
                workSheet.Cells[1, "Q"] = "Телефон";
                workSheet.Cells[1, "R"] = "Событие";
                workSheet.Cells[1, "S"] = "Часовая разница";

                // Start from 1 col
                int i = 2;

                // Write data to xls
                foreach (GetPhoneCallsData phoneCall in phoneCalls)
                {
                    if (!String.IsNullOrEmpty(phoneCall.CompanyId.ToString()))
                        workSheet.Cells[i, "A"] = phoneCall.CompanyId.ToString();
                    if (!String.IsNullOrEmpty(phoneCall.CompanyName))
                        workSheet.Cells[i, "B"] = phoneCall.CompanyName;
                    if (!String.IsNullOrEmpty(phoneCall.PhoneCallId.ToString()))
                        workSheet.Cells[i, "C"] = phoneCall.PhoneCallId.ToString();
                    if (!String.IsNullOrEmpty(phoneCall.PhoneCallNr))
                        workSheet.Cells[i, "D"] = phoneCall.PhoneCallNr;
                    if (!String.IsNullOrEmpty(phoneCall.Pin))
                        workSheet.Cells[i, "E"] = phoneCall.Pin;
                    if (!String.IsNullOrEmpty(phoneCall.CustomerName))
                        workSheet.Cells[i, "F"] = phoneCall.CustomerName;
                    if (!String.IsNullOrEmpty(phoneCall.Inn))
                        workSheet.Cells[i, "G"] = phoneCall.Inn;
                    if (!String.IsNullOrEmpty(phoneCall.Kpp))
                        workSheet.Cells[i, "H"] = phoneCall.Kpp;
                    if (!String.IsNullOrEmpty(phoneCall.Email))
                        workSheet.Cells[i, "I"] = phoneCall.Email;
                    if (!String.IsNullOrEmpty(phoneCall.BitrixId))
                        workSheet.Cells[i, "J"] = phoneCall.BitrixId;
                    if (!String.IsNullOrEmpty(phoneCall.ScheduledStart.ToString()))
                        workSheet.Cells[i, "K"] = phoneCall.ScheduledStart.ToString();
                    if (!String.IsNullOrEmpty(phoneCall.Overview))
                        workSheet.Cells[i, "L"] = phoneCall.Overview;
                    if (!String.IsNullOrEmpty(phoneCall.Priority.ToString()))
                        workSheet.Cells[i, "M"] = phoneCall.Priority.ToString();
                    if (!String.IsNullOrEmpty(phoneCall.EventNr.ToString()))
                        workSheet.Cells[i, "N"] = phoneCall.EventNr.ToString();
                    if (!String.IsNullOrEmpty(phoneCall.DateEvent.ToString()))
                        workSheet.Cells[i, "O"] = phoneCall.DateEvent.ToString();
                    if (!String.IsNullOrEmpty(phoneCall.ProductEvent))
                        workSheet.Cells[i, "P"] = phoneCall.ProductEvent;
                    if (!String.IsNullOrEmpty(phoneCall.Phone))
                        workSheet.Cells[i, "Q"] = phoneCall.Phone;
                    if (!String.IsNullOrEmpty(phoneCall.EventName))
                        workSheet.Cells[i, "R"] = phoneCall.EventName;
                    if (!String.IsNullOrEmpty(phoneCall.TimeDifference.ToString()))
                        workSheet.Cells[i, "S"] = phoneCall.TimeDifference.ToString();

                    i++;
                }

                // Write xls
                Random rnd = new Random();
                string fname = Application.StartupPath + @"\phone_calls_" + rnd.Next(1000, 9000).ToString() + ".xlsx";
                workBook.SaveAs(fname, Excel.XlFileFormat.xlOpenXMLWorkbook);
                workBook.Close(Type.Missing, Type.Missing, Type.Missing);
                
                excelApp.Quit();

                Marshal.FinalReleaseComObject(workSheet);
                Marshal.FinalReleaseComObject(workBook);
                Marshal.FinalReleaseComObject(excelApp);

                //workbook.SaveToFile("phone_calls.xls");

                // Read xls
                answer = File.ReadAllBytes(fname);

                // Delete xls
                if (File.Exists(fname)) File.Delete(fname);

                //send xls to client
                return answer;
            }

            return Encoding.UTF8.GetBytes("");
        }

        protected byte[] getActionStatuses()
        {
            string companyId = Properties.Settings.Default.ActionDefaultCompanyId;

            if (RequestArr != null && RequestArr.Length > 0)
            {
                foreach (string req_str in RequestArr)
                {
                    string[] p = req_str.Split("=".ToCharArray());

                    if (p[0] == "company_id")
                    {
                        companyId = p[1].Replace("%20", " ");
                    }

                }
            }

            try
            {
                System.Guid guid = System.Guid.Parse(companyId);
            }
            catch(Exception e)
            {
                string error = String.Format("<error>Exception: {0}</error>", e.Message);
                Log.add(String.Format("Ошибка: {0}", e.Message));
                return Encoding.UTF8.GetBytes(error);
            }

            Log.add("Подключаетмя к Актиону");
            PhoneCallsServiceClient client = new PhoneCallsServiceClient();

            client.ClientCredentials.UserName.UserName = Properties.Settings.Default.ActionUser.ToString();
            client.ClientCredentials.UserName.Password = Properties.Settings.Default.ActionPassword.ToString();

            Log.add("Запрашиваем ActionStatusesData...");
            ActionStatusesData[] statusesData = client.GetActionStatusesForCampaignId(System.Guid.Parse(companyId));
            Log.add("ActionStatusesData получено");

            string answer = "";

            Log.add("Формируем xml файл ActionStatusesData");
            ContentType = "xml";

            // Create XML
            answer += "<actionStatuses>";

            foreach (ActionStatusesData statusData in statusesData)
            {
                answer += "<actionStatus id=\"" + statusData.ActionStatusId + "\">" +
                "<actionStatusName>" + statusData.ActionStatusName + "</actionStatusName>" +
                "<typeActionStatus >" + statusData.TypeActionStatus + "</typeActionStatus >" +
                "<typeActionStatusName  >" + statusData.TypeActionStatusName + "</typeActionStatusName  >" +
                "<isSpecifyRefusalReason  >" + statusData.IsSpecifyRefusalReason + "</isSpecifyRefusalReason  >" +
                "<isFinal  >" + statusData.IsFinal + "</isFinal  >" +
                "<typeActionStatus >" + statusData.TypeActionStatus + "</typeActionStatus >" +
                "</actionStatus>";
            }
            answer += "</actionStatuses>";

            return Encoding.UTF8.GetBytes(answer);
        }

        protected byte[] completePhoneCall()
        {
            string phoneCallId = "";
            string actionStatusId = "";
            string declineReasonStatusId = "";
            string nextCallTime = "";

            if (RequestArr != null && RequestArr.Length > 0)
            {
                foreach (string req_str in RequestArr)
                {
                    string[] p = req_str.Split("=".ToCharArray());

                    if (p[0] == "phoneCallId")
                    {
                        phoneCallId = p[1].Replace("%20", " ");
                    }

                    if (p[0] == "actionStatusId")
                    {
                        actionStatusId = p[1].Replace("%20", " ");
                    }

                    if (p[0] == "declineReasonStatusId")
                    {
                        declineReasonStatusId = p[1].Replace("%20", " ");
                    }

                    if (p[0] == "nextCallTime")
                    {
                        nextCallTime = p[1].Replace("%20", " ");
                    }
                }
            }

            if (String.IsNullOrEmpty(phoneCallId))
            {
                return Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "<result><status>-1</status><message>Empty phoneCallId</message></result>");
            }

            if (String.IsNullOrEmpty(actionStatusId))
            {
                return Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "<result><status>-1</status><message>Empty actionStatusId</message></result>");
            }

            try
            {
                Action Action = new Action();
                RequestError result = new RequestError();

                if (String.IsNullOrEmpty(declineReasonStatusId) && !String.IsNullOrEmpty(nextCallTime))
                {
                    result = Action.completePhoneCall(new Guid(phoneCallId), new Guid(actionStatusId), null, DateTime.Parse(nextCallTime));
                }
                if (!String.IsNullOrEmpty(declineReasonStatusId) && String.IsNullOrEmpty(nextCallTime))
                {
                    result = Action.completePhoneCall(new Guid(phoneCallId), new Guid(actionStatusId), new Guid(declineReasonStatusId), null);
                }
                if (String.IsNullOrEmpty(nextCallTime) && String.IsNullOrEmpty(declineReasonStatusId))
                {
                    result = Action.completePhoneCall(new Guid(phoneCallId), new Guid(actionStatusId), null, null);
                }
                if (!String.IsNullOrEmpty(nextCallTime) && !String.IsNullOrEmpty(declineReasonStatusId))
                {
                    result = Action.completePhoneCall(new Guid(phoneCallId), new Guid(actionStatusId), new Guid(declineReasonStatusId), DateTime.Parse(nextCallTime));
                }                

                //result = Action.completePhoneCall(new Guid(phoneCallId), new Guid(actionStatusId), new Guid(declineReasonStatusId), DateTime.Parse(nextCallTime));

                DB db = new DB();
                db.completePhoneCall(phoneCallId, actionStatusId, declineReasonStatusId, nextCallTime);

                return Encoding.UTF8.GetBytes(String.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "<result><status>{0}</status><message>{1}</message></result>", result.Code, result.Message));
            }
            catch (Exception e)
            {
                return Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "<result><status>0</status><message>" + e.Message + "</message></result>");
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
