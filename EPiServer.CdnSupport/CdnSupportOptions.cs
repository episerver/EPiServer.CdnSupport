using System;

namespace EPiServer.CdnSupport
{
    /// <summary>
    /// Options for CDN module
    /// </summary>
    public class CdnSupportOptions
    {
        /// <summary>
        /// Override host to CDN, ie mystaticfiles.example.com
        /// </summary>
        public string OverrideHost { get; set; }

        /// <summary>
        /// Override max-age header for URL coming from CDN
        /// </summary>
        public TimeSpan? MaxAge { get; set; } = TimeSpan.FromDays(365);
    }
}