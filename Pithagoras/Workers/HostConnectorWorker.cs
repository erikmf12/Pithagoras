using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pithagoras.Models;
using Pithagoras.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pithagoras.Workers
{
	public class HostConnectorWorker : BackgroundService
	{
		private readonly IClientCache _clientCache;
		private readonly IOptionsMonitor<AppConfig> _config;
		private readonly ILogger<HostConnectorWorker> _logger;

		public HostConnectorWorker(IClientCache clientCache,
			IOptionsMonitor<AppConfig> config,
			ILogger<HostConnectorWorker> logger)
		{
			_clientCache = clientCache;
			_config = config;
			_logger = logger;
		}


		protected override async Task ExecuteAsync(CancellationToken st)
		{
			while (!st.IsCancellationRequested)
			{
				var hosts = _config.CurrentValue.Hosts;
				if (hosts != null && hosts.Length > 0)
				{
					foreach (var client in _config.CurrentValue.Hosts)
					{
						if(_clientCache.Clients.Any(x => x.IPAddress == client.IPAddress))
						{
							continue;
						}
						if (TryConnectingToHost(client))
						{
							_logger.LogInformation($"Connected to {client.Name}");
						}
					}
				}
				await Task.Delay(2000);
			}
		}

		private bool TryConnectingToHost(PiHost host)
		{
			PiClient piClient = new PiClient();
			if( piClient.ConnectToServer(host.Name, host.IPAddress, host.Port))
			{
				_clientCache.AddClient(piClient);
				return true;
			}
			return false;
		}
	}
}
