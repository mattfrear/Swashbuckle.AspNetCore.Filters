using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Newtonsoft;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes
{
    public class FakeNewtonsoftSchemaGenerator : NewtonsoftSchemaGenerator
    {
        public FakeNewtonsoftSchemaGenerator() : base(new SchemaGeneratorOptions(), new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        })
        { }
    }
}
