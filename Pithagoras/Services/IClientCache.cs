using Pithagoras.Models;
using System.Collections.Generic;

namespace Pithagoras.Services
{
	public interface IClientCache
	{
		IEnumerable<PiClient> Clients { get; }

		void AddClient(PiClient client);
		void RemoveClient(string clientAddress);
	}
}