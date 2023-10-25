using Opc.Ua;
using Opc.Ua.Configuration;
using System;
using System.IO;
using System.Threading;

namespace OpcServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ApplicationInstance application = new ApplicationInstance();
            application.ApplicationType = ApplicationType.Server;
            application.ConfigSectionName = "QiagenOpcServer";

            try
            {
                // process and command line arguments.
                if (application.ProcessCommandLine())
                {
                    return;
                }

                // check if running as a service.
                if (!Environment.UserInteractive)
                {
                    application.StartAsService(new QiagenOpcServer());
                    return;
                }
                // load the application configuration.
                //var opcConfig = new OpcApplicationConfiguration().Configure();
                try
                {
                    application.LoadApplicationConfiguration(Directory.GetCurrentDirectory() + "/Config/BatchPlantServer.Config.xml", false).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Config: " + ex.Message);
                    throw ex;
                }

                // check the application certificate.
                application.CheckApplicationInstanceCertificate(false, 0).Wait();

                Console.WriteLine($"#### Starting server on endpoint {application.ApplicationConfiguration.ServerConfiguration.BaseAddresses[0]} ####");
                // start the server.
                application.Start(new QiagenOpcServer()).Wait();
                Console.WriteLine("Server started, press Ctrl+C to exit ...");

                // allow canceling the connection process
                CancellationToken cancellationToken = default;
                var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                Console.CancelKeyPress += (_, eArgs) =>
                {
                    Console.WriteLine("\n#### Server exit ####");

                    cancellationTokenSource.Cancel();
                    eArgs.Cancel = true;
                    application.Stop();
                };

                // wait for Ctrl-C
                while (!cancellationTokenSource.Token.WaitHandle.WaitOne(1000))
                {
                }
            }
            catch (Exception e)
            {
                string text = "Exception: " + e.Message;
                if (e.InnerException != null)
                {
                    text += "\r\nInner exception: ";
                    text += e.InnerException.Message;
                }
            }
            Console.ReadLine();
        }
    }
}
