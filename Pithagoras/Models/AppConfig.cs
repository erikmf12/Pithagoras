using System;
using System.Collections.Generic;
using System.Text;

namespace Pithagoras.Models
{
	public class AppConfig
	{
		public int Port { get; set; }
		public PiHost[] Hosts { get; set; }
	}

	public class PiHost
	{
		public string Name { get; set; }
		public string IPAddress { get; set; }
		public int Port { get; set; }
	}
}
