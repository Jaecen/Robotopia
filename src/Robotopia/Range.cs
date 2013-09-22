using System;

namespace Robotopia
{
	public struct Range
	{
		public readonly uint LengthOffset;
		public readonly uint WidthOffset;
		public readonly uint HeightOffset;
		public readonly uint LengthRun;
		public readonly uint WidthRun;
		public readonly uint HeightRun;

		public uint DataSize
		{ get { return LengthRun * WidthRun * HeightRun; } }

		public Range(uint lengthOffset, uint widthOffset, uint heightOffset, uint lengthRun, uint widthRun, uint heightRun)
		{
			LengthOffset = lengthOffset;
			WidthOffset = widthOffset;
			HeightOffset = heightOffset;
			LengthRun = lengthRun;
			WidthRun = widthRun;
			HeightRun = heightRun;
		}
	}
}
