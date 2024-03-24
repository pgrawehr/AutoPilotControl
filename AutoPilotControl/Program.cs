using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Device.Spi;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using Iot.Device.EPaper;
using Iot.Device.EPaper.Fonts;
using Iot.Device.Rtc;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Runtime.Native;
using nanoFramework.UI;

namespace AutoPilotControl
{
	public class Program
	{
		private static GpioController s_GpioController;
		private static Gdew0154M09 s_display;
		private static Pcf8563 s_rtc;

		public static void Main()
		{
			s_GpioController = new GpioController();

			SpiDevice spiDevice;
			SpiConnectionSettings connectionSettings;
			// You can get the values of SpiBus
			SpiBusInfo spiBusInfo = SpiDevice.GetBusInfo(1);
			Debug.WriteLine($"{nameof(spiBusInfo.MaxClockFrequency)}: {spiBusInfo.MaxClockFrequency}");
			Debug.WriteLine($"{nameof(spiBusInfo.MinClockFrequency)}: {spiBusInfo.MinClockFrequency}");

			Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);
			Configuration.SetPinFunction(22, DeviceFunction.I2C1_CLOCK);

			I2cDevice device = I2cDevice.Create(new I2cConnectionSettings(1, Pcf8563.DefaultI2cAddress));
			s_rtc = new Pcf8563(device);

			// Reasonable date?
			if (s_rtc.DateTime.Year >= 2024)
			{
				Rtc.SetSystemTime(s_rtc.DateTime);
			}
			

			var dt = DateTime.UtcNow;
			Debug.WriteLine($"Startup Time: {dt.ToString("yyyy/MM/dd HH:mm:ss")}");

			Configuration.SetPinFunction(18, DeviceFunction.SPI1_CLOCK);
			Configuration.SetPinFunction(23, DeviceFunction.SPI1_MOSI);
			Configuration.SetPinFunction(19, DeviceFunction.SPI1_MISO);
			connectionSettings = new SpiConnectionSettings(1, -1);

			spiDevice = SpiDevice.Create(connectionSettings);
			s_display = new Gdew0154M09(0, 15, 4, spiDevice, 9, s_GpioController, false);
			s_display.Clear(0);
			s_display.SetInvertMode(true);

			MenuItems menu = new MenuItems(s_GpioController, s_display, s_rtc);
			menu.Run();

			Sleep.EnableWakeupByPin(Sleep.WakeupGpioPin.Pin27, 0);
			// Sleep.StartDeepSleep(); // Works, but how to get it back alive?
		}
	}
}
