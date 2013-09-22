using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robotopia.Client
{
    class Program
    {
        static void Main(string[] args)
        {
			if(args.Length < 1)
				throw new ArgumentException("You must provide a hostname/IP to connect to");

			var host = args[0];

			int port = 0;
			if(args.Length == 2 && (!Int32.TryParse(args[1], out port) || port < 0 || port > 65535))
				throw new ArgumentException("Second argument must be a valid port number");

			ZeroMQ.ZmqContext zmqContext = ZeroMQ.ZmqContext.Create();

			var worldView = new WorldView(zmqContext, host, port);
			worldView.Move(50, 50, 49);
			worldView.Size(79, 25);

			var running = true;
			while(running)
			{
				var command = Console.ReadKey(true);
				var dirty = false;

				if(command.Key == ConsoleKey.Q)
					running = false;

				if(command.Key == ConsoleKey.UpArrow)
				{
					worldView.Move(0, 1, 0);
					dirty = true;
				}

				if(command.Key == ConsoleKey.DownArrow)
				{
					worldView.Move(0, -1, 0);
					dirty = true;
				}

				if(command.Key == ConsoleKey.LeftArrow)
				{
					worldView.Move(-1, 0, 0);
					dirty = true;
				}

				if(command.Key == ConsoleKey.RightArrow)
				{
					worldView.Move(1, 0, 0);
					dirty = true;
				}

				if(command.Key == ConsoleKey.A)
				{
					worldView.Move(0, 0, 1);
					dirty = true;
				}

				if(command.Key == ConsoleKey.Z)
				{
					worldView.Move(0, 0, -1);
					dirty = true;
				}


				if(dirty)
				{
					worldView.Fetch();
					worldView.Render();
				}
			}
        }
    }
}
