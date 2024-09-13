using System;
using System.Drawing;
using System.Text;
using Iot.Device.EPaper.Buffers;

namespace AutoPilotControl
{
	internal class FrameBuffer1BitPerPixelFast : FrameBuffer1BitPerPixel
	{
		public FrameBuffer1BitPerPixelFast(int height, int width) : base(height, width)
		{
		}

		public FrameBuffer1BitPerPixelFast(int height, int width, byte[] buffer) : base(height, width, buffer)
		{
		}

		public override void Clear(Color color)
		{
			byte num = color.To1bpp();
			if (num == 0)
			{
				Array.Clear(Buffer, 0, BufferByteCount);
			}
			else
			{
				// Don't have something better, it seems
				base.Clear(color);
			}
		}

		public void SetPixelOn(int x, int yTimesWidth)
		{
			int bufferIndexForPoint = (x + yTimesWidth) / 8;
			byte mask = (byte)(128 >> (x & 7));
			Buffer[bufferIndexForPoint] |= mask;
		}

		public void SetPixelOff(int x, int yTimesWidth)
		{
			int bufferIndexForPoint = (x + yTimesWidth) / 8;
			byte mask = (byte)~(128 >> (x & 7));
			Buffer[bufferIndexForPoint] &= mask;
		}

		public override void SetPixel(Point point, Color pixelColor)
		{
			if (!IsPointWithinFrameBuffer(point))
				return;
			int bufferIndexForPoint = this.GetFrameBufferIndexForPoint(point);
			if (pixelColor == Color.Black)
				this[bufferIndexForPoint] &= (byte)(uint)~this.GetPointByteMask(point);
			else
				this[bufferIndexForPoint] |= (byte)(uint)this.GetPointByteMask(point);
		}
	}
}
