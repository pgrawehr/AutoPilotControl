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
using Iot.Device.EPaper.Buffers;
using Point = System.Drawing.Point;

namespace AutoPilotControl
{
	internal class MenuItems
	{
		private const int UDP_PORT = 10101;
		private readonly Gdew0154M09 _display;
		private readonly Pcf8563 _rtc;
		private readonly GpioHandling _pinHandling;

		private readonly Graphics _gfx;

		private IFont _bigFont;
		private IFont _mediumFont;

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
		private bool _autopilotDesiredHeadingValid = false;

		public MenuItems(GpioHandling pinHandling, Pcf8563 rtc)
		{
			_pinHandling = pinHandling;
			_rtc = rtc;

			var connectionSettings = new SpiConnectionSettings(1, -1);

			var spiDevice = SpiDevice.Create(connectionSettings);
			_display = new Gdew0154M09(0, 15, 4, spiDevice, 9, new GpioController(), false);
			_display.Clear(true);
			_display.SetInvertMode(true);

			_gfx = new Graphics(_display);
			
			_position = new GeographicPosition();

			_bigFont = new Font16x26Reduced();
			_mediumFont = _bigFont;

			_destinationWp = string.Empty;
		}

		

		public void Run()
		{
			Startup();

			bool exit = false;

			var mainMenu = new ArrayList();

			mainMenu.Add(new MenuEntry("NMEA Display", ParserMode));

			mainMenu.Add(new MenuEntry("Blink Led", () =>
			{
				_pinHandling.BlinkLed(500, 10);
			}));

			mainMenu.Add(new MenuEntry("Show Clock", ShowClock));

			mainMenu.Add(new MenuEntry("Exit", () => exit = true));

			while (!exit)
			{
				ShowMenu("Main menu", mainMenu);
			}

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

		private void WaitNoButtonsPressed()
		{
			_pinHandling.WaitNoButtonsPressed();
		}

		private void ShowMenu(string title, ArrayList menuOptions)
		{
			IFrameBuffer dummy = null;
			ShowMenu(title, menuOptions, false, ref dummy);
		}

		private void ShowMenu(String title, ArrayList menuOptions, bool doSaveBuffer, ref IFrameBuffer menuRestoreBuffer)
		{
			int selectedEntry = 0;
			if (menuRestoreBuffer == null)
			{
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

				if (doSaveBuffer)
				{
					menuRestoreBuffer = _display.CloneFrameBuffer();
				}
			}
			else
			{
				_gfx.DrawBitmap(menuRestoreBuffer);
			}

			while (true)
			{
				int yStart = 40 + (_bigFont.Height + 2) * selectedEntry;
				_display.InverseFillRectangle(0, yStart, 200, yStart + _bigFont.Height + 2);

				_display.UpdateScreen();

				// Debouncing helper (initially set, as we expect no buttons to be pressed when a menu is first displayed)
				WaitNoButtonsPressed();

				while (true)
				{
					if (_pinHandling.DownButtonWasClicked(true))
					{
						if (selectedEntry < menuOptions.Count - 1)
						{
							// Inverse again to un-mark the selected entry
							_display.InverseFillRectangle(0, yStart, 200, yStart + _bigFont.Height + 2);
							selectedEntry++;
							break;
						}
					}

					if (_pinHandling.UpButtonWasClicked(true))
					{
						if (selectedEntry > 0)
						{
							// Inverse again to un-mark the selected entry
							_display.InverseFillRectangle(0, yStart, 200, yStart + _bigFont.Height + 2);
							selectedEntry--;
							break;
						}
					}

					if (_pinHandling.EnterButtonWasClicked())
					{
						MenuEntry menuEntry = (MenuEntry)menuOptions[selectedEntry];
						menuEntry.Execute();
						WaitNoButtonsPressed();
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
				_autopilotDesiredHeadingValid = htd.DesiredHeadingValid;
			}
		}

		private Angle AddAngle(Angle angle, Angle correction)
		{
			double newValue = angle.Degrees + correction.Degrees;
			if (correction.Degrees > 0)
			{
				newValue = Math.Ceiling(newValue);
			}
			else
			{
				newValue = Math.Floor(newValue);
			}

			while (newValue < 0)
			{
				newValue += 360;
			}

			while (newValue >= 360)
			{
				newValue -= 360;
			}

			return Angle.FromDegrees(newValue);
		}

		private void AutopilotControlMenu(NmeaParser ps)
		{
			bool leave = false;
			bool enterMenu = false;
			_display.Clear(false);

			bool cancel = false;

			string lastStatus = string.Empty;

			bool hasValidDesiredHeading = false;
			Angle desiredHeading = Angle.Zero;
			bool narrowDeadbandLimit = false;
			bool firstTime = true;

			int correctionValue = 0;

			ArrayList modeMenu = new ArrayList();
			modeMenu.Add(new MenuEntry("Standby", () => SendHeadingCorrection('M', Angle.Zero, false, false, ps)));
			modeMenu.Add(new MenuEntry("Auto", () => SendHeadingCorrection('S', Angle.Zero, false, false, ps)));
			modeMenu.Add(new MenuEntry("Track", () => SendHeadingCorrection('T', Angle.Zero, false, false, ps)));
			modeMenu.Add(new MenuEntry("Wind", () => SendHeadingCorrection('W', Angle.Zero, false, false, ps)));
			modeMenu.Add(new MenuEntry("Back", () => { }));

			// Only calculate the menu once (ideally even right here, at startup)
			IFrameBuffer menuBackupBuffer = null;
			while (!leave)
			{
				int y = 2;
				var status = AutopilotStatusString();

				Size textSize = _gfx.MeasureString(status, _bigFont, 0, 0);
				int startX = (_display.Width / 2) - (textSize.Width / 2); // center on screen
				_display.FastFillRectangle(0, y, _display.Width, textSize.Height + y, 0);
				_gfx.DrawTextEx(status, _bigFont, startX, y, Color.White);

				y += textSize.Height + 2;
				ValueBlock(false, firstTime, "Current HDG", _autopilotHeading.Degrees.ToString("F0") + "°",
					string.Empty, ref y, ref cancel, Color.White);
				string desHdgString = hasValidDesiredHeading ? desiredHeading.Degrees.ToString("F0") + "°" : "N/A";
				ValueBlock(false, firstTime, "Desired HDG", desHdgString, string.Empty, ref y, ref cancel, Color.White);

				firstTime = false;
				_display.UpdateScreen();

				if (enterMenu)
				{
					ShowMenu("Change Mode", modeMenu, true, ref menuBackupBuffer);
					enterMenu = false;
					Interlocked.Exchange(ref correctionValue, 0);
					_display.Clear(false);
					firstTime = true;
					cancel = false;
				}

				if (IsAutopilotActive())
				{
					if (_autopilotDesiredHeadingValid)
					{
						desiredHeading = _autopilotDesiredHeading;
						hasValidDesiredHeading = true;
					}
				}
				else
				{
					narrowDeadbandLimit = false;
				}

				int delta = Interlocked.Exchange(ref correctionValue, 0);
				if (delta != 0)
				{
					SendHeadingCorrection(_autopilotStatus, AddAngle(desiredHeading, Angle.FromDegrees(delta)), true,
						narrowDeadbandLimit, ps);
				}
			}
		}

		private void SendHeadingCorrection(char newStatus, Angle newAngle, bool newAngleValid, bool narrowDeadBandMode, NmeaParser ps)
		{
			if (newAngleValid)
			{
				Debug.WriteLine($"Remaining in status {newStatus}, delta: {newAngle.Degrees}");
			}
			else
			{
				Debug.WriteLine($"Setting new status {newStatus}");
			}

			var msg = new HeadingAndTrackControl(newStatus.ToString(), Angle.Zero, string.Empty, string.Empty, Angle.Zero, 
				narrowDeadBandMode ? Angle.Zero : Angle.FromDegrees(10), Length.Zero, 1, newAngle, newAngleValid,
				Length.Zero, newAngle, false);
			ps.SendMessage(msg);
		}

		public void ParserMode()
		{
			NmeaParser ps = new NmeaParser(UDP_PORT);

			bool leave = false;
			ArrayList modes = new ArrayList();
			modes.Add(new MenuEntry("GPS Status", () => ShowNmeaData(0, ps)));
			modes.Add(new MenuEntry("NAV Status", () => ShowNmeaData(1, ps)));
			modes.Add(new MenuEntry("AP Status", () => ShowNmeaData(2, ps)));
			modes.Add(new MenuEntry("AP Control", () => AutopilotControlMenu(ps)));
			modes.Add(new MenuEntry("Back", () => leave = true));

			try
			{
				ps.NewMessage += OnNewMessage;
				ps.StartDecode();

				while (!leave)
				{
					ShowMenu("NMEA Display", modes);
				}
			}
			finally
			{
				ps.StopDecode();
				ps.Dispose();
			}
		}

		public void ShowNmeaData(int startPage, NmeaParser parser)
		{
			const int numPages = 3;
			int page = startPage % numPages;
			bool fullRefreshPending = true;
			bool nmeaParserRunning = true; // Require a full refresh. Also used as interrupt signal

			bool hadData = true; // flag to avoid constantly redrawing the no-data symbol, which is expensive
			while (nmeaParserRunning)
			{
				bool fullRefresh = false;
				if (_pinHandling.UpButtonWasClicked(true))
				{
					if (page > 0)
					{
						page--;
					}
					else
					{
						page = numPages - 1;
					}

					fullRefreshPending = true;
				}

				if (_pinHandling.DownButtonWasClicked(true))
				{
					page = (page + 1) % numPages;
					fullRefreshPending = true;
				}

				if (_pinHandling.BackButtonWasClicked())
				{
					nmeaParserRunning = false;
					continue;
				}

				if (fullRefreshPending)
				{
					_display.Clear(false);
					fullRefresh = true;
					fullRefreshPending = false;
					Debug.WriteLine($"Full refresh, next page: {page}");
				}

				if (parser.IsReceivingData)
				{
					int y = 0;
					if (page == 0)
					{
						ValueBlock(false, fullRefresh, "Latitude", _position.GetLatitudeString(), string.Empty, ref y, ref fullRefreshPending, Color.White);
						ValueBlock(false, fullRefresh, "Longitude", _position.GetLongitudeString(), string.Empty, ref y, ref fullRefreshPending, Color.White);
						ValueBlock(true, fullRefresh, "SOG", _speedKnots.ToString("F2"), "kts", ref y, ref fullRefreshPending, Color.White);
						ValueBlock(true, fullRefresh, "TGT", _track.Degrees.ToString("F1") + "°", string.Empty, ref y, ref fullRefreshPending, Color.White);
					}
					else if (page == 1)
					{
						ValueBlock(true, fullRefresh, "WPT", _destinationWp, string.Empty, ref y, ref fullRefreshPending, Color.White);
						ValueBlock(true, fullRefresh, "WP DST", _distanceToWaypoint.NauticalMiles.ToString("F2"), "nm", ref y, ref fullRefreshPending, Color.White);
						ValueBlock(true, fullRefresh, "VMG", _vmgKnots.ToString("F2"), "kts", ref y, ref fullRefreshPending, Color.White);
						ValueBlock(true, fullRefresh, "BRG", _bearingToWaypoint.Degrees.ToString("F1") + "°", string.Empty, ref y, ref fullRefreshPending, Color.White);
						ValueBlock(true, fullRefresh, "XTE", _crossTrackError.NauticalMiles.ToString("F2"), "nm", ref y, ref fullRefreshPending, Color.White);
					}
					else if (page == 2)
					{
						var status = AutopilotStatusString();

						y = 2;
						Size textSize = _gfx.MeasureString(status, _bigFont, 0, 0);
						int startX = (_display.Width / 2) - (textSize.Width / 2); // center on screen
						_display.FastFillRectangle(0, y, _display.Width, textSize.Height + y, 0);
						_gfx.DrawTextEx(status, _bigFont, startX, y, Color.White);
						y += textSize.Height + 2;
						_display.DrawHorizontalLine(0, y - 1, 200, 0xff);
						ValueBlock(true, fullRefresh, "AP HDG", _autopilotHeading.Degrees.ToString("F0") + "°", string.Empty, ref y, ref fullRefreshPending, Color.White);
						if (_autopilotStatus != ' ' && _autopilotStatus != 'M')
						{
							ValueBlock(false, fullRefresh, "AP DES HDG", _autopilotDesiredHeading.Degrees.ToString("F0") + "°", string.Empty, ref y, ref fullRefreshPending, Color.White);
						}
					}

					hadData = true;

					if (!fullRefreshPending)
					{
						_display.UpdateScreen();
					}
				}
				else if (hadData)
				{
					_gfx.DrawTextEx("No data", _bigFont, 0, 0, Color.White);
					_gfx.DrawCircle(100, 100, 70, Color.White, true);
					_gfx.DrawRectangle(40, 90, 120, 20, Color.Black, true);
					hadData = false;

					_display.UpdateScreen();
				}
			}
		}

		private bool IsAutopilotActive()
		{
			return _autopilotStatus is 'S' or 'T' or 'W';
		}

		private string AutopilotStatusString()
		{
			string status = _autopilotStatus switch
			{
				'S' => "Auto",
				'T' => "Track",
				'M' => "Standby",
				'W' => "Wind",
				_ => "Offline",
			};
			return status;
		}

		private void ValueBlock(bool singleLine, bool fullRefresh, string label, string value, string unit, ref int y, ref bool cancel, Color color)
		{
			if (cancel)
			{
				return;
			}
			Size labelSize = _gfx.MeasureString(label, _mediumFont, 2, 8);
			Size valueSize = _gfx.MeasureString(value, _bigFont, 2, 4);
			Size unitSize = _gfx.MeasureString(unit, _mediumFont, 0, 0);
			int maxHeight = Math.Max(labelSize.Height, valueSize.Height);
			int offset = maxHeight - unitSize.Height;

			if (singleLine)
			{
				// Everything should fit on one line
				if (fullRefresh)
				{
					_gfx.DrawTextEx(label, _mediumFont, 0, y, color);
					_gfx.DrawTextEx(value, _bigFont, labelSize.Width, y, color);
					if (cancel)
					{
						return;
					}
					_gfx.DrawTextEx(unit, _mediumFont, labelSize.Width + valueSize.Width, y + offset, color);
				}
				else
				{
					_display.FastFillRectangle(labelSize.Width, y, _display.Width, y + valueSize.Height, 0);
					_gfx.DrawTextEx(value, _bigFont, labelSize.Width, y, color);
					if (cancel)
					{
						return;
					}
					_gfx.DrawTextEx(unit, _mediumFont, labelSize.Width + valueSize.Width, y + offset, color);
				}

				y += maxHeight;
			}
			else
			{
				if (fullRefresh)
				{
					_gfx.DrawTextEx(label, _mediumFont, 0, y, color);
					y += labelSize.Height;
					_gfx.DrawTextEx(value, _bigFont, 0, y, color);
					if (cancel)
					{
						return;
					}
					_gfx.DrawTextEx(unit, _mediumFont, valueSize.Width, y + offset, color);
					y += valueSize.Height;
				}
				else
				{
					y += labelSize.Height;
					_display.FastFillRectangle(0, y, _display.Width, _bigFont.Height + y, 0);
					_gfx.DrawTextEx(value, _bigFont, 0, y, color);
					if (cancel)
					{
						return;
					}
					_gfx.DrawTextEx(unit, _mediumFont, valueSize.Width, y + offset, color);
					y += valueSize.Height;
				}
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
