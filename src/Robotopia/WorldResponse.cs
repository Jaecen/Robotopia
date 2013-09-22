using System;

namespace Robotopia
{
	public enum WorldResponse : byte
	{
		Unknown = 0x00,
		Ok = 0x01,
		Error = 0xE0,
		ErrorBadMessage = 0xE1,
		ErrorUnrecognizedRequest = 0xE2
	}
}
