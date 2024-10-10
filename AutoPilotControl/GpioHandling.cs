using System;
using System.Device.Gpio;
using System.Device.Pwm;
using System.Threading;

namespace AutoPilotControl
{
	internal sealed class GpioHandling : IDisposable
	{
		private const int LED_PIN = 10;

		private GpioController _controller;
		private object _lock = new object();

		private GpioPin _led;
		private GpioPin _up;
		private GpioPin _down;
		private GpioPin _enter;
		private GpioPin _topButton;

		private int _upButtonCounts;
		private int _downButtonCounts;
		private int _enterButtonCounts;
		private int _topButtonCounts;

		private PwmChannel _speaker;

		public GpioHandling()
		{
			_controller = new GpioController();
		}

		public void Init()
		{
			_up = _controller.OpenPin(37, PinMode.Input);
			_enter = _controller.OpenPin(38, PinMode.Input);
			_down = _controller.OpenPin(39, PinMode.Input);
			_topButton = _controller.OpenPin(5, PinMode.Input);

			_led = _controller.OpenPin(LED_PIN, PinMode.Output);
			// If the output is high, the led is off
			_led.Write(PinValue.High);

			_controller.RegisterCallbackForPinValueChangedEvent(_topButton.PinNumber, PinEventTypes.Falling, OnBackButtonClicked);
			_controller.RegisterCallbackForPinValueChangedEvent(_up.PinNumber, PinEventTypes.Falling, OnUpButtonClicked);
			_controller.RegisterCallbackForPinValueChangedEvent(_down.PinNumber, PinEventTypes.Falling, OnDownButtonClicked);
			_controller.RegisterCallbackForPinValueChangedEvent(_enter.PinNumber, PinEventTypes.Falling, OnEnterButtonClicked);

			_speaker = PwmChannel.CreateFromPin(2, 1000, 0.5);
			_speaker.Start(); // To make sure it goes off
			_speaker.Stop();
		}

		public void ClearEventHandlers()
		{
		}

		public void WaitNoButtonsPressed()
		{
			while (_up.Read() == PinValue.Low || _down.Read() == PinValue.Low || _enter.Read() == PinValue.Low || _topButton.Read() == PinValue.Low)
			{
				Thread.Sleep(10);
			}
			lock (_lock)
			{
				_downButtonCounts = 0;
				_upButtonCounts = 0;
				_topButtonCounts = 0;
				_enterButtonCounts = 0;
			}
		}

		public void Beep(int frequency = 1000)
		{
			lock (_speaker)
			{
				_speaker.Frequency = frequency;
				_speaker.Start();
				Thread.Sleep(50);
				_speaker.Stop();
			}
		}

		public void BlinkLed(int delayMs, int count)
		{
			for (int i = 0; i < count; i++)
			{
				_led.Toggle();
				Thread.Sleep(delayMs);
			}

			_led.Write(PinValue.High);
		}

		public void Dispose()
		{
			ClearEventHandlers();

			_controller.UnregisterCallbackForPinValueChangedEvent(_topButton.PinNumber, OnBackButtonClicked);
			_controller.UnregisterCallbackForPinValueChangedEvent(_up.PinNumber, OnUpButtonClicked);
			_controller.UnregisterCallbackForPinValueChangedEvent(_down.PinNumber, OnDownButtonClicked);
			_controller.UnregisterCallbackForPinValueChangedEvent(_enter.PinNumber, OnEnterButtonClicked);
		}

		private void OnBackButtonClicked(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
		{
			lock (_lock)
			{
				if (_topButtonCounts == 0)
				{
					Beep();
					_topButtonCounts = 1;
				}
			}
		}

		private void OnUpButtonClicked(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
		{
			lock (_lock)
			{
				Beep(850);
				_upButtonCounts++;
			}
		}

		public bool UpButtonWasClicked(bool andReset)
		{
			lock (_lock)
			{
				if (_upButtonCounts > 0)
				{
					_upButtonCounts--;
					if (andReset)
					{
						_upButtonCounts = 0;
					}
					return true;
				}

				return false;
			}
		}

		public bool DownButtonWasClicked(bool andReset)
		{
			lock (_lock)
			{
				if (_downButtonCounts > 0)
				{
					_downButtonCounts--;
					if (andReset)
					{
						_downButtonCounts = 0;
					}
					return true;
				}

				return false;
			}
		}

		private void OnDownButtonClicked(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
		{
			lock (_lock)
			{
				Beep(850);
				_downButtonCounts++;
			}
		}

		private void OnEnterButtonClicked(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
		{
			lock (_lock)
			{
				if (_enterButtonCounts == 0)
				{
					Beep();
					_enterButtonCounts = 1;
				}
			}
		}

		public bool EnterButtonWasClicked()
		{
			lock (_lock)
			{
				if (_enterButtonCounts > 0)
				{
					_enterButtonCounts = 0;
					return true;
				}

				return false;
			}
		}

		public bool TopButtonWasClicked()
		{
			lock (_lock)
			{
				if (_topButtonCounts > 0)
				{
					_topButtonCounts = 0;
					return true;
				}

				return false;
			}
		}
	}
}
