﻿namespace News.Common.Data
{
    public struct ConfigurationData
    {
        public string DiscoveryUri { get; set; }
        public string[] NewsPrefixes { get; set; }
        public RouteData[] Routes { get; set; }
        
        public int MaxCount { get; set; }
    }
}