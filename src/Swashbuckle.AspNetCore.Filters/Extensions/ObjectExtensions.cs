using System.Xml.Linq;

namespace Swashbuckle.AspNetCore.Filters.Extensions
{
    internal static class ObjectExtensions
    {
        public static string FormatXml(this string unformattedXml)
        {
            if (string.IsNullOrEmpty(unformattedXml))
            {
                return string.Empty;
            }

            return XDocument.Parse(unformattedXml).ToString();
        }
    }
}
