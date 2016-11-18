using action_gw.PhoneCalls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace action_gw
{
    class Action
    {
        Log Log = Log.getInstance();

        public GetPhoneCallsData[] getPhoneCalls()
        {
            try
            {
                DateTime dt = Convert.ToDateTime("01.01.2016");

                PhoneCallsServiceClient client = new PhoneCallsServiceClient();

                client.ClientCredentials.UserName.UserName = Properties.Settings.Default.ActionUser.ToString();
                client.ClientCredentials.UserName.Password = Properties.Settings.Default.ActionPassword.ToString();

                return client.GetPhoneCalls(dt);
            }
            catch (Exception e)
            {
                Log.add(String.Format("[ERR] Системная ошибка: {0} Stack: {1}", e.Message, e.StackTrace));
                return null;
            }
        }

        public RequestError completePhoneCall(Guid PhoneCallId, Guid ActionStatusId, Guid? DeclineReasonStatusId, DateTime? NextCallTime)
        {
            try
            {
                PhoneCallsServiceClient client = new PhoneCallsServiceClient();

                client.ClientCredentials.UserName.UserName = Properties.Settings.Default.ActionUser.ToString();
                client.ClientCredentials.UserName.Password = Properties.Settings.Default.ActionPassword.ToString();

                RequestError result = client.CompletePhoneCall(PhoneCallId, ActionStatusId, DeclineReasonStatusId, NextCallTime);
                return result;
            }
            catch (Exception e)
            {
                Log.add(String.Format("[ERR] Системная ошибка: {0} Stack: {1}", e.Message, e.StackTrace));
                return null;
            }
        }
    }
}
