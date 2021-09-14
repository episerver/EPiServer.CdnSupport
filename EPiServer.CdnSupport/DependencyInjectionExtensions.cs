using EPiServer.CdnSupport;
using EPiServer.Core.Routing;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static void AddCdnSupport(this IServiceCollection services) => services.AddCdnSupport(null);

        public static void AddCdnSupport(this IServiceCollection services, Action<CdnSupportOptions> options)
        {
            services.AddOptions<CdnSupportOptions>();
            if (options != null)
            {
                services.Configure<CdnSupportOptions>(options);
            }
            services.AddSingleton<CacheHeaderPreProcessor>();
            services.AddSingleton<UrlRewriter>().Forward<UrlRewriter, IRoutedContentEvaluator>();
            services.Configure<MediaOptions>(m => m.AddPreProcessor<CacheHeaderPreProcessor>());
        }
        
        public static void UseCdnSupport(this IApplicationBuilder app)
        {
            var rw = app.ApplicationServices.GetService<UrlRewriter>();
            app.ApplicationServices.GetService<IContentUrlGeneratorEvents>().GeneratedUrl += rw.GeneratedUrl;
            app.ApplicationServices.GetService<IContentUrlResolverEvents>().ResolvingUrl += rw.ResolvingUrl;
        }
    }
}
