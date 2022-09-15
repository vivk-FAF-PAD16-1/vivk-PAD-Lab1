﻿namespace Gateway.Common
{
    public struct RouteData
    {
        public string Endpoint { get; set; }
        public string DestinationUri { get; set; }

        public bool IsValid =>
            Endpoint != null &&
            DestinationUri != null;
    }
}