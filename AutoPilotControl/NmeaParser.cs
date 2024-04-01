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
		private UdpClient m_client;

		public NmeaParser(int udpPort)
		{
			m_udpPort = udpPort;
			IsReceivingData = false;

			m_client = new UdpClient(m_udpPort);
			m_client.Client.ReceiveTimeout = 5000;
			m_client.Client.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.Broadcast, true);
			// m_client.Connect(IPAddress.Parse("192.168.1.255"), m_udpPort);
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
			m_client?.Dispose();
			m_client = null;
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

		public void SendMessage(NmeaSentence sentence)
		{
			string msg = sentence.ToNmeaMessage() + "\r\n";
			byte[] data = Encoding.UTF8.GetBytes(msg);
			m_client.Send(data, new IPEndPoint(IPAddress.Broadcast, m_udpPort));
		}

		private void HandleUdpNmeaStream()
		{
			byte[] buffer = new byte[1024];
			DateTime lastTime = DateTime.UtcNow;
			while (!m_tokenSource.IsCancellationRequested)
			{
				try
				{
					var remote = new IPEndPoint(IPAddress.Any, m_udpPort);
					int length = m_client.Receive(buffer, ref remote);
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
