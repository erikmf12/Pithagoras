using Microsoft.Extensions.Caching.Memory;
using Pithagoras.Models;
using System;
using System.Collections.Generic;

namespace Pithagoras.Services
{
	public class ClientCache : IClientCache
	{
		private readonly IMemoryCache _memoryCache;
		private List<string> _allClientAddresses = new List<string>();

		public IEnumerable<PiClient> Clients
		{
			get
			{
				foreach (var addr in _allClientAddresses.ToArray())
				{
					if (_memoryCache.TryGetValue(addr, out var client))
					{
						yield return client as PiClient;
					}
					else
					{
						_allClientAddresses.Remove(addr);
					}
				}
			}
		}

		public ClientCache(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}



		public void AddClient(PiClient client)
		{
			if (client == null || string.IsNullOrEmpty(client.IPAddress))
				throw new ArgumentNullException("Client was null or had no IP Address");

			_allClientAddresses.Add(client.IPAddress);
			_memoryCache.Set(client.IPAddress, client);
		}

		public void RemoveClient(string clientAddress)
		{
			_memoryCache.Remove(clientAddress);
			_allClientAddresses.Remove(clientAddress);
		}

	}
}
