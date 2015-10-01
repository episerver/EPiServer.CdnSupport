# EPiServer.CdnSupport

Rewrites media URLs in EPiServer CMS to an external CDN server. Urls are made unique and cacheable for 1 year. No configuration required.

Example: Media URL /globalassets/alloy-track/alloytrack.png will be rewritten to "/globalassets/alloy-track/alloytrack.png" to "/4a768e/globalassets/alloy-track/alloytrack.png" where
the injected key is short form of the saved/published date ensuring the URL changes when the media updates.

OPTIONAL: If URL's to media should be rewritten to point to an external URL use this configuration, this configuration is not compatible with site specific assets in a multi site setup:

```xml
  <appSettings>
    <add key="episerver:ExternalMediaUrl" value="http://static.site.com/"/>
  </appSettings>
```

 Supports CMS 9.
