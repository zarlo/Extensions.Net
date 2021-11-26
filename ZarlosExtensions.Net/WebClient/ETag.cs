using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace ZarlosExtensions.Net.WebClientEx
{
    public static partial class WebClientExtensions
    {

        public static string GetETag(this HttpResponseHeaders headers)
        {
            IEnumerable<string> values;
            if (!headers.TryGetValues("ETag", out values))
                return string.Empty;

            var eTag = values.FirstOrDefault();

            return !string.IsNullOrWhiteSpace(eTag)
                ? eTag.TrimStart('"').TrimEnd('"')
                : string.Empty;
        }

    }
}
