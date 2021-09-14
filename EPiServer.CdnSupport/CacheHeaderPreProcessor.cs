using EPiServer.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPiServer.CdnSupport
{
    internal class CacheHeaderPreProcessor : IStaticFilePreProcessor
    {
        private readonly IOptions<CdnSupportOptions> _options;

        public int Order => 1000000;

        public CacheHeaderPreProcessor(IOptions<CdnSupportOptions> options)
        {
            this._options = options;
        }

        public void PrepareResponse(StaticFileResponseContext staticFileResponseContext)
        {
            if (!staticFileResponseContext.Context.User?.Identity?.IsAuthenticated ?? false)
            {
                var headers = staticFileResponseContext.Context.Response.GetTypedHeaders();
                var cc = new Microsoft.Net.Http.Headers.CacheControlHeaderValue();
                cc.MaxAge = _options.Value.MaxAge;
                headers.CacheControl = cc;  
            }
            
        }
    }
}
