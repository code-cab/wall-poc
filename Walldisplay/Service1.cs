using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Walldisplay
{
    public partial class Service1 : ServiceBase
    {
        IDisposable webApp;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            String url = args.FirstOrDefault(a => a.StartsWith("http"));
            if (url == null)
            {
                url = "http://127.0.0.1:9000/";
            }

            if (!Environment.UserInteractive) EventLog.WriteEntry("Walldisplay webinterface listening at: " + url, EventLogEntryType.Information);
            Console.WriteLine("Listening at: " + url);
            webApp = WebApp.Start<WebStartup>(url);
            if (!Environment.UserInteractive) EventLog.WriteEntry("Walldisplay started", EventLogEntryType.Information);

        }

        protected override void OnStop()
        {
            webApp.Dispose();
            EventLog.WriteEntry("Walldisplay stopped", EventLogEntryType.Information);
        }

        public void ConsoleStart(string[] args)
        {
            OnStart(args);
        }

        public void ConsoleStop()
        {
            OnStop();
        }
    }
}
