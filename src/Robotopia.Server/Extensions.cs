using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robotopia.Server
{
	public static class Extensions
	{
		public static Range ReadRange(this BinaryReader that)
		{
			var lengthOffset = that.ReadUInt32();
			var widthOffset = that.ReadUInt32();
			var heightOffset = that.ReadUInt32();
			var lengthRun = that.ReadUInt32();
			var widthRun = that.ReadUInt32();
			var heightRun = that.ReadUInt32();

			return new Range(lengthOffset, widthOffset, heightOffset, lengthRun, widthRun, heightRun);
		}

		public static void Write(this BinaryWriter that, Range range)
		{
			that.Write(range.LengthOffset);
			that.Write(range.WidthOffset);
			that.Write(range.HeightOffset);
			that.Write(range.LengthRun);
			that.Write(range.WidthRun);
			that.Write(range.HeightRun);
		}
	}
}
