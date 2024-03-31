using Iot.Device.Nmea0183;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using Iot.Device.Nmea0183.Sentences;

namespace AutoPilotControl
{
	public delegate void MessageReceivedDelegate(NmeaSentence sentence);

	public class NmeaParser : IDisposable
	{
		private CancellationTokenSource m_tokenSource;
		private Thread m_thread;
		private int m_udpPort;

		public NmeaParser(int udpPort)
		{
			m_udpPort = udpPort;
			IsReceivingData = false;
		}

		public event MessageReceivedDelegate NewMessage;

		public bool IsReceivingData
		{
			get;
			private set;
		}

		public virtual void StartDecode()
		{
			m_tokenSource = new CancellationTokenSource();
			if (m_thread != null)
			{
				return;
			}

			m_thread = new Thread(HandleUdpNmeaStream);
			m_thread.Start();
		}

		public virtual void StopDecode()
		{
			m_tokenSource.Cancel();
			m_thread?.Join();
			m_thread = null;
		}

		protected virtual void Dispose(bool disposing)
		{
			StopDecode();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void HandleUdpNmeaStream()
		{
			using var client = new UdpClient(m_udpPort);
			IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
			client.Client.ReceiveTimeout = 5000;
			client.Client.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.Broadcast, true);
			client.Connect(IPAddress.Any, m_udpPort);
			byte[] buffer = new byte[1024];
			DateTime lastTime = DateTime.UtcNow;
			while (!m_tokenSource.IsCancellationRequested)
			{
				try
				{
					int length = client.Receive(buffer, ref remote);
					if (length > 2 && length < buffer.Length)
					{
						string data = null;
						try
						{
							data = Encoding.UTF8.GetString(buffer, 0, length);
						}
						catch (Exception)
						{
							Debug.WriteLine("Invalid characters detected");
							continue;
						}

						NmeaError errorCode;
						TalkerSentence ts = TalkerSentence.FromSentenceString(data, out errorCode);
						if (errorCode != NmeaError.None)
						{
							Debug.WriteLine($"Nmea parser error: {errorCode}");
						}
						else
						{
							IsReceivingData = true;
							var sentence = ts.TryGetTypedValue(ref lastTime);
							NewMessage?.Invoke(sentence);
						}
					}
				}
				catch (SocketException ex) when (ex.ErrorCode == (int)SocketError.TimedOut)
				{
					// Ignore
					IsReceivingData = false;
				}
			}
		}
	}
}
