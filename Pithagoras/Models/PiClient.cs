using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pithagoras.Models
{
	[Serializable]
	public class PiClient
	{
		public string Name { get; set; }
		public string IPAddress { get; set; }
		public int RemotePort { get; set; }
		public TcpClient TcpClient { get; set; }
		private readonly ILogger<PiClient> _logger;

		[NonSerialized]
		public bool Connected;
		public PiClient() : this(null) { }

		public PiClient(TcpClient client, ILoggerFactory loggerFactory = null)
		{
			if (client != null)
			{
				TcpClient = client;
				var endPoint = client.Client.RemoteEndPoint as IPEndPoint;
				RemotePort = endPoint.Port;
				IPAddress = endPoint.Address.ToString();
			}

			if (loggerFactory == null)
			{
				loggerFactory = LoggerFactory.Create(x => x.AddConsole());
			}
			_logger = loggerFactory.CreateLogger<PiClient>();
		}

		public void StartListening(CancellationToken token)
		{
			Task.Run(async () =>
			{
				try
				{
					var stream = TcpClient.GetStream();
					_logger.LogInformation($"Client {Name} connected!");
					Connected = true;
					while (!token.IsCancellationRequested)
					{



						// todo: listen for messages
						// handle messages



						// repeat
						await Task.Delay(1000, token);
					}
				}
				catch (TaskCanceledException ex)
				{

				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Client Error");
				}
				finally
				{
					Connected = false;
				}
			});
		}

		public bool ConnectToServer(string name, string ipAddress, int port)
		{
			try
			{
				this.Name = name;
				this.RemotePort = port;
				this.IPAddress = ipAddress;
				TcpClient = new TcpClient(ipAddress, port);
				Connected = true;
				return true;
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Error connecting to client");
			}
			return false;
		}

	}
}
