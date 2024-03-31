using Iot.Device.EPaper;
using nanoFramework.UI;
using System;
using System.Collections;
using System.Device.Gpio;
using System.Device.Pwm;
using System.Drawing;
using System.Threading;
using nanoFramework.Networking;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Iot.Device.Nmea0183;
using Iot.Device.Nmea0183.Sentences;
using Iot.Device.Rtc;
using UnitsNet;
using System.Device.Spi;

namespace AutoPilotControl
{
	internal class MenuItems
	{
		private const int UDP_PORT = 10110;
		private const int LED_PIN = 10;
		private readonly GpioController _controller;
		private readonly Gdew0154M09 _display;
		private readonly Pcf8563 _rtc;

		private readonly Graphics _gfx;

		private PwmChannel _speaker;

		private GpioPin _led;
		private GpioPin _up;
		private GpioPin _down;
		private GpioPin _enter;
		private GpioPin _topButton;
		private IFont _bigFont;
		private IFont _mediumFont;

		private bool _nmeaParserRunning;

		private GeographicPosition _position;
		private double _speedKnots;
		private Angle _track = Angle.Zero;
		private double _vmgKnots;
		private Angle _bearingToWaypoint = Angle.Zero;
		private Length _distanceToWaypoint = Length.Zero;
		private Length _crossTrackError = Length.Zero;
		private string _destinationWp;
		private Angle _autopilotHeading = Angle.Zero;
		private char _autopilotStatus = ' ';
		private Angle _autopilotDesiredHeading = Angle.Zero;

		public MenuItems(GpioController controller, Pcf8563 rtc)
		{
			_controller = controller;
			_rtc = rtc;

			var connectionSettings = new SpiConnectionSettings(1, -1);

			var spiDevice = SpiDevice.Create(connectionSettings);
			_display = new Gdew0154M09(0, 15, 4, spiDevice, 9, _controller, false);
			_display.Clear(0);
			_display.SetInvertMode(true);

			_gfx = new Graphics(_display);
			_led = _controller.OpenPin(LED_PIN, PinMode.Output);
			_led.Write(PinValue.Low);
			_up = _controller.OpenPin(37, PinMode.Input);
			_enter = _controller.OpenPin(38, PinMode.Input);
			_down = _controller.OpenPin(39, PinMode.Input);
			_topButton = _controller.OpenPin(5, PinMode.Input);
			_position = new GeographicPosition();

			_bigFont = new Font16x26Reduced();
			_mediumFont = _bigFont;

			_speaker = PwmChannel.CreateFromPin(2, 1000, 0.5);
			_speaker.Start(); // To make sure it goes off
			_speaker.Stop();
			_destinationWp = string.Empty;
		}

		public void Beep()
		{
			_speaker.Start();
			Thread.Sleep(50);
			_speaker.Stop();
		}

		public void Run()
		{
			// Led should be high in idle state
			_led.Write(PinValue.High);

			Startup();

			bool exit = false;

			var mainMenu = new ArrayList();

			mainMenu.Add(new MenuEntry("NMEA Display", ShowNmeaData));

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

			_led.Write(PinValue.High);
			_display.Clear(true);

			_display.PowerOff();
			
			_gfx.Dispose(); // Disposes the display!
		}

		public void Startup()
		{
			_display.Clear(false);
			_gfx.DrawTextEx("Startup!", _bigFont, 0, 0, Color.White);
			_gfx.DrawLine(0, _bigFont.Height, 200, _bigFont.Height, Color.White);

			int y = _bigFont.Height + 2;
			_gfx.DrawTextEx("Display.[OK]", _bigFont, 0, y, Color.White);

			y += _bigFont.Height + 2;
			_gfx.DrawTextEx("Wifi...", _bigFont, 0, y, Color.White);
			_display.UpdateScreen();

			if (ConnectToWifi())
			{
				_gfx.DrawTextEx("Wifi.[OK]", _bigFont, 0, y, Color.White);
			}
			else
			{
				_gfx.DrawTextEx("Wifi.[FAIL]", _bigFont, 0, y, Color.White);
			}

			y += _bigFont.Height + 2;
			_gfx.DrawTextEx("SNTP...", _bigFont, 0, y, Color.White);
			_display.UpdateScreen();

			Sntp.Server1 = "time.windows.com";
			Sntp.Start();
			Sntp.UpdateNow();
			_gfx.DrawTextEx("SNTP." + (Sntp.IsStarted ? "[OK]" : "[FAIL]"), _bigFont, 0, y, Color.White);
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
			_gfx.DrawTextEx(title, _bigFont, 0, 10, Color.White);
			_gfx.DrawLine(0, 36, 200, 36, Color.White);
			int y = 40;
			for (int i = 0; i < menuOptions.Count; i++)
			{
				MenuEntry menuEntry = (MenuEntry)menuOptions[i];
				_gfx.DrawTextEx(menuEntry.ToString(), _bigFont, 0, y + 1, Color.White);
				y += _bigFont.Height + 2;
			}

			while (true)
			{
				int yStart = 40 + (_bigFont.Height + 2) * selectedEntry;
				_display.InverseFillRectangle(0, yStart, 200, yStart + _bigFont.Height + 2);

				_display.UpdateScreen();

				while (true)
				{
					if (_down.Read() == PinValue.Low)
					{
						Beep();
						if (selectedEntry < menuOptions.Count - 1)
						{
							// Inverse again to un-mark the selected entry
							_display.InverseFillRectangle(0, yStart, 200, yStart + _bigFont.Height + 2);
							selectedEntry++;
							break;
						}
					}

					if (_up.Read() == PinValue.Low)
					{
						Beep();
						if (selectedEntry > 0)
						{
							// Inverse again to un-mark the selected entry
							_display.InverseFillRectangle(0, yStart, 200, yStart + _bigFont.Height + 2);
							selectedEntry--;
							break;
						}
					}

					if (_enter.Read() == PinValue.Low)
					{
						Beep();
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
			if (dt.Year >= 2024)
			{
				_rtc.DateTime = dt;
			}

			string date = dt.ToString("dddd,");
			_gfx.DrawTextEx(date, _bigFont, 0, 0, Color.White);
			date = dt.ToString("dd. MMM yyyy");
			_gfx.DrawTextEx(date, _bigFont, 0, _bigFont.Height + 2, Color.White);
			string time = dt.ToString("T");
			_gfx.DrawTextEx(time, _bigFont, 20, 100 - (_bigFont.Height / 2), Color.White);
			_display.UpdateScreen();

			if (dt.Year >= 2024)
			{
				_rtc.DateTime = dt;
			}
			Thread.Sleep(2000);
		}

		private void OnBackButtonClicked(object sender, PinValueChangedEventArgs e)
		{
			_nmeaParserRunning = false;
			Beep();
		}

		private void OnNewMessage(NmeaSentence sentence)
		{
			if (sentence is RecommendedMinimumNavigationInformation rmc)
			{
				_position = rmc.Position;
				_speedKnots = RecommendedMinimumNavigationInformation.MetersPerSecondToKnots(rmc.SpeedOverGround);
				_track = rmc.TrackMadeGoodInDegreesTrue;
			}

			if (sentence is RecommendedMinimumNavToDestination rmb)
			{
				_vmgKnots = RecommendedMinimumNavigationInformation.MetersPerSecondToKnots(rmb.ApproachSpeed);
				_bearingToWaypoint = rmb.BearingToWayPoint;
				_distanceToWaypoint = rmb.DistanceToWayPoint;
				_destinationWp = rmb.NextWayPointName;
			}

			if (sentence is CrossTrackError xte)
			{
				_crossTrackError = xte.Distance;
			}

			if (sentence is HeadingAndTrackControlStatus htd)
			{
				_autopilotHeading = htd.ActualHeading;
				_autopilotStatus = htd.Status.Length >= 1 ? htd.Status[0] : ' ';
				_autopilotDesiredHeading = htd.DesiredHeading;
			}
		}

		public void ShowNmeaData()
		{
			_nmeaParserRunning = true;
			NmeaParser ps = new NmeaParser(UDP_PORT);
			ps.NewMessage += OnNewMessage;

			const int numPages = 3;
			int page = 0;

			void OnUpButtonClicked(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
			{
				if (page > 0)
				{
					page--;
				}
				else
				{
					page = numPages - 1;
				}

				Beep();
			}

			void OnDownButtonClicked(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
			{
				page = (page + 1) % numPages;
				Beep();
			}

			try
			{
				ps.StartDecode();
				_controller.RegisterCallbackForPinValueChangedEvent(_topButton.PinNumber, PinEventTypes.Falling, OnBackButtonClicked);
				_controller.RegisterCallbackForPinValueChangedEvent(_up.PinNumber, PinEventTypes.Falling, OnUpButtonClicked);
				_controller.RegisterCallbackForPinValueChangedEvent(_down.PinNumber, PinEventTypes.Falling, OnDownButtonClicked);
				bool hadData = true; // flag to avoid constantly redrawing the no-data symbol, which is expensive
				while (_nmeaParserRunning)
				{
					_display.Clear(false);
					if (ps.IsReceivingData)
					{
						int y = 0;
						if (page == 0)
						{
							ValueBlock("Latitude", _position.GetLatitudeString(), string.Empty, ref y, Color.White);
							ValueBlock("Longitude", _position.GetLongitudeString(), string.Empty, ref y, Color.White);
							ValueBlock("SOG", _speedKnots.ToString("F2"), "kts", ref y, Color.White);
							ValueBlock("TGT", _track.Degrees.ToString("F1") + "°", string.Empty, ref y, Color.White);
						}
						else if (page == 1)
						{
							ValueBlock("WPT", _destinationWp, string.Empty, ref y, Color.White);
							ValueBlock("DST to WP", _distanceToWaypoint.NauticalMiles.ToString("F2"), "nm", ref y, Color.White);
							ValueBlock("VMG", _vmgKnots.ToString("F2"), "kts", ref y, Color.White);
							ValueBlock("BRG", _bearingToWaypoint.Degrees.ToString("F1") + "°", string.Empty, ref y, Color.White);
							ValueBlock("XTE", _crossTrackError.NauticalMiles.ToString("F2"), "nm", ref y, Color.White);
						}
						else if (page == 2)
						{
							string status = _autopilotStatus switch
							{
								'S' => "Auto",
								'T' => "Track",
								'M' => "Manual",
								'W' => "Wind",
								_ => "Offline",
							};

							y = 2;
							Size textSize = _gfx.MeasureString(status, _bigFont, 0, 0);
							int startX = (_display.Width / 2) - (textSize.Width / 2); // center on screen
							_gfx.DrawTextEx(status, _bigFont, startX, y, Color.White);
							y += textSize.Height + 2;
							_display.DrawHorizontalLine(0, y - 1, 200, 0xff);
							ValueBlock("AP HDG", _autopilotHeading.Degrees.ToString("F0") + "°", string.Empty, ref y, Color.White);
							if (_autopilotStatus != ' ' && _autopilotStatus != 'M')
							{
								ValueBlock("AP DES HDG", _autopilotDesiredHeading.Degrees.ToString("F0") + "°", string.Empty, ref y, Color.White);
							}
						}

						hadData = true;

						_display.UpdateScreen();
					}
					else if (hadData)
					{
						_gfx.DrawTextEx("No data", _bigFont, 0, 0, Color.White);
						_gfx.DrawCircle(100, 100, 70, Color.White, true);
						_gfx.DrawRectangle(40, 90, 120, 20, Color.Black, true);
						hadData = false;

						_display.UpdateScreen();
					}

					// Thread.Sleep(100);
				}
			}
			finally
			{
				_controller.UnregisterCallbackForPinValueChangedEvent(_topButton.PinNumber, OnBackButtonClicked);
				_controller.UnregisterCallbackForPinValueChangedEvent(_up.PinNumber, OnUpButtonClicked);
				_controller.UnregisterCallbackForPinValueChangedEvent(_down.PinNumber, OnDownButtonClicked);
				ps.StopDecode();
				ps.Dispose();
			}
		}

		private void ValueBlock(string label, string value, string unit, ref int y, Color color)
		{
			Size labelSize = _gfx.MeasureString(label, _mediumFont, 2, 4);
			Size valueSize = _gfx.MeasureString(value, _bigFont, 2, 4);
			Size unitSize = _gfx.MeasureString(unit, _mediumFont, 0, 0);
			int maxHeight = Math.Max(labelSize.Height, valueSize.Height);
			int offset = maxHeight - unitSize.Height;

			if (labelSize.Width + valueSize.Width + unitSize.Width <= _display.Width)
			{
				// Everything fits on one line
				_gfx.DrawTextEx(label, _mediumFont, 0, y, color);
				_gfx.DrawTextEx(value, _bigFont, labelSize.Width, y, color);
				_gfx.DrawTextEx(unit, _mediumFont, labelSize.Width + valueSize.Width, y + offset, color);
				y += maxHeight;
			}
			else
			{
				_gfx.DrawTextEx(label, _mediumFont, 0, y, color);
				y += labelSize.Height;
				_gfx.DrawTextEx(value, _bigFont, 0, y, color);
				_gfx.DrawTextEx(unit, _mediumFont, valueSize.Width, y + offset, color);
				y += valueSize.Height;
			}

			_display.DrawHorizontalLine(0, y - 1, _display.Width, 0xff);
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
