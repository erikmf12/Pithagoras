using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pithagoras.Models;
using Pithagoras.Workers;
using Pithagoras.Services;

namespace Pithagoras
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureServices((hostContext, services) =>
				{
					services.Configure<AppConfig>(hostContext.Configuration);

					services.AddMemoryCache();

					services.AddSingleton<IClientCache, ClientCache>();

					services.AddHostedService<TcpListenerWorker>();
					services.AddHostedService<HostConnectorWorker>();
				});
	}
}
