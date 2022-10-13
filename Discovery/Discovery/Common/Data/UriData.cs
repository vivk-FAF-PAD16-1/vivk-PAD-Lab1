namespace Discovery.Common.Data
{
	public struct UriData
	{
		public string Uri { get; set; }
		public string Param { get; set; }
		public string Endpoint { get; set; }

		public UriData(string uri, string param, string endpoint)
		{
			Uri = uri;
			Param = param;
			Endpoint = endpoint;
		}
	}
}