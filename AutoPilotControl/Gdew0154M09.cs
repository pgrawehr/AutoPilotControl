using System;
using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;

namespace AutoPilotControl
{
	public sealed class Gdew0154M09 : IDisposable
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

		private byte[] _bitBuffer;

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

			_bitBuffer = new byte[bits];
			RunInitSequence();
		}

		public int Width => 200;
		public int Height => 200;

		private void WaitNotBusy()
		{
			while (_busyPin.Read() == PinValue.Low)
			{
				Thread.Sleep(1);
			}
		}

		private void WriteCommand(byte commandByte)
		{
			WaitNotBusy();
			_commandDataPin.Write(PinValue.Low);
			_bus.WriteByte(commandByte);
			_commandDataPin.Write(PinValue.High);
		}

		private void WriteData(byte dataByte)
		{
			_commandDataPin.Write(PinValue.High);
			_bus.WriteByte(dataByte);
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

				WaitNotBusy();
			}
		}

		public void Clear(byte pattern = 0)
		{
			for (int i = 0; i < _bitBuffer.Length; i++)
			{
				_bitBuffer[i] = pattern;
			}
			UpdateScreen();
		}

		public void SetPixel(int x, int y, int color)
		{
			if (x < 0 || x >= Width)
			{
				throw new ArgumentOutOfRangeException(nameof(x));
			}

			if (y < 0 || y >= Height)
			{
				throw new ArgumentOutOfRangeException(nameof(y));
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
			WaitNotBusy();
			WriteCommand(0x10);
			for (int i = 0; i < _bitBuffer.Length; i++)
			{
				WriteData(_bitBuffer[i]);
			}

			Thread.Sleep(2);
			WaitNotBusy();
			WriteCommand(0x13);
			for (int i = 0; i < _bitBuffer.Length; i++)
			{
				WriteData(_bitBuffer[i]);
			}

			Thread.Sleep(2);

			WriteCommand(0x12);
			WaitNotBusy();
			Thread.Sleep(10);
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
