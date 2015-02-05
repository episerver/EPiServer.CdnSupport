# EPiServer.CdnSupport

Rewrites media URLs in EPiServer CMS to an external CDN server. Urls are made unique and cacheable for 1 year.

Configuration;

```xml
  <appSettings>
    <add key="CdnUrl" value="http://cdn.site.com/"/>
  </appSettings>
```

 Supports CMS 8.
