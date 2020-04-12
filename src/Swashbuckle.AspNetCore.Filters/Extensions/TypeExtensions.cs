using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Swashbuckle.AspNetCore.Filters.Test,PublicKey=" +
    "00240000048000009400000006020000002400005253413100040000010001000d2acb3789e62b" +
"52eccb8a2050a611e9290675dd2f48f2b21167c94a0cba514cfd3e0bfcb297d8f806d3c3972683" +
"aae72598e9182e50e7b058763b904e5dfd3b98d0b730b618d594969fcf49044ffa46ae81f350a7" +
"b72933a64e4e4f912ff7bad06cffbd28fc33d5b3768ce554acd1addb1e5a5118559f4d20b9ceec" +
"3bfc55b7")]
namespace Swashbuckle.AspNetCore.Filters.Extensions
{
    internal static class TypeExtensions
    {
        public static string SchemaDefinitionName(this Type type)
        {
            string name = null;

            if (!type.GetTypeInfo().IsGenericType)
            {
                name = type.Name; // this doesn't work for generic types
            }
            else
            {
                var nullableUnderlyingType = Nullable.GetUnderlyingType(type);
                if (nullableUnderlyingType != null)
                {
                    return nullableUnderlyingType.Name;
                }

                // remove `# from the generic type name
                var friendlyName = type.Name.Remove(type.Name.IndexOf('`'));
                // for generic, Schema will be TypeName[GenericTypeName]
                var genericArguments = type.GetGenericArguments();
                name = $"{string.Concat(genericArguments.Select(a => a.Name).ToList())}{friendlyName}";
            }

            return name;
        }
    }
}
