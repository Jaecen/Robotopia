using System;
using System.IO;

namespace Robotopia
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

		public static RangeData ReadRangeData(this BinaryReader that)
		{
			var range = that.ReadRange();
			var data = that.ReadBytes((int)range.DataSize);

			return new RangeData(range, data);
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

		/// <summary>
		/// Returns a value bounded between the given extremes.
		/// </summary>
		/// <param name="that">The value to check against the bounds.</param>
		/// <param name="minimum">The minimum value, inclusive, to return.</param>
		/// <param name="maximum">The maximum value, inclusive, to return.</param>
		public static uint Constrain(this uint that, uint minimum, uint maximum)
		{
			if(that < minimum)
				return minimum;

			if(that > maximum)
				return maximum;

			return that;
		}

		public static Range Constrain(this Range that, uint length, uint width, uint height)
		{
			var lengthStart = that.LengthOffset.Constrain(0, length);
			var widthStart = that.WidthOffset.Constrain(0, width);
			var heightStart = that.HeightOffset.Constrain(0, height);

			var lengthEnd = (that.LengthOffset + that.LengthRun).Constrain(0, length);
			var widthEnd = (that.WidthOffset + that.WidthRun).Constrain(0, width);
			var heightEnd = (that.HeightOffset + that.HeightRun).Constrain(0, height);

			var lengthRun = lengthEnd - lengthStart;
			var widthRun = widthEnd - widthStart;
			var heightRun = heightEnd - heightStart;

			return new Range(lengthStart, widthStart, heightStart, lengthRun, widthRun, heightRun);
		}
	}
}
