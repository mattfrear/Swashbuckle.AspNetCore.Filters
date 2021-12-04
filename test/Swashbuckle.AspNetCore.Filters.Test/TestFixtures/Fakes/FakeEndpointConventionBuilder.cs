using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes
{
    public class FakeEndpointConventionBuilder : IEndpointConventionBuilder
    {
        internal EndpointBuilder EndpointBuilder { get; }

        private readonly List<Action<EndpointBuilder>> conventions;

        public FakeEndpointConventionBuilder(EndpointBuilder endpointBuilder)
        {
            EndpointBuilder = endpointBuilder;
            conventions = new List<Action<EndpointBuilder>>();
        }

        public void Add(Action<EndpointBuilder> convention)
        {
            conventions.Add(convention);
        }

        public Endpoint Build()
        {
            foreach (var convention in conventions)
            {
                convention(EndpointBuilder);
            }

            return EndpointBuilder.Build();
        }
    }
}
