using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Filters.Extensions;

namespace Swashbuckle.AspNetCore.Filters.Examples
{
    internal static class ExampleFormats
    {
        public static readonly ExampleFormat Xml = new XmlExampleFormat();
        public static readonly ExampleFormat Json = new ExampleFormat("application/json; charset=utf-8");
        public static readonly ExampleFormat Yaml = new ExampleFormat("application/yaml; charset=utf-8");

        public static IEnumerable<ExampleFormat> All()
        {
            yield return Xml;
            yield return Json;
            yield return Yaml;
        }

        public static ExampleFormat GetFormat(string mime)
        {
            return All().First(x => new MediaTypeHeaderValue(mime).IsSubsetOf(x.MimeType));
        }
    }

    internal class ExampleFormat
    {
        public ExampleFormat(string mime)
        {
            MimeType = MediaTypeHeaderValue.Parse(mime);
        }

        public MediaTypeHeaderValue MimeType { get; private set; }

        public virtual string Format(string s)
        {
            // NoOp in base
            return s;
        }
    }

    internal class XmlExampleFormat : ExampleFormat
    {
        public XmlExampleFormat() : base("application/xml; charset=utf-8")
        {

        }

        public override string Format(string s)
        {
            return s.FormatXml();
        }
    }


}
