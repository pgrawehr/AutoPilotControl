using Iot.Device.EPaper;
using nanoFramework.UI;
using System;
using System.Collections;
using System.Device.Gpio;
using System.Drawing;
using System.Threading;
using nanoFramework.Networking;
using System.Diagnostics;

namespace AutoPilotControl
{
	internal class MenuItems
	{
		private const int LED_PIN = 10;
		private readonly GpioController _controller;
		private readonly Gdew0154M09 _display;

		private readonly Graphics _gfx;

		private GpioPin _led;
		private GpioPin _up;
		private GpioPin _down;
		private GpioPin _enter;
		private GpioPin _topButton;
		private Font16x26 _font;

		public MenuItems(GpioController controller, Gdew0154M09 display)
		{
			_controller = controller;
			_display = display;
			_gfx = new Graphics(_display);
			_led = _controller.OpenPin(LED_PIN, PinMode.Output);
			_led.Write(PinValue.High);
			_up = _controller.OpenPin(37, PinMode.Input);
			_enter = _controller.OpenPin(38, PinMode.Input);
			_down = _controller.OpenPin(39, PinMode.Input);
			_topButton = _controller.OpenPin(5, PinMode.Input);
		}

		public void Run()
		{
			// Led should be high in idle state
			_led.Write(PinValue.High);
			_font = new Font16x26();

			Startup();

			bool exit = false;

			var mainMenu = new ArrayList();
			mainMenu.Add(new MenuEntry("Blink Led", () =>
			{
				for (int i = 0; i < 10; i++)
				{
					_led.Toggle();
					Thread.Sleep(200);
				}

				_led.Write(PinValue.High);
			}));

			mainMenu.Add(new MenuEntry("Show Clock", ShowClock));

			mainMenu.Add(new MenuEntry("Exit", () => exit = true));

			while (!exit)
			{
				ShowMenu("Main menu", mainMenu);
			}

			_led.Write(PinValue.Low);
			_display.Clear(true);
			_gfx.Dispose();
		}

		public void Startup()
		{
			_display.Clear(false);
			_gfx.DrawTextEx("Startup!", _font, 0, 0, Color.White);
			_gfx.DrawLine(0, _font.Height, 200, _font.Height, Color.White);

			int y = _font.Height + 2;
			_gfx.DrawTextEx("Display.[OK]", _font, 0, y, Color.White);

			y += _font.Height + 2;
			_gfx.DrawTextEx("Wifi...", _font, 0, y, Color.White);
			_display.UpdateScreen();

			if (ConnectToWifi())
			{
				_gfx.DrawTextEx("Wifi.[OK]", _font, 0, y, Color.White);
			}
			else
			{
				_gfx.DrawTextEx("Wifi.[FAIL]", _font, 0, y, Color.White);
			}

			y += _font.Height + 2;
			_gfx.DrawTextEx("SNTP...", _font, 0, y, Color.White);
			_display.UpdateScreen();

			Sntp.Server1 = "time.windows.com";
			Sntp.Start();
			Sntp.UpdateNow();
			_gfx.DrawTextEx("SNTP." + (Sntp.IsStarted ? "[OK]" : "[FAIL]"), _font, 0, y, Color.White);
			_display.UpdateScreen();

			Thread.Sleep(2000);

			ShowClock();
		}

		private bool ConnectToWifi()
		{
			// Give 60 seconds to the wifi join to happen
			CancellationTokenSource cs = new(60000);
			// Use connect here to set up the wifi password for the first time (but DO NOT COMMIT!!!)
			var success = WifiNetworkHelper.Reconnect(requiresDateTime: false, token: cs.Token);
			if (!success)
			{
				// Something went wrong, you can get details with the ConnectionError property:
				Debug.WriteLine($"Can't connect to the network, error: {WifiNetworkHelper.Status}");
				if (WifiNetworkHelper.HelperException != null)
				{
					Debug.WriteLine($"ex: {WifiNetworkHelper.HelperException}");
				}

				return false;
			}
			else
			{
				Debug.WriteLine("Connected to Wifi.");
				return true;
			}
		}

		private void ShowMenu(String title, ArrayList menuOptions)
		{
			int selectedEntry = 0;
			_display.Clear(false);
			_gfx.DrawTextEx(title, _font, 0, 10, Color.White);
			_gfx.DrawLine(0, 36, 200, 36, Color.White);
			int y = 40;
			for (int i = 0; i < menuOptions.Count; i++)
			{
				MenuEntry menuEntry = (MenuEntry)menuOptions[i];
				_gfx.DrawTextEx(menuEntry.ToString(), _font, 0, y + 1, Color.White);
				y += _font.Height + 2;
			}

			while (true)
			{
				int yStart = 40 + (_font.Height + 2) * selectedEntry;
				_display.InverseFillRectangle(0, yStart, 200, yStart + _font.Height + 2);

				_display.UpdateScreen();

				while (true)
				{
					if (_down.Read() == PinValue.Low)
					{
						if (selectedEntry < menuOptions.Count - 1)
						{
							// Inverse again to un-mark the selected entry
							_display.InverseFillRectangle(0, yStart, 200, yStart + _font.Height + 2);
							selectedEntry++;
							break;
						}
					}

					if (_up.Read() == PinValue.Low)
					{
						if (selectedEntry > 0)
						{
							// Inverse again to un-mark the selected entry
							_display.InverseFillRectangle(0, yStart, 200, yStart + _font.Height + 2);
							selectedEntry--;
							break;
						}
					}

					if (_enter.Read() == PinValue.Low)
					{
						MenuEntry menuEntry = (MenuEntry)menuOptions[selectedEntry];
						menuEntry.Execute();
						return;
					}
				}
			}
		}

		public void ShowClock()
		{
			_display.Clear(false);
			var dt = DateTime.UtcNow;
			string date = dt.ToString("dddd,");
			_gfx.DrawTextEx(date, _font, 0, 0, Color.White);
			date = dt.ToString("dd. MMM yyyy");
			_gfx.DrawTextEx(date, _font, 0, _font.Height + 2, Color.White);
			string time = dt.ToString("T");
			_gfx.DrawTextEx(time, _font, 20, 100 - (_font.Height / 2), Color.White);
			_display.UpdateScreen();
			Thread.Sleep(2000);
		}

		private sealed class MenuEntry
		{
			private readonly string _name;
			private readonly Action _action;

			public MenuEntry(string name, Action action)
			{
				_name = name;
				_action = action;
			}

			public override string ToString()
			{
				return _name;
			}

			public void Execute()
			{
				_action();
			}
		}
	}
}
