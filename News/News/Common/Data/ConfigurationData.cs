namespace News.Common.Data
{
    public struct ConfigurationData
    {
        public string DiscoveryUri { get; set; }
        public string CacheUri { get; set; }
        public string[] NewsPrefixes { get; set; }
        public RouteData[] Routes { get; set; }
        
        public int MaxCount { get; set; }
        
        public string ServerDb { get; set; }
        public int PortDb { get; set; }
        public string UserDb { get; set; }
        public string PassDb { get; set; }
        
        public bool Independent { get; set; }
    }
}