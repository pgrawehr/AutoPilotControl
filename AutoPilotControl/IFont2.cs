using System;

namespace AutoPilotControl
{
	internal interface IFont2
	{
		byte Width
		{
			get;
		}

		byte Height
		{
			get;
		}

		SpanByte this[char character]
		{
			get;
		}
	}
}
