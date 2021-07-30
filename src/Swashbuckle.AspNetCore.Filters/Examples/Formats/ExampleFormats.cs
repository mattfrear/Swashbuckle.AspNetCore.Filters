using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Net.Http.Headers;

namespace Swashbuckle.AspNetCore.Filters.Examples
{
    internal static class ExampleFormats
    {
        public static readonly ExampleFormat Xml = new XmlExampleFormat();
        public static readonly ExampleFormat Json = new JsonExampleFormat();
        public static readonly ExampleFormat Yaml = new YamlExampleFormat();

        public static IEnumerable<ExampleFormat> All()
        {
            yield return Xml;
            yield return Json;
            yield return Yaml;
        }

        public static ExampleFormat GetFormat(string mime)
        {
            return All().FirstOrDefault(x => new MediaTypeHeaderValue(mime).IsSubsetOf(x.MimeType));
        }
    }
}
