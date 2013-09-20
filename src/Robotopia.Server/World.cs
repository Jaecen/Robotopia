using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robotopia.Server
{
	class World
	{
		byte[] Preamble = new byte[] { (byte)'R', (byte)'O', (byte)'B', (byte)'O' };
		ushort Version = 0;

		uint Length;
		uint Width;
		uint Height;
		byte[] Terrain;

		// Width is left to right
		// Length is top to bottom
		// Height is bottom to top
		// Terrain is stored in length then width then height

		public World(uint length, uint width, uint height)
		{
			//TODO: 2GB cap
			if(length * width * height > Int32.MaxValue)
				throw new ArgumentException("World volume can't exceed 2^31");

			Length = length;
			Width = width;
			Height = height;
			Terrain = new byte[Length * Width * Height];

			// Fill the world from 0 to height / 2
			for(uint h = 0; h < Height; h++)
				for(uint l = 0; l < Length; l++)
					for(uint w = 0; w < Width; w++)
					{
						var index = (h * Length * Width) + (l * Width) + w;
						Terrain[index] = h < height / 2 ? (byte)1 : (byte)0;
					}
		}

		public World(Stream inputStream)
		{
			using(var worldFileReader = new BinaryReader(inputStream))
			{
				var preamble = worldFileReader.ReadBytes(4);
				var version = worldFileReader.ReadUInt16();
				var width = worldFileReader.ReadUInt32();
				var height = worldFileReader.ReadUInt32();
				var depth = worldFileReader.ReadUInt32();

				//TODO: 2GB cap
				Terrain = worldFileReader.ReadBytes((int)(Length * Width * Height));
			}
		}

		public void Save(Stream outputStream)
		{
			using(var worldFileWriter = new BinaryWriter(outputStream))
			{
				worldFileWriter.Write(Preamble);
				worldFileWriter.Write(Version);
				worldFileWriter.Write(Length);
				worldFileWriter.Write(Width);
				worldFileWriter.Write(Height);

				worldFileWriter.Write(Terrain);
			}
		}

		internal RangeData GetRange(Range range)
		{
			// Need to account for edges when getting the requested range
			throw new NotImplementedException();
		}

		internal RangeData SetRange(Range range, byte value)
		{
			throw new NotImplementedException();
		}
	}
}
