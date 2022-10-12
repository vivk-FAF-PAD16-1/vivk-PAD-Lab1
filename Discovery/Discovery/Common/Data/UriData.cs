namespace Discovery.Common.Data
{
	public struct UriData
	{
		public string Uri { get; set; }
		public string Endpoint { get; set; }

		public UriData(string uri, string endpoint)
		{
			Uri = uri;
			Endpoint = endpoint;
		}
	}
}