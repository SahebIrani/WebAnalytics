using System;

using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Demo.IntegratingGoogleAnalytics
{
    public class GoogleAnalyticsTagHelperComponent : TagHelperComponent
    {
        public GoogleAnalyticsTagHelperComponent(IOptionsSnapshot<GoogleAnalyticsOptions> googleAnalyticsOptions)
            => _googleAnalyticsOptions = googleAnalyticsOptions.Value;

        private readonly GoogleAnalyticsOptions _googleAnalyticsOptions;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Inject the code only in the head element
            if (string.Equals(output.TagName, "head", StringComparison.OrdinalIgnoreCase))
            {
                // Get the tracking code from the configuration
                string trackingCode = _googleAnalyticsOptions.TrackingCode;
                if (!string.IsNullOrEmpty(trackingCode))
                {
                    // PostContent correspond to the text just before closing tag
                    output.PostContent
                        .AppendHtml("<script async src='https://www.googletagmanager.com/gtag/js?id=")
                        .AppendHtml(trackingCode)
                        .AppendHtml("'></script><script>window.dataLayer=window.dataLayer||[];function gtag(){dataLayer.push(arguments)}gtag('js',new Date);gtag('config','")
                        .AppendHtml(trackingCode)
                        .AppendHtml("',{displayFeaturesTask:'null'});</script>");
                }
            }
        }
    }
