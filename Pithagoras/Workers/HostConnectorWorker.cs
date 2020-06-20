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
		private List<PiHost> _errorHosts = new List<PiHost>();
		private CancellationToken _token;

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
			_token = st;
			while (!st.IsCancellationRequested)
			{
				var hosts = _config.CurrentValue.Hosts;
				if (hosts != null && hosts.Length > 0)
				{
					foreach (var host in _config.CurrentValue.Hosts)
					{
						if (_clientCache.Clients.Any(x => x.IPAddress == host.IPAddress))
						{
							continue;
						}
						else if (_errorHosts.Contains(host))
						{
							continue;
						}

						PiClient piClient = new PiClient();
						if (await piClient.ConnectToServerAsync(host.Name, host.IPAddress, host.Port, _token))
						{
							_logger.LogInformation($"Connected to {host.Name}");
							_clientCache.AddClient(piClient);
						}
						else
						{
							_errorHosts.Add(host);
						}
					}
				}
				await Task.Delay(5000, st);
			}
		}
	}
}
