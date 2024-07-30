using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XLSConfigBackupService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //XLSBackupService Servicio = new XLSBackupService();
            //Servicio.Debug();
            //Thread.Sleep(Timeout.Infinite);

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new XLSBackupService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
