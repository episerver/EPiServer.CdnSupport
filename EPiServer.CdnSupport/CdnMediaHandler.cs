using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Web;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System;
using System.Web;

namespace EPiServer.CdnSupport
{
    [TemplateDescriptor(Inherited = true, TemplateTypeCategory = TemplateTypeCategories.HttpHandler, Default = true)]
    public class CdnMediaHandler : ContentMediaHttpHandler, IRenderTemplate<IContentMedia>
    {
        protected override void SetCachePolicy(System.Web.HttpContextBase context, DateTime fileChangedDate)
        {
            if (!IsRequestComingFromCdn(context))
            {
                base.SetCachePolicy(context, fileChangedDate);
                return;
            }

            var helper = ServiceLocator.Current.GetInstance<ContentRouteHelper>();
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

            context.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(1));
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
