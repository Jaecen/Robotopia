using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZeroMQ;

namespace Robotopia.Client
{
	class WorldView
	{
		readonly ZmqSocket WorldSocket;

		uint Width;
		uint Height;
		uint PositionX;
		uint PositionY;
		uint PositionZ;
		byte[] Buffer;

		public WorldView(ZmqContext zmqContext, string host, int port)
		{
			var worldUri = String.Format("tcp://{0}:{1}", host, port);
			WorldSocket = zmqContext.CreateSocket(SocketType.REQ);
			WorldSocket.Connect(worldUri);
		}

		public void Size(uint width, uint height)
		{
			Width = width; 
			Height = height;
		}

		public void Move(int x, int y, int z)
		{
			if(x > 0)
				PositionX += (uint)x;
			else if(PositionY > 0)
				PositionX -= (uint)(-x);

			if(y > 0)
				PositionY += (uint)y;
			else if(PositionY > 0)
				PositionY -= (uint)(-y);
			
			if(z > 0)
				PositionZ += (uint)z;
			else if(PositionZ > 0)
				PositionZ -= (uint)(-z);
		}

		public void Fetch()
		{
			var lengthOffset = PositionY < (Height / 2) ? 0 : PositionY - (Height / 2);
			var widthOffset = PositionX < (Width / 2) ? 0 : PositionX - (Width / 2);
			var requestRange = new Range(lengthOffset, widthOffset, PositionZ, Height, Width, 1);

			using(var requestStream = new MemoryStream())
			{
				using(var requestWriter = new BinaryWriter(requestStream))
				{
					requestWriter.Write((byte)WorldRequest.GetRange);
					requestWriter.Write(requestRange);
				}

				WorldSocket.Send(requestStream.ToArray());
			}

			var response = new byte[Width * Height * 1 * 2];
			var responseSize = WorldSocket.Receive(response);
			using(var responseStream = new MemoryStream(response, 0, responseSize))
			using(var responseReader = new BinaryReader(responseStream))
			{
				var responseCode = (WorldResponse)responseReader.ReadByte();

				if((WorldResponse)((byte)responseCode & 0xF0) == WorldResponse.Error)
					throw new Exception("World response error: " + responseCode.ToString());

				var rangeData = responseReader.ReadRangeData();
				Buffer = rangeData.Data;
			}
		}

		public void Render()
		{
			Console.CursorVisible = false;
			Console.SetCursorPosition(0, 0);

			for(int i = 0; i < Buffer.Length; i++)
			{
				if(i > 0 && i % Width == 0)
				{
					Console.CursorLeft = 0;
					Console.CursorTop++;
				}

				if(Buffer[i] == 0)
					Console.Write(' ');
				else
					Console.Write((char)(41 + Buffer[i]));
			}
		}
	}
}
