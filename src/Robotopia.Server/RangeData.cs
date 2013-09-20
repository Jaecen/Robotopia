using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robotopia.Server
{
	class RangeData
	{
		public readonly Range Range;
		public readonly byte[] Data;

		public RangeData(Range range, byte[] data)
		{
			Range = range;
			Data = data;
		}
	}
}
