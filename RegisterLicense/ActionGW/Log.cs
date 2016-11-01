using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace action_gw
{
    class Log
    {
        private static Log instance;
        List<string> logText = new List<string>();

        private Log()
        { }

        public static Log getInstance()
        {
            if (instance == null)
                instance = new Log();
            return instance;
        }

        public void add(string text)
        {
            try
            {
                string fileName = String.Format("log_{0:dd_MM_yy}.txt", DateTime.Now);
                string fileText = "";

                if (File.Exists(fileName)) fileText = System.IO.File.ReadAllText(fileName);

                System.IO.StreamWriter file = new System.IO.StreamWriter(fileName);
                file.Write(fileText + Environment.NewLine + "[" + DateTime.Now + "] RegisterLicense Service: " + text);
                file.Close();
            }
            catch (Exception er)
            {
            }
        }

        public string getLog()
        {
            string fileName = String.Format("log_{0:dd_MM_yy}.txt", DateTime.Now);

            try
            {
                if (File.Exists(fileName))
                    return System.IO.File.ReadAllText(fileName);
                else
                    return "";
            }
            catch (Exception er)
            {
                return "";
            }
        }
    }
}
