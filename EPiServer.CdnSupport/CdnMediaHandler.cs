using EPiServer.Core;
using EPiServer.Framework.Blobs;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Web;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System;
using System.Configuration;
using System.Web;

namespace EPiServer.CdnSupport
{
    [TemplateDescriptor(Inherited = true, TemplateTypeCategory = TemplateTypeCategories.HttpHandler, Default = true)]
    public class CdnMediaHandler : BlobHttpHandler, IRenderTemplate<IContentMedia>
    {
        private static Lazy<TimeSpan> _cdnExpiration = new Lazy<TimeSpan>(() => TimeSpan.Parse(ConfigurationManager.AppSettings["episerver:CdnExpirationTime"] as string ?? "365.00:00:00"));

        protected override Blob GetBlob(HttpContextBase httpContext)
        {
            var filename = httpContext.Request.RequestContext.GetCustomRouteData<string>("Download");
            if (!string.IsNullOrEmpty(filename))
            {
                httpContext.Response.AppendHeader("Content-Disposition",
                    string.Format("attachment; filename=\"{0}\"; filename*=UTF-8''{1}", filename, Uri.EscapeDataString(filename)));
            }

            var routeHelper = ServiceLocator.Current.GetInstance<IContentRouteHelper>();
            var binaryStorable = routeHelper.Content as IBinaryStorable;

            return binaryStorable != null ? binaryStorable.BinaryData : null;
        }

        protected override void SetCachePolicy(System.Web.HttpContextBase context, DateTime fileChangedDate)
        {
            if (!IsRequestComingFromCdn(context))
            {
                base.SetCachePolicy(context, fileChangedDate);
                return;
            }

            var helper = ServiceLocator.Current.GetInstance<IContentRouteHelper>();
            if(helper.Content !=null)
            {
                var cdnString = (string)context.Items[CdnModule.CdnRequest];
                var expectedString = CdnModule.Unique(helper.Content);
                if(!String.Equals(cdnString, expectedString, StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.RedirectPermanent("/" + expectedString + context.Request.Url.PathAndQuery);
                }
            }

            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetLastModified(fileChangedDate);

            context.Response.Cache.SetExpires(DateTime.UtcNow + _cdnExpiration.Value);
            context.Response.Cache.SetValidUntilExpires(true);
            context.Response.Cache.VaryByParams.IgnoreParams = true;
            context.Response.Cache.SetOmitVaryStar(true);
            context.Response.Cache.SetNoServerCaching();
        }

        public static bool IsRequestComingFromCdn(HttpContextBase ctx)
        {
            return !ctx.Request.IsAuthenticated && ctx.Items[CdnModule.CdnRequest] != null;
        }
    }
}
