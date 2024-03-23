using Iot.Device.EPaper;
using nanoFramework.UI;
using System;
using System.Collections;
using System.Device.Gpio;
using System.Drawing;
using System.Threading;

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

			DrawStartupPage();

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

		public void DrawStartupPage()
		{
			_display.Clear(false);
			_gfx.DrawTextEx("Startup!", _font, 10, 10, Color.White);
			_display.UpdateScreen();
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
			string time = DateTime.UtcNow.ToString("T");
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
