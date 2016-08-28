using System;

namespace Stubbornium.Sample.Utils
{
    public static class UriHelper
    {
        public static Uri Uri(this string stringUri) => new Uri(stringUri);

        public static Uri Combine(this Uri baseUri, string relativeUri) => new Uri(baseUri, relativeUri);

        public static Uri Combine(this Uri baseUri, Uri relativeUri) => new Uri(baseUri, relativeUri);
    }
}