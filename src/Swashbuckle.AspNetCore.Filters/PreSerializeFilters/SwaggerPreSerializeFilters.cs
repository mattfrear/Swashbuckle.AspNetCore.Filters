using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;

namespace Swashbuckle.AspNetCore.Filters.PreSerializeFilters
{
    public static class SwaggerPreSerializeFilters
    {
        /// <summary>
        /// Filters swagger document before serialization to JSON
        /// </summary>
        /// <param name="doc">Swagger document object</param>
        /// <param name="req">Current http request</param>
        /// <param name="queryParamName">Name of query param from which to take collection of tag value</param>
        /// <param name="separator"></param>
        /// <remarks>
        /// Usage:
        /// app.UseSwagger(x => {
        ///     ...;
        ///     x.PreSerializeFilters.Add((doc, req) => SwaggerPreSerializeFilters.OperationTagFilterByQueryParam(doc, req, "tags", ","));
        /// }
        /// </remarks>
        public static void OperationTagFilterByQueryParam(SwaggerDocument doc, HttpRequest req, string queryParamName, char separator)
        {
            if (string.IsNullOrEmpty(queryParamName))
            {
                return;
            }

            var tags = GetQueryParamsByKey(req, queryParamName, separator);
            if (tags.Length == 0)
            {
                return;
            }

            // get paths with tags
            var filteredPaths = doc.Paths
                .Where(path =>
                {
                    var operations = new Operation[] { path.Value.Options, path.Value.Patch, path.Value.Get, path.Value.Post, path.Value.Put, path.Value.Delete };
                    foreach (var operation in operations)
                    {
                        if (operation.TryFindTags(tags))
                        {
                            return true;
                        }
                    }
                    return false;
                })
                .ToDictionary(f => f.Key, f => f.Value);

            doc.Paths = filteredPaths;
            if (filteredPaths.Count == 0)
            {
                doc.Definitions = null;
            }
        }

        private static string[] GetQueryParamsByKey(HttpRequest req, string key, char separator)
        {
            return req.Query[key].ToString().Split(separator);
        }

        private static bool TryFindTags(this Operation operation, IEnumerable<string> tags)
        {
            if (operation == null)
            {
                return false;
            }

            return operation.Tags.Any(tag => tags.Contains(tag, StringComparer.OrdinalIgnoreCase));
        }
    }
}
