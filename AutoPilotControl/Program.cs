using System;
using System.Device.Gpio;
using System.Device.Spi;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using Iot.Device.EPaper;
using Iot.Device.EPaper.Fonts;
using nanoFramework.Hardware.Esp32;
using nanoFramework.UI;

namespace AutoPilotControl
{
	public class Program
	{
		private static GpioController s_GpioController;
		private static Gdew0154M09 s_display;

		public static void Main()
		{
			s_GpioController = new GpioController();

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
			s_display.SetInvertMode(true);

			MenuItems menu = new MenuItems(s_GpioController, s_display);
			menu.Run();

			Sleep.EnableWakeupByPin(Sleep.WakeupGpioPin.Pin27, 0);
			// Sleep.StartDeepSleep(); // Works, but how to get it back alive?
		}
	}
}
