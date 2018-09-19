using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Walldisplay
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (var service = new Service1())
            {
                if (!Environment.UserInteractive)
                {
                    ServiceBase.Run(service);
                }
                else
                {
                    try
                    {
                        service.ConsoleStart(args);
                        Console.WriteLine("Press any key to stop...");
                        Console.ReadKey();
                        service.ConsoleStop();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.ToString());
                        throw e;
                    }
                }
            }
        }
    }
}
