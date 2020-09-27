using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes
{
    public class FakeApiDescriptionGroupCollectionProvider : IApiDescriptionGroupCollectionProvider
    {
        private readonly IEnumerable<ApiDescription> _apiDescriptions;

        public FakeApiDescriptionGroupCollectionProvider(IEnumerable<ApiDescription> apiDescriptions)
        {
            _apiDescriptions = apiDescriptions;
        }

        public ApiDescriptionGroupCollection ApiDescriptionGroups
        {
            get
            {
                var apiDescriptionGroups = _apiDescriptions
                    .GroupBy(item => item.GroupName)
                    .Select(grouping => new ApiDescriptionGroup(grouping.Key, grouping.ToList()))
                    .ToList();

                return new ApiDescriptionGroupCollection(apiDescriptionGroups, 1);
            }
        }
    }
}