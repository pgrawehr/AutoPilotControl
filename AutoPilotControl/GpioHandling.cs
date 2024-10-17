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
		private GpioPin _backButton;

		private int _upButtonCounts;
		private int _downButtonCounts;
		private int _enterButtonCounts;
		private int _backButtonCounts;

		private Thread _buttonThread;
		private bool _terminate;

		private PwmChannel _speaker;

		public GpioHandling()
		{
			_controller = new GpioController();
			_buttonThread = new Thread(ButtonPollThread);
		}

		public void Init()
		{
			_up = _controller.OpenPin(37, PinMode.Input);
			_enter = _controller.OpenPin(38, PinMode.Input);
			_down = _controller.OpenPin(39, PinMode.Input);
			_backButton = _controller.OpenPin(5, PinMode.Input);

			_led = _controller.OpenPin(LED_PIN, PinMode.Output);
			// If the output is high, the led is off
			_led.Write(PinValue.High);

			// Just not working as expected: Will trigger on the wrong edge and repeatedly, even if the status clearly
			// does not change.
			//_controller.RegisterCallbackForPinValueChangedEvent(_topButton.PinNumber, PinEventTypes.Rising, OnBackButtonClicked);
			//_controller.RegisterCallbackForPinValueChangedEvent(_up.PinNumber, PinEventTypes.Rising, OnUpButtonClicked);
			//_controller.RegisterCallbackForPinValueChangedEvent(_down.PinNumber, PinEventTypes.Rising, OnDownButtonClicked);
			//_controller.RegisterCallbackForPinValueChangedEvent(_enter.PinNumber, PinEventTypes.Rising, OnEnterButtonClicked);

			_speaker = PwmChannel.CreateFromPin(2, 1000, 0.5);
			_speaker.Start(); // To make sure it goes off
			_speaker.Stop();

			_terminate = false;
			_buttonThread.Start();
		}

		private void ButtonPollThread()
		{
			PinValue topButtonState = _backButton.Read();
			PinValue enterButtonState = _enter.Read();
			PinValue upButtonState = _up.Read();
			PinValue downButtonState = _down.Read();
			while (!_terminate)
			{
				var newState = _backButton.Read();
				if (newState != topButtonState)
				{
					OnBackButtonClicked(this,
						newState == PinValue.High
							? new PinValueChangedEventArgs(PinEventTypes.Rising, _backButton.PinNumber) : 
							new PinValueChangedEventArgs(PinEventTypes.Falling, _backButton.PinNumber));
					topButtonState = newState;
				}

				newState = _enter.Read();
				if (newState != enterButtonState)
				{
					OnEnterButtonClicked(this,
						newState == PinValue.High
							? new PinValueChangedEventArgs(PinEventTypes.Rising, _enter.PinNumber) :
							new PinValueChangedEventArgs(PinEventTypes.Falling, _enter.PinNumber));
					enterButtonState = newState;
				}

				newState = _up.Read();
				if (newState != upButtonState)
				{
					OnUpButtonClicked(this,
						newState == PinValue.High
							? new PinValueChangedEventArgs(PinEventTypes.Rising, _up.PinNumber) :
							new PinValueChangedEventArgs(PinEventTypes.Falling, _up.PinNumber));
					upButtonState = newState;
				}

				newState = _down.Read();
				if (newState != downButtonState)
				{
					OnDownButtonClicked(this,
						newState == PinValue.High
							? new PinValueChangedEventArgs(PinEventTypes.Rising, _down.PinNumber) :
							new PinValueChangedEventArgs(PinEventTypes.Falling, _down.PinNumber));
					downButtonState = newState;
				}

				Thread.Sleep(50);
			}
		}

		public void ClearEventHandlers()
		{
		}

		public void WaitNoButtonsPressed()
		{
			while (_up.Read() == PinValue.Low || _down.Read() == PinValue.Low || _enter.Read() == PinValue.Low || _backButton.Read() == PinValue.Low)
			{
				Thread.Sleep(10);
			}
			lock (_lock)
			{
				_downButtonCounts = 0;
				_upButtonCounts = 0;
				_backButtonCounts = 0;
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
			_terminate = true;
			_buttonThread.Join();
			ClearEventHandlers();

			//_controller.UnregisterCallbackForPinValueChangedEvent(_topButton.PinNumber, OnBackButtonClicked);
			//_controller.UnregisterCallbackForPinValueChangedEvent(_up.PinNumber, OnUpButtonClicked);
			//_controller.UnregisterCallbackForPinValueChangedEvent(_down.PinNumber, OnDownButtonClicked);
			//_controller.UnregisterCallbackForPinValueChangedEvent(_enter.PinNumber, OnEnterButtonClicked);
		}

		private void OnBackButtonClicked(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
		{
			lock (_lock)
			{
				if (pinValueChangedEventArgs.ChangeType == PinEventTypes.Rising)
				{
					return;
				}
				if (_backButtonCounts == 0)
				{
					Beep();
					_backButtonCounts = 1;
				}
			}
		}

		private void OnUpButtonClicked(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
		{
			lock (_lock)
			{
				if (pinValueChangedEventArgs.ChangeType == PinEventTypes.Rising)
				{
					return;
				}
				Beep(850);
				_upButtonCounts++;
			}
		}

		public bool IsDownButtonPressed()
		{
			return _down.Read() == PinValue.Low;
		}

		public bool IsUpButtonPressed()
		{
			return _up.Read() == PinValue.Low;
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
				if (pinValueChangedEventArgs.ChangeType == PinEventTypes.Rising)
				{
					return;
				}
				Beep(850);
				_downButtonCounts++;
			}
		}

		private void OnEnterButtonClicked(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
		{
			lock (_lock)
			{
				if (pinValueChangedEventArgs.ChangeType == PinEventTypes.Rising)
				{
					return;
				}
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

		public bool BackButtonWasClicked()
		{
			lock (_lock)
			{
				if (_backButtonCounts > 0)
				{
					_backButtonCounts = 0;
					return true;
				}

				return false;
			}
		}
	}
}
