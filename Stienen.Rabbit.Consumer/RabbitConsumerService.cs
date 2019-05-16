using System;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;

namespace Stienen.Rabbit.Consumer {
    [RunInstaller(true)]
    public class RabbitConsumerService : ServiceBase {
        public RabbitConsumerService()
        {
            AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            Program.StartUp();
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

//        //InstallUtil.exe
//        [RunInstallerAttribute(true)]
//        public class GatewayServiceInstaller : ServiceProcessInstaller
//        {
//            public void AddInstaller()
//            {
//                Account = ServiceAccount.NetworkService;
//
//                ServiceInstaller si = new ServiceInstaller();
//                si.DisplayName = Context.Parameters["DisplayName"] ?? "RabbitConsumer - Stienen BE";
//                si.ServiceName = Context.Parameters["ServiceName"] ?? "Fc3RabbitConsumerService";
//                si.StartType = ServiceStartMode.Manual;
//                Installers.Add(si);
//            }
//
//            public override void Install(System.Collections.IDictionary stateSaver)
//            {
//                AddInstaller();
//                base.Install(stateSaver);
//            }
//            public override void Commit(System.Collections.IDictionary savedState)
//            {
//                AddInstaller();
//                base.Commit(savedState);            }
//            public override void Rollback(System.Collections.IDictionary savedState)
//            {
//                AddInstaller();
//                base.Rollback(savedState);
//            }
//            public override void Uninstall(System.Collections.IDictionary savedState)
//            {
//                AddInstaller();
//                base.Uninstall(savedState);
//            }
//        }

        private void InitializeComponent()
        {
            // 
            // RabbitConsumerService
            // 
            this.ServiceName = "Fc3RabbitConsumerService";

        }
    }
}