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
		private static Pcf8563 s_rtc;

		public static void Main()
		{
			// You can get the values of SpiBus
			SpiBusInfo spiBusInfo = SpiDevice.GetBusInfo(1);
			Debug.WriteLine($"{nameof(spiBusInfo.MaxClockFrequency)}: {spiBusInfo.MaxClockFrequency}");
			Debug.WriteLine($"{nameof(spiBusInfo.MinClockFrequency)}: {spiBusInfo.MinClockFrequency}");

			Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);
			Configuration.SetPinFunction(22, DeviceFunction.I2C1_CLOCK);

			Configuration.SetPinFunction(2, DeviceFunction.PWM1);
			Configuration.SetPinFunction(18, DeviceFunction.SPI1_CLOCK);
			Configuration.SetPinFunction(23, DeviceFunction.SPI1_MOSI);
			Configuration.SetPinFunction(19, DeviceFunction.SPI1_MISO);
			
			I2cDevice device = I2cDevice.Create(new I2cConnectionSettings(1, Pcf8563.DefaultI2cAddress));
			s_rtc = new Pcf8563(device);

			// Reasonable date?
			try
			{
				var rtcTime = s_rtc.DateTime;
				if (rtcTime.Year >= 2024)
				{
					Rtc.SetSystemTime(s_rtc.DateTime);
				}
			}
			catch (ArgumentOutOfRangeException)
			{
				Debug.WriteLine("RTC clock lost the correct time");
			}

			var dt = DateTime.UtcNow;
			Debug.WriteLine($"Startup Time: {dt.ToString("yyyy/MM/dd HH:mm:ss")}");
			var handler = new GpioHandling();
			handler.Init();
			MenuItems menu = new MenuItems(handler, s_rtc);
			menu.Run();
			handler.Dispose();
			Sleep.EnableWakeupByPin(Sleep.WakeupGpioPin.Pin27, 0);
			Sleep.StartDeepSleep(); // Works, but how to get it back alive?
		}
	}
}
