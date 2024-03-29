using System;
using System.Device.Gpio;
using System.Device.Spi;
using System.Drawing;
using System.Threading;
using Iot.Device.EPaper;
using Iot.Device.EPaper.Buffers;
using Iot.Device.EPaper.Drivers;

namespace AutoPilotControl
{
	public sealed class Gdew0154M09 : IDisposable, IEPaperDisplay
	{
		private readonly byte[] _initSequence =
		{
			0x00, 2, 0xdf, 0x0e, //panel setting
			0x4D, 1, 0x55, //FITIinternal code
			0xaa, 1, 0x0f,
			0xe9, 1, 0x02,
			0xb6, 1, 0x11,
			0xf3, 1, 0x0a,
			0x61, 3, 0xc8, 0x00, 0xc8, //resolution setting
			0x60, 1, 0x00, //Tcon setting
			0xe3, 1, 0x00,
			0x04, 0, //Power on
			// 0x00, 0x82, 0xff, 0x0e, 160, //panel setting (with delay bit)

			0x20, 56, 0x01, 0x04, 0x04, 0x03, 0x01, 0x01, 0x01,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x21, 42, 0x01, 0x04, 0x04, 0x03, 0x01, 0x01, 0x01,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x22, 56, 0x01, 0x84, 0x84, 0x83, 0x01, 0x01, 0x01,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x23, 56, 0x01, 0x44, 0x44, 0x43, 0x01, 0x01, 0x01,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x24, 56, 0x01, 0x04, 0x04, 0x03, 0x01, 0x01, 0x01,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0xFF, 0xFF, // end
		};

		private readonly GpioPin _resetPin;
		private readonly GpioPin _commandDataPin;
		private readonly GpioPin _busyPin;
		private readonly SpiDevice _bus;
		private readonly GpioPin _csPin;
		private readonly GpioController _controller;
		private readonly bool _shouldDispose;

		private int _drawPositionX;
		private int _drawPositionY;

		private FrameBuffer1BitPerPixel _bitBuffer;

		public Gdew0154M09(int resetPin, int commandDataPin, int busyPin, SpiDevice bus, int csPin = -1, GpioController controller = null, bool shouldDispose = true)
		{
			_bus = bus ?? throw new ArgumentNullException(nameof(bus));
			if (controller != null)
			{
				_controller = controller;
				_shouldDispose = shouldDispose;
			}
			else
			{
				_controller = new GpioController();
				_shouldDispose = true;
			}

			_drawPositionX = 0;
			_drawPositionY = 0;
			_resetPin = _controller.OpenPin(resetPin, PinMode.Output);
			_resetPin.Write(PinValue.High); // Reset is active low, and we're not resetting by default
			_commandDataPin = _controller.OpenPin(commandDataPin, PinMode.Output);
			_commandDataPin.Write(PinValue.High);
			_busyPin = _controller.OpenPin(busyPin, PinMode.Input);
			if (csPin != -1)
			{
				// Let's cheat a bit: We can just keep CS low if there's only one device attached to the bus (which is normally the case)
				_csPin = _controller.OpenPin(csPin, PinMode.Output);
				_csPin.Write(PinValue.Low);
			}

			int bits = Width * Height / 8;

			_bitBuffer = new FrameBuffer1BitPerPixel(Height, Width);
			_bitBuffer.StartPoint = new Point(0, 0);
			////_resetPin.Write(PinValue.Low);
			////Thread.Sleep(100); // It's not documented how long we have to wait
			////_resetPin.Write(PinValue.High);
			////Thread.Sleep(10);
			RunInitSequence();
		}

		public int Width => 200;
		public int Height => 200;

		public IFrameBuffer FrameBuffer => _bitBuffer;
		public bool PagedFrameDrawEnabled => false;

		public bool WaitReady(int waitingTime, CancellationTokenSource cancellationToken)
		{
			if (cancellationToken == default)
            {
                if (waitingTime < 0)
                {
                    throw new ArgumentNullException(nameof(cancellationToken), $"{nameof(cancellationToken)} cannot be null with {nameof(waitingTime)} < 0");
                }

                cancellationToken = new CancellationTokenSource(waitingTime);
            }

            while (!cancellationToken.IsCancellationRequested && _busyPin.Read() == PinValue.Low)
            {
                cancellationToken.Token.WaitHandle.WaitOne(5, true);
            }

            return !cancellationToken.IsCancellationRequested;
		}

		public void WaitReady()
		{
			WaitReady(1000, null);
		}

		public void BeginFrameDraw()
		{
			// Nothing to do?
		}

		public bool NextFramePage()
		{
			throw new NotImplementedException();
		}

		public void EndFrameDraw()
		{
			UpdateScreen();
		}

		private void WriteCommand(byte commandByte)
		{
			WaitReady();
			_commandDataPin.Write(PinValue.Low);
			_bus.WriteByte(commandByte);
			_commandDataPin.Write(PinValue.High);
		}

		private void WriteData(byte dataByte)
		{
			_commandDataPin.Write(PinValue.High);
			_bus.WriteByte(dataByte);
		}

		/// <summary>
		/// Inverts the display (black becomes white and white becomes black)
		/// </summary>
		/// <param name="invert">True = Background is white, false = Background is black</param>
		public void SetInvertMode(bool invert)
		{
			if (invert)
			{
				WriteCommandSequence(0x50, 0xC7);
			}
			else
			{
				WriteCommandSequence(0x50, 0xD7);
			}
		}

		private void RunInitSequence()
		{
			int addr = 0;
			while (true)
			{
				byte cmd = _initSequence[addr++];
				int num = _initSequence[addr++]; // Number of args to follow
				if (cmd == 0xFF && num == 0xFF)
				{
					return;
				}

				WriteCommand(cmd); // Read, issue command
				int ms = num & 0x80; // If hibit set, delay follows args
				num = (num & ~0x80); // Mask out delay bit
				if (num != 0)
				{
					do
					{
						// For each argument...
						WriteData(_initSequence[addr++]); // Read, issue argument
					} while (--num > 0);
				}

				if (ms > 0)
				{
					ms = _initSequence[addr++]; // Read post-command delay time (ms)
					Thread.Sleep((ms == 255 ? 500 : ms));
				}

				WaitReady();
			}
		}

		public void PowerOn()
		{
			WriteCommandSequence(0x04);
		}

		public void PowerOff()
		{
			WriteCommandSequence(0x02);
		}

		public void WriteCommandSequence(byte cmd, params byte[] data)
		{
			WaitReady();
			WriteCommand(cmd);
			for (int i = 0; i < data.Length; i++)
			{
				WriteData(data[i]);
			}
		}

		public void Clear(byte pattern = 0)
		{
			for (int i = 0; i < _bitBuffer.BufferByteCount; i++)
			{
				_bitBuffer[i] = pattern;
			}
			UpdateScreen();
		}

		public void Clear(bool triggerPageRefresh = false)
		{
			for (int i = 0; i < _bitBuffer.BufferByteCount; i++)
			{
				_bitBuffer[i] = 0;
			}

			if (triggerPageRefresh)
			{
				UpdateScreen();
			}
		}

		public void Flush()
		{
			UpdateScreen();
		}

		public bool PerformFullRefresh()
		{
			UpdateScreen();
			return true;
		}

		public bool PerformPartialRefresh()
		{
			// TODO: This should be possible in a more performant way
			UpdateScreen();
			return true;
		}

		public void SetPosition(int x, int y)
		{
			_drawPositionX = x;
			_drawPositionY = y;
		}

		public void DrawPixel(int x, int y, Color color)
		{
			SetPixel(x, y, color == Color.Black ? 0 : 1);
		}

		public void SendCommand(params byte[] command)
		{
			if (command == null)
			{
				throw new ArgumentNullException();
			}

			if (command.Length != 1)
			{
				throw new ArgumentException();
			}

			WriteCommand(command[0]);
		}

		public void SendData(params byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException();
			}

			for (int i = 0; i < data.Length; i++)
			{
				WriteData(data[i]);
			}
		}

		public void SetPixel(int x, int y, int color)
		{
			// Ignore out-of-bounds drawing (simplifies clipping)
			if (x < 0 || x >= Width)
			{
				return;
			}

			if (y < 0 || y >= Height)
			{
				return;
			}

			int byteOffset = (y * 25) + (x / 8);
			int bit = x % 8;
			bit = 0x80 >> bit;
			byte b = _bitBuffer[byteOffset];
			if (color > 0)
			{
				// set pixel black
				b |= (byte)bit;
			}
			else
			{
				b &= (byte)~bit;
			}

			_bitBuffer[byteOffset] = b;
		}

		public void UpdateScreen()
		{
			WaitReady();
			WriteCommand(0x13);
			_bus.Write(_bitBuffer.Buffer);

			Thread.Sleep(2);

			WriteCommand(0x12);
			WaitReady();
		}

		/// <summary>
		/// Inverses a block of pixels. startX and endX must be a multiple of 8
		/// </summary>
		public void InverseFillRectangle(int startX, int startY, int endX, int endY)
		{
			for (int currentY = startY; currentY != endY; currentY++)
			{
				for (int xx = startX / 8; xx != endX / 8; xx++)
				{
					int index = xx + (currentY * 25);
					int b = _bitBuffer[index];
					_bitBuffer[index] = (byte)(b ^ 0xff);
				}
			}
		}

		public void Dispose()
		{
			_resetPin?.Dispose();
			_commandDataPin?.Dispose();
			_busyPin?.Dispose();
			_csPin?.Dispose();
			if (_shouldDispose)
			{
				_controller.Dispose();
			}
		}
	}
}
