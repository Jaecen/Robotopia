using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroMQ;

namespace Robotopia.Server
{
	class Program
	{
		static void Main(string[] args)
		{
			var zmqContext = ZmqContext.Create();

			if(args.Length < 2)
				throw new Exception("You must specify a listen port and a path to a world file");

			int port;
			if(!Int32.TryParse(args[0], out port) || port < 0 || port > 65535)
				throw new ArgumentException("First argument must be a valid port number");
			
			var listenAddress = String.Format("tcp://127.0.0.1:{0}", port);

			var world = InitializeWorld(args[1]);
			Listen(zmqContext, listenAddress, world);
		}

		static World InitializeWorld(string worldFilePath)
		{
			if(File.Exists(worldFilePath))
				using(var worldFileStream = File.OpenRead(worldFilePath))
					return new World(worldFileStream);
			else
			{
				var world = new World(100, 100, 100);

				using(var worldFileStream = File.OpenWrite(worldFilePath))
					world.Save(worldFileStream);

				return world;
			}
		}

		static void Listen(ZmqContext zmqContext, string listenAddress, World world)
		{
			var handlers = new Dictionary<WorldRequest, Func<BinaryReader, World, byte[]>>
			{
				{ WorldRequest.GetRange, GetRangeHandler },
				{ WorldRequest.Set, SetHandler },
			};

			var listenBuffer = new byte[4096];

			using(var listenSocket = zmqContext.CreateSocket(SocketType.REP))
			{
				listenSocket.Bind(listenAddress);

				var listening = true;
				while(listening)
				{
					var requestSize = listenSocket.Receive(listenBuffer);
					if(listenSocket.ReceiveMore || requestSize < 1)
					{
						listenSocket.ReceiveMessage();
						listenSocket.Send(new byte[] { (byte)WorldResponse.ErrorBadMessage });
						continue;
					}

					using(var requestStream = new MemoryStream(listenBuffer, 0, requestSize))
					using(var requestReader = new BinaryReader(requestStream))
					{
						var requestType = (WorldRequest)requestReader.ReadByte();

						if(!handlers.ContainsKey(requestType))
						{
							listenSocket.Send(new byte[] { (byte)WorldResponse.ErrorUnrecognizedRequest });
							continue;
						}

						var response = handlers[requestType](requestReader, world);
						listenSocket.Send(response);
					}
				}
			}
		}

		static byte[] GetRangeHandler(BinaryReader requestReader, World world)
		{
			var requestedRange = requestReader.ReadRange();
			var rangeData = world.GetRange(requestedRange);

			using(var responseStream = new MemoryStream())
			{
				using(var responseWriter = new BinaryWriter(responseStream))
				{
					responseWriter.Write((byte)WorldResponse.Ok);
					responseWriter.Write(rangeData.Range);
					responseWriter.Write(rangeData.Data);
				}

				return responseStream.ToArray();
			}
		}

		static byte[] SetHandler(BinaryReader requestReader, World world)
		{
			var lengthOffset = requestReader.ReadUInt32();
			var widthOffset = requestReader.ReadUInt32();
			var heightOffset = requestReader.ReadUInt32();
			var value = requestReader.ReadByte();

			var updateRange = new Range(lengthOffset, widthOffset, heightOffset, 1, 1, 1);
			var rangeData = world.SetRange(updateRange, value);

			using(var responseStream = new MemoryStream())
			{
				using(var responseWriter = new BinaryWriter(responseStream))
				{
					responseWriter.Write((byte)WorldResponse.Ok);
					responseWriter.Write(rangeData.Range.LengthOffset);
					responseWriter.Write(rangeData.Range.WidthOffset);
					responseWriter.Write(rangeData.Range.HeightOffset);
					responseWriter.Write(rangeData.Data);
				}

				return responseStream.ToArray();
			}
		}
	}
}
