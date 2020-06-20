using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pithagoras.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pithagoras.Workers
{
	public class TcpListenerWorker : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly IOptionsMonitor<AppConfig> _config;
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<TcpListenerWorker> _logger;
		private int _port;
		private CancellationToken _appStopToken;
		private TcpListener _tcpListener;

		public TcpListenerWorker(IServiceProvider serviceProvider,
			IOptionsMonitor<AppConfig> config,
			ILoggerFactory loggerFactory)
		{
			_serviceProvider = serviceProvider;
			_config = config;
			_loggerFactory = loggerFactory;
			_logger = loggerFactory.CreateLogger<TcpListenerWorker>();
		}

		protected override async Task ExecuteAsync(CancellationToken st)
		{
			_appStopToken = st;
			st.Register(StopListening);

			var args = Environment.GetCommandLineArgs();
			if (args.Length < 2 || !int.TryParse(args[1], out _port))
			{
				if (_config.CurrentValue.Port == 0)
				{
					throw new Exception("Port not configured");
				}
				_port = _config.CurrentValue.Port;
			}

			InitListener();

			_logger.LogInformation($"Listening for clients on port {_port}");
			while (!st.IsCancellationRequested)
			{
				try
				{
					var client = await _tcpListener.AcceptTcpClientAsync();
					AcceptClient(client);
					await Task.Delay(100);
				}
				catch { }
			}
		}

		private void StopListening()
		{
			_tcpListener.Stop();
		}



		private void AcceptClient(TcpClient client)
		{
			var piClient = new PiClient(client, _loggerFactory);
			piClient.StartListening(_appStopToken);
		}

		private void InitListener()
		{
			if (_tcpListener != null)
			{
				_tcpListener.Server?.Close();
				_tcpListener.Stop();
			}
			_tcpListener = new TcpListener(IPAddress.Any, _port);
			_tcpListener.Start();
		}
	}
}
