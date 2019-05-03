using System;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;

namespace RabbitConsumer {
    [RunInstaller(true)]
    public class RabbitConsumerService : ServiceBase {
        public RabbitConsumerService()
        {
            AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            Program.StartUp(args);
        }

        protected override void OnStop()
        {
            Program.ShutDown();
        }
        public void RunConsole(string[] args)
        {
            Trace.Close();
            Trace.Listeners.Add(new ConsoleTraceListener());
            OnStart(args);
            Trace.WriteLine("Service running... press any key to stop");
            Console.Read();
            OnStop();
        }
    }
}