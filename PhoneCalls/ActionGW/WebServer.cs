using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace action_gw
{
    class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        Log Log = Log.getInstance();

        public WebServer(params string[] prefixes)
        {
            if (!HttpListener.IsSupported) throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");

            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _listener.Start();
        }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                byte[] buf;

                                Request req = new Request();
                                byte[] rstr = req.WebRequest(ctx.Request);

                                buf = rstr;

                                if (req.ContentType == "xls")
                                {
                                    ctx.Response.AddHeader("Content-Transfer-Encoding", "binary");
                                    ctx.Response.AddHeader("Content-Disposition", "inline; filename=\"Phone calls.xlsx\"");
                                    ctx.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                }
                                if (req.ContentType == "xml")
                                {
                                    ctx.Response.ContentType = "text/xml";
                                }

                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch (Exception e)
                            {
                                Log.add(e.ToString());
                            } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}
