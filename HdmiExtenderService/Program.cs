using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using HdmiExtenderLib;
using Fclp;

namespace HdmiExtenderService
{
    public class ApplicationArguments
    {
        public bool cmd { get; set; }
        public bool verbose { get; set; }
        public int port { get; set; }
        public int networkInterface { get; set; }
        public List<string> devices { get; set; }
    }
    
        static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{

            // create a generic parser for the ApplicationArguments type
            var cmdParser = new FluentCommandLineParser<ApplicationArguments>();

            // Define application arguments
            cmdParser.Setup(arg => arg.cmd)
             .As('c', "cmd")
             .WithDescription("Run as a console application rather than a Windows Service.")
             .SetDefault(false);

            cmdParser.Setup(arg => arg.port)
             .As('p', "port")
             .WithDescription("Port to use for the output streams.")
             .SetDefault(18080);

            cmdParser.Setup(arg => arg.networkInterface)
             .As('n', "interface")
             .WithDescription("Network interface id where the input device is connected to.")
             .SetDefault(1);

            cmdParser.Setup(arg => arg.devices)
             .As('d', "devices")
             .WithDescription("Input device IPs.");

            cmdParser.Setup(arg => arg.verbose)
             .As('v', "verbose")
             .WithDescription("Print 'netdrop1' or 'netdrop2'  in the console when a frame is dropped.")
             .SetDefault(true);

            cmdParser.SetupHelp("h", "help")
             .Callback(text => Console.WriteLine(text));

            //Parse arguments
            var cmdParseResult = cmdParser.Parse(args);

            //If no errors, continue
            if (!cmdParseResult.HasErrors && !cmdParseResult.HelpCalled)
            {

                // Check whether ro run as a service or a console application
                if (cmdParser.Object.cmd)
                {
                    MainService svc = new MainService();

                    HdmiExtenderReceiver receiver = new HdmiExtenderReceiver(cmdParser.Object.devices[0], cmdParser.Object.networkInterface);

                    VideoWebServer server = new VideoWebServer(cmdParser.Object.port, -1, receiver);
                    server.Start();

                    receiver.Start();

                    Console.WriteLine("Jpeg still image:");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("http://localhost:" + cmdParser.Object.port + "/image.jpg");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine("Motion JPEG:");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("http://localhost:" + cmdParser.Object.port + "/image.mjpg");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine("PCM 48kHz, Signed 32 bit, Big Endian");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("http://localhost:" + cmdParser.Object.port + "/audio.wav");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine("Press ENTER to exit.");
                    Console.ReadLine();
                    Console.WriteLine("Shutting down...");
                    server.Stop();
                }
                else
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                    new MainService()
                    };
                    ServiceBase.Run(ServicesToRun);
                }
            }
		}
	}
}
