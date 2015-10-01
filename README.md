# EPiServer.CdnSupport

Rewrites media URLs in EPiServer CMS. URLs are made unique and cacheable for 1 year. No configuration required.

Example: Media URL /globalassets/alloy-track/alloytrack.png will be rewritten to "/4a768e/globalassets/alloy-track/alloytrack.png" where
the injected key is short form of the saved/published date ensuring the URL changes when the media changes. Requests where the injected
key is outdated will be permanently redirected to an updated key for SEO reasons and to make sure bookmarks continue to work.

OPTIONAL: If URLs to media should be rewritten to point to an external CDN use this configuration, do not use in
combination with site specific assets in a multi site setup since multiple CDN hosts are required.

```xml
  <appSettings>
    <add key="episerver:ExternalMediaUrl" value="http://static.site.com/"/>
  </appSettings>
```

 Supports CMS 9.
