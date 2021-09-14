## EPiServer.CdnSupport

Rewrites media URLs in EPiServer CMS. URLs are made unique and cacheable for 1 year.

Example: Media URL /globalassets/alloy-track/alloytrack.png will be rewritten to "/4a768e/globalassets/alloy-track/alloytrack.png" where
the injected key is short form of the saved/published date ensuring the URL changes when the media changes. Requests where the injected
key is outdated will be permanently redirected to an updated key for SEO reasons and to make sure bookmarks continue to work.

Supports CMS 12+. Previous version can be found on the [CMS11 branch](tree/CMS11).

## Getting started

```
 public void ConfigureServices(IServiceCollection services)
 {
  .....
  //Optional config: services.Configure<CdnSupportOptions>(o=>o.MaxAge = TimeSpan.FromDays(7));
  services.AddCdnSupport();
 }

 public void Configure(IApplicationBuilder app)
 {
  ....
   app.UseCdnSupport();
 }
```

## CdnSupportOptions

*MaxAge*: The default expiration of 1 year is designed to optimize the length media stays in browser caches and that does not mean
the content have to stay in a CDN for a year, see CDN documentation on which rules apply.

*OverrideHost*:  If URLs to media should be rewritten to point to an external CDN use this configuration, do not use in
combination with site specific assets in a multi site setup since multiple CDN hosts would have been required.