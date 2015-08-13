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

            //Parse arguments
            var cmdParseResult = cmdParser.Parse(args);

            //If no errors, continue
            if (cmdParseResult.HasErrors == false)
            {

                // Check whether ro run as a service or a console application
                if (cmdParser.Object.cmd)
                {
                    ushort port = 18080;
                    MainService svc = new MainService();
                    VideoWebServer server = new VideoWebServer(port, -1, "192.168.168.55", 1);
                    server.Start();
                    Console.WriteLine("This service was run with the command line argument \"cmd\".");
                    Console.WriteLine("When run without arguments, this application acts as a Windows Service.");
                    Console.WriteLine();
                    Console.WriteLine("Jpeg still image:");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\thttp://localhost:" + port + "/image.jpg");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine("Motion JPEG:");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\thttp://localhost:" + port + "/image.mjpg");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine("PCM 48kHz, Signed 32 bit, Big Endian");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\thttp://localhost:" + port + "/audio.wav");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.Write("When you see ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("netdrop1");
                    Console.ResetColor();
                    Console.Write(" or ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("netdrop2");
                    Console.ResetColor();
                    Console.WriteLine(" in the console, this means a frame was dropped due to data loss between the Sender device and this program.");
                    Console.WriteLine();
                    Console.WriteLine("Http server running on port " + port + ". Press ENTER to exit.");
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
