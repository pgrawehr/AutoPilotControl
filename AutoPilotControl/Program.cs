using System;
using System.Device.Gpio;
using System.Device.Spi;
using System.Diagnostics;
using System.Threading;
using nanoFramework.Hardware.Esp32;

namespace AutoPilotControl
{
	public class Program
	{
		private const int LED_PIN = 10;
		private static GpioController s_GpioController;
		private static Gdew0154M09 s_display;
		public static void Main()
		{
			s_GpioController = new GpioController();

			var led = s_GpioController.OpenPin(LED_PIN, PinMode.Output);

			var topButton = s_GpioController.OpenPin(5, PinMode.Input);

			led.Write(PinValue.Low);

			SpiDevice spiDevice;
			SpiConnectionSettings connectionSettings;
			Debug.WriteLine("Hello from sample for System.Device.Spi!");
			// You can get the values of SpiBus
			SpiBusInfo spiBusInfo = SpiDevice.GetBusInfo(1);
			Debug.WriteLine($"{nameof(spiBusInfo.MaxClockFrequency)}: {spiBusInfo.MaxClockFrequency}");
			Debug.WriteLine($"{nameof(spiBusInfo.MinClockFrequency)}: {spiBusInfo.MinClockFrequency}");

			Configuration.SetPinFunction(18, DeviceFunction.SPI1_CLOCK);
			Configuration.SetPinFunction(23, DeviceFunction.SPI1_MOSI);
			Configuration.SetPinFunction(19, DeviceFunction.SPI1_MISO);
			connectionSettings = new SpiConnectionSettings(1, -1);

			spiDevice = SpiDevice.Create(connectionSettings);
			s_display = new Gdew0154M09(0, 15, 4, spiDevice, 9, s_GpioController, false);
			s_display.Clear(0);

			// Draw a cross
			for (int i = 0; i < s_display.Width; i++)
			{
				s_display.SetPixel(i, i, 1);
				s_display.SetPixel(s_display.Width - i - 1, i, 1);
			}

			s_display.UpdateScreen();

			while (topButton.Read() == PinValue.High)
			{
				led.Toggle();
				Thread.Sleep(125);
				led.Toggle();
				Thread.Sleep(125);
				led.Toggle();
				Thread.Sleep(125);
				led.Toggle();
				Thread.Sleep(525);
			}

			led.Write(PinValue.Low);
			Sleep.EnableWakeupByPin(Sleep.WakeupGpioPin.Pin27, 0);
			// Sleep.StartDeepSleep(); // Works, but how to get it back alive?
		}
	}
}
