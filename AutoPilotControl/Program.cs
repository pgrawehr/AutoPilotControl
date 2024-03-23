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
		private const int LED_PIN = 10;
		private static GpioController s_GpioController;
		private static Gdew0154M09 s_display;
		private static IFont s_font;

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
			s_display.SetInvertMode(true);

			s_font = new Font16x26();

			DrawStartupPage();

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

		public static void DrawStartupPage()
		{
			s_display.Clear(false);
			using var gfx = new Graphics(s_display);
			gfx.DrawTextEx("Hello World!", s_font, 10, 10, Color.White);
			string chars = string.Empty;
			for (char i = '\u0001'; i < 256; i++)
			{
				chars = chars + i;
			}
			gfx.DrawTextEx(chars, s_font, 0, 25, Color.White);
			s_display.UpdateScreen();
		}
	}
}
