using EPiServer.Core;
using EPiServer.Core.Routing;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EPiServer.CdnSupport
{
    internal class UrlRewriter : IRoutedContentEvaluator
    {
        private readonly IHttpContextAccessor _http;
        private readonly IContextModeResolver _context;
        private readonly IContentLoader _loader;
        private static string[] _mediaPaths = new string[] { "/" + RoutingConstants.ContentAssetSegment, "/" + RoutingConstants.SiteAssetSegment, "/" + RoutingConstants.GlobalAssetSegment };
        private static Regex _validateUrl = new Regex("^/[a-z0-9]{6}/.+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static IIdentity _anonymousIdentity = new GenericIdentity(String.Empty);
        private static IPrincipal _anonymousPrincipal = new GenericPrincipal(_anonymousIdentity, null);
        private CdnSupportOptions _options;

        public UrlRewriter(IHttpContextAccessor http, IContextModeResolver context, IContentLoader loader, IOptions<CdnSupportOptions> options)
        {
            _http = http;
            _context = context;
            _loader = loader;
            _options = options.Value;
        }

        public void GeneratedUrl(object sender, UrlGeneratorEventArgs e)
        {
            if (_context.CurrentMode.EditOrPreview() || !_mediaPaths.Any(p => e.Context.Url.Path.StartsWith(p)))
            {
                return;
            }
            
            if (_loader.TryGet<IContent>(e.Context.ContentLink, out var content) && ((ISecurable)content).GetSecurityDescriptor().HasAccess(_anonymousPrincipal, AccessLevel.Read))
            {
                if (_options.OverrideHost != null)
                {
                    e.Context.Url.Host = _options.OverrideHost;
                }
                e.Context.Url.Path = $"/{Unique(content)}{ e.Context.Url.Path}";
            }
        }

        public void ResolvingUrl(object sender, UrlResolverEventArgs e)
        {
            if (!_validateUrl.IsMatch(e.Context.Url.Path))
            {
                return;
            }
            string newPath = e.Context.Url.Path.Substring(7);
            if (_mediaPaths.Any(p => newPath.StartsWith(p)))
            {
                var builder = new UrlBuilder(e.Context.Url);
                builder.Path = newPath;
                e.Context.Url = new Url(builder.ToString());
            }
        }

        public static string Unique(IContent content)
        {
            var d = ((IChangeTrackable)content).Saved;
            int hash = 17;
            unchecked
            {
                hash = hash * 23 + d.Month;
                hash = hash * 23 + d.Day;
                hash = hash * 23 + d.Minute;
                hash = hash * 23 + d.Second;
            }
            return hash.ToString("x6");
        }

        public Task<RoutedContentEvaluationResult> EvaluateRoutedContentAsync(ContentRouteData routedContentData)
        {
            if (!(routedContentData.Content is MediaData) || !_validateUrl.IsMatch(_http.HttpContext.Request.Path))
            {
                return Task.FromResult(RoutedContentEvaluationResult.OK);
            }
            var expectedString = UrlRewriter.Unique(routedContentData.Content);
            if (!_http.HttpContext.Request.Path.StartsWithSegments("/" + expectedString))
            {
                return Task.FromResult(RoutedContentEvaluationResult.Redirect(HttpRedirect.Permanent, $"/{expectedString}{_http.HttpContext.Request.Path.Value.Substring(7)}{_http.HttpContext.Request.QueryString}"));
            }
            return Task.FromResult(RoutedContentEvaluationResult.OK);
        }
    }
}
