using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using action_gw.PhoneCalls;
using System.Windows.Forms;

namespace action_gw
{
    class DB
    {
        protected MySqlConnection MySQL;
        protected Log Log = Log.getInstance();

        public bool connect()
        {
            String ConnectionString = String.Format("Server={1}; Database={0}; Uid={2}; Pwd={3}; CharSet=utf8; default command timeout=120;",
                Properties.Settings.Default.MysqlDB, 
                Properties.Settings.Default.MysqlHost, 
                Properties.Settings.Default.MysqlUser, 
                Properties.Settings.Default.MysqlPass);
            try
            {
                //Log.add("Подключаемся к БД...");   
                MySQL = new MySqlConnection(ConnectionString);
                MySQL.Open();
                //Log.add("Подключились к БД успешно.");

                return true;
            }
            catch(Exception e)
            {
                Log.add("Ошибка подключения к БД: " + e.Message);
                return false;
            }
        }

        public bool updatePhoneCalls(GetPhoneCallsData[] PhoneCalls)
        {
            connect();

            //start transaction
            MySqlCommand sql_cmd1 = new MySqlCommand("START TRANSACTION;", MySQL);
            sql_cmd1.ExecuteNonQuery();


            //clear
            MySqlCommand sql_cmd2 = new MySqlCommand("TRUNCATE TABLE phone_calls;", MySQL);
            sql_cmd2.ExecuteNonQuery();

            //insert phone calls
            foreach (GetPhoneCallsData phoneCall in PhoneCalls)
            {
                String SQL = "INSERT INTO phone_calls SET";

                if (!String.IsNullOrEmpty(phoneCall.CompanyId.ToString()))
                    SQL += String.Format(" CompanyId = '{0}'", phoneCall.CompanyId.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.CompanyName))
                    SQL += String.Format(", CompanyName = '{0}'", phoneCall.CompanyName.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.PhoneCallId.ToString()))
                    SQL += String.Format(", PhoneCallId = '{0}'", phoneCall.PhoneCallId.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.PhoneCallNr))
                    SQL += String.Format(", PhoneCallNr = '{0}'", phoneCall.PhoneCallNr.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.Pin))
                    SQL += String.Format(", Pin = '{0}'", phoneCall.Pin.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.CustomerName))
                    SQL += String.Format(", CustomerName = '{0}'", phoneCall.CustomerName.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.Inn))
                    SQL += String.Format(", Inn = '{0}'", phoneCall.Inn.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.Kpp))
                    SQL += String.Format(", Kpp = '{0}'", phoneCall.Kpp.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.Email))
                    SQL += String.Format(", Email = '{0}'", phoneCall.Email.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.BitrixId))
                    SQL += String.Format(", BitrixId = '{0}'", phoneCall.BitrixId.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.ScheduledStart.ToString()))
                    SQL += String.Format(", ScheduledStart = '{0}'", phoneCall.ScheduledStart.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.Overview))
                    SQL += String.Format(", Overview = '{0}'", phoneCall.Overview.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.Priority.ToString()))
                    SQL += String.Format(", Priority = '{0}'", phoneCall.Priority.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.EventNr.ToString()))
                    SQL += String.Format(", EventNr = '{0}'", phoneCall.EventNr.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.DateEvent.ToString()))
                    SQL += String.Format(", DateEvent = '{0}'", phoneCall.DateEvent.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.ProductEvent))
                    SQL += String.Format(", ProductEvent = '{0}'", phoneCall.ProductEvent.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.Phone))
                    SQL += String.Format(", Phone = '{0}'", phoneCall.Phone.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.EventName))
                    SQL += String.Format(", EventName = '{0}'", phoneCall.EventName.ToString().Replace("'", "\\'"));
                if (!String.IsNullOrEmpty(phoneCall.TimeDifference.ToString()))
                    SQL += String.Format(", TimeDifference = '{0}'", phoneCall.TimeDifference.ToString().Replace("'", "\\'"));


                MySqlCommand sql_cmd3 = new MySqlCommand(SQL, MySQL);
                sql_cmd3.ExecuteNonQuery();
            }

            //end transaction
            MySqlCommand sql_cmd4 = new MySqlCommand("COMMIT;", MySQL);
            sql_cmd4.ExecuteNonQuery();

            return true;
        }

        public List<GetPhoneCallsData> getPhoneCalls(String Date)
        {
            List<GetPhoneCallsData> PhoneCalls = new List<GetPhoneCallsData>();

            connect();

            MySqlCommand sql_cmd = new MySqlCommand(String.Format("SELECT pc.* FROM phone_calls pc LEFT JOIN call_status cs ON pc.PhoneCallId = cs.phoneCallId WHERE STR_TO_DATE(pc.DateEvent, '%d.%m.%Y') >= STR_TO_DATE('{0}', '%d.%m.%Y') AND cs.id IS NULL", Date), MySQL);
            MySqlDataReader MyDataReader;
            MyDataReader = sql_cmd.ExecuteReader();

            int count = 0;

            while (MyDataReader.Read())
            {
                try
                {
                    GetPhoneCallsData phone_call = new GetPhoneCallsData();

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("CompanyId").ToString()))
                        phone_call.CompanyId = new Guid(MyDataReader.GetString("CompanyId"));

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("CompanyName").ToString()))
                        phone_call.CompanyName = MyDataReader.GetString("CompanyName");

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("PhoneCallId").ToString()))
                        phone_call.PhoneCallId = new Guid(MyDataReader.GetString("PhoneCallId"));

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("PhoneCallNr").ToString()))
                        phone_call.PhoneCallNr = MyDataReader.GetString("PhoneCallNr");

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("Pin").ToString()))
                        phone_call.Pin = MyDataReader.GetString("Pin");

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("CustomerName").ToString()))
                        phone_call.CustomerName = MyDataReader.GetString("CustomerName");

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("Inn").ToString()))
                        phone_call.Inn = MyDataReader.GetString("Inn");

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("Kpp").ToString()))
                        phone_call.Kpp = MyDataReader.GetString("Kpp");

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("Email").ToString()))
                        phone_call.Email = MyDataReader.GetString("Email");

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("BitrixId").ToString()))
                        phone_call.BitrixId = MyDataReader.GetString("BitrixId");

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("ScheduledStart").ToString().Trim()))
                        phone_call.ScheduledStart = DateTime.Parse(MyDataReader.GetString("ScheduledStart"));

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("Overview").ToString()))
                        phone_call.Overview = MyDataReader.GetString("Overview");

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("Priority").ToString()))
                        phone_call.Priority = MyDataReader.GetInt16("Priority");

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("EventNr").ToString()))
                        phone_call.EventNr = MyDataReader.GetInt32("EventNr");

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("DateEvent").ToString()))
                    {
                        phone_call.DateEvent = DateTime.Parse(MyDataReader.GetString("DateEvent").ToString());
                    }

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("ProductEvent").ToString()))
                        phone_call.ProductEvent = MyDataReader.GetString("ProductEvent");

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("Phone").ToString()))
                        phone_call.Phone = MyDataReader.GetString("Phone");

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("EventName").ToString()))
                        phone_call.EventName = MyDataReader.GetString("EventName");

                    if (!String.IsNullOrEmpty(MyDataReader.GetString("TimeDifference").ToString()))
                        phone_call.TimeDifference = MyDataReader.GetInt16("TimeDifference");

                    PhoneCalls.Add(phone_call);
                } catch(Exception e)
                {
                    
                }

                count++;
            }
            MyDataReader.Close();

            return PhoneCalls;
        }

        public void completePhoneCall(String phoneCallId, String actionStatusId, String declineReasonStatusId, String nextCallTime)
        {
            connect();

            MySqlCommand sql_cmd = new MySqlCommand(String.Format("INSERT INTO call_status SET phoneCallId = '{0}', actionStatusId = '{1}', declineReasonStatusId = '{2}', nextCallTime = '{3}';",
                phoneCallId, actionStatusId, declineReasonStatusId, nextCallTime), MySQL);
            sql_cmd.ExecuteNonQuery();
        }

        public bool disconnect()
        {
            MySQL.Close();
            return true;
        }
    }
}
