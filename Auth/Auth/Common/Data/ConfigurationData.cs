namespace Auth.Common.Data
{
    public struct ConfigurationData
    {
        public string DiscoveryUri { get; set; }
        public string[] AuthPrefixes { get; set; }
        public RouteData[] Routes { get; set; }
        
        public int MaxCount { get; set; }
        
        public string ConnectionDb { get; set; }
    }
}