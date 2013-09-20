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

			var worldView = new WorldView(host, port);
        }
    }
}
