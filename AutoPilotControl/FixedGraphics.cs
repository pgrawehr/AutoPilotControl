using System;
using Iot.Device.EPaper.Drivers;
using nanoFramework.UI;
using System.Drawing;
using Iot.Device.EPaper;

namespace AutoPilotControl
{
	internal static class FixedGraphics
	{
		/// <summary>
		/// Writes text to the display.
		/// </summary>
		/// <param name="gfx">The graphics library to fix</param>
		/// <param name="text">The text to write.</param>
		/// <param name="font">The font to use.</param>
		/// <param name="x">Starting X point.</param>
		/// <param name="y">Starting Y point.</param>
		/// <param name="color">The font color.</param>
		public static void DrawTextEx(this Graphics gfx, string text, IFont font, int x, int y, Color color)
		{
			var col = 0;
			var line = 0;
			int bytesPerChar = (font.Width / 8) + ((font.Width % 8) == 0 ? 0 : 1);

			foreach (char character in text)
			{
				if (col >= gfx.EPaperDisplay.Width)
				{
					col = 0;
					line += font.Height + 1;
				}

				var characterBitmap = font[character];
				if (bytesPerChar > 1)
				{
					for (var i = 0; i < font.Height; i++)
					{
						var xPos = x + col;
						var yPos = y + line + i;
						var bitMask = 0x01;
						uint b;
						if (bytesPerChar == 2)
						{
							b = BitConverter.ToUInt16(characterBitmap, i * 2);
						}
						else if (bytesPerChar == 3)
						{
							b = (uint)(characterBitmap[i * 3] | characterBitmap[i * 3 + 1] << 8 | characterBitmap[i * 3 + 2] << 16); 
						}
						else
						{
							b = BitConverter.ToUInt32(characterBitmap, i * 4);
						}

						for (var pixel = 0; pixel < font.Width; pixel++)
						{
							if ((b & bitMask) > 0)
							{
								gfx.DrawPixel(xPos + pixel, yPos, color);
							}

							bitMask <<= 1;
						}
					}
				}
				else
				{
					for (var i = 0; i < font.Height; i++)
					{
						var xPos = x + col;
						var yPos = y + line + i;
						var bitMask = 0x01;
						var b = characterBitmap[i];

						for (var pixel = 0; pixel < 8; pixel++)
						{
							if ((b & bitMask) > 0)
							{
								gfx.DrawPixel(xPos + pixel, yPos, color);
							}

							bitMask <<= 1;
						}
					}
				}

				col += font.Width;
			}
		}
	}
}
