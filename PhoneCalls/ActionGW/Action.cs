using action_gw.PhoneCalls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace action_gw
{
    class Action
    {
        public GetPhoneCallsData[] getPhoneCalls()
        {
            DateTime dt = Convert.ToDateTime("01.01.2016");

            PhoneCallsServiceClient client = new PhoneCallsServiceClient();

            client.ClientCredentials.UserName.UserName = Properties.Settings.Default.ActionUser.ToString();
            client.ClientCredentials.UserName.Password = Properties.Settings.Default.ActionPassword.ToString();

            return client.GetPhoneCalls(dt);
        }

        public RequestError completePhoneCall(Guid PhoneCallId, Guid ActionStatusId, Guid? DeclineReasonStatusId, DateTime? NextCallTime)
        {
            PhoneCallsServiceClient client = new PhoneCallsServiceClient();

            client.ClientCredentials.UserName.UserName = Properties.Settings.Default.ActionUser.ToString();
            client.ClientCredentials.UserName.Password = Properties.Settings.Default.ActionPassword.ToString();

            RequestError result = client.CompletePhoneCall(PhoneCallId, ActionStatusId, DeclineReasonStatusId, NextCallTime);
            return result;
        }
    }
}
