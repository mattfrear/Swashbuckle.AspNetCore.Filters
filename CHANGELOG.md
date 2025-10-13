# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [9.0.1] - 2025-10-14
### Fixed
- Fix issue #261 (thanks @ntark)

## [9.0.0] - 2025-06-17
### Changed
- Issue #257 upgrade from Swashbuckle.AspNetCore.SwaggerGen 5.0.0 to 8.0.0 because Swashbuckle.AspNetCore 9.0.0 didn't work. No longer support .netcore3.1 or .net5.0, only support .netstandard20 and .net80.

### Fixed
- Issue #253

## [8.0.3] - 2025-05-03
### Changed
- Issue #254 - Made MvcOutputformatter public so that it can be overridden.

## [8.0.2] - 2024-05-01
### Fixed
- Issue #244 - SecurityRequirementsOperationFilter adds duplicate security requirement when using .WithOpenApi()

## [8.0.1] - 2024-02-16
### Added
- Issue #242. Add ReadMe to NuGet package.

## [8.0.0] - 2024-01-09
### Changed
- Issue #237 - breaking change. Default to camelCase instead of PascalCase for examples in minimal APIs.

## [7.0.12] - 2023-10-24
### Added
- PR #235 added support for text/csv in request examples and IMultipleExamplesProvider (thanks @Bl4d3s)

## [7.0.11] - 2023-09-07
### Changed
- Issue #230, inject IServiceProvider to MvcOutputFormatter instead of creating a new one

## [7.0.10] - 2023-09-07
### Fixed
- Fix bug in GetControllerAndActionAttributes, introduced by the last release which causes AddResponseHeadersFilter to fail.

## [7.0.9] - 2023-09-06
### Fixed
- Issue #227 - SecurityRequirementsOperationFilter for minimal APIs doesn't work with Swashbuckle.AspNetCore > 6.3.0 (thanks @hartmair)

## [7.0.8] - 2023-07-15
### Fixed
- Issue #231 - NuGet package now built in Release mode

## [7.0.7] - 2023-07-14
### Fixed
- Issue #225 - SecurityRequirementsOperationFilter doesn't add Auth to methods derived from a BaseController (thanks @andrew-yustyk)

## [7.0.6] - 2022-11-14
### Changed
- Allow SwaggerResponseHeaderAttribute to be used on classes.

## [7.0.5] - 2022-08-22
### Fixed
- Issue #209 - Request example clash when using API versioning

## [7.0.4] - 2022-08-01

### Added
- Issues #43 and #129 - added support for text/csv in response examples (thanks @icnocop)

## [7.0.3] - 2022-05-24
### Fixed
- Issue #189 - fix exception when using Minimal APIs (thanks @dotdiego)

### Added
- Issue #165 - add Description to multiple examples (thanks @penenkel)

### Removed
- Issue #162 - removed internal OpenApiRawString and use OpenApiString with isRawString parameter (thanks @WillGunn)

## [7.0.2] - 2021-04-03
### Fixed
- Fixed License

## [7.0.1] - 2021-04-03
### Added
- Add symbols NuGet package

## [7.0.0] - 2021-04-03
### Security
- PR #176 update NuGet packages

### Changed
- PR #178 dependency cleanup

### Added
- PR #179 introduce Swashbuckle.AspNetCore.Filters.Abstractions package so that IExamplesProvider can be consumed without Swashbuckle dependency.
- Target .NET 5.0
- Microsoft.SourceLink.GitHub NuGet reference

### Removed
- Issue #174 remove ExcludeObsoletePropertiesResolver and NewtonsoftJson dependency.

## [6.1.0] - 2021-02-15
### Added
- Issue #171 support auto example for `ProducesDefaultResponseType`.

## [6.0.1] - 2020-10-16
### Fixed
- PR #161 restore NewtonsoftJson dependency.

## [6.0.0] - 2020-09-27
### Fixed 
- Issue #132 Support `[System.Text.JsonPropertyNameAttribute]`. This is a breaking change which rewrote how the examples
 are generated. Instead of explicitly using Newtonsoft's `JsonConvert.SerializeObject()`, I now use whichever JSON
 serializer is registered with the MVC pipeline, i.e. during `services.AddControllers()`.

### Removed
- contractResolver and jsonConverter parameters from `[SwaggerRequestExampleAttribute]`.
- contractResolver and jsonConverter parameters from `[SwaggerResponseExampleAttribute]`.
- `Microsoft.AspNetCore.Mvc.NewtonsoftJson` dependency.

### Changed
- If you are using `options.IgnoreObsoleteProperties();` and you want your Examples to not have the 
  obsolete properties, then you will need to register my custom Newtonsoft `ExcludeObsoletePropertiesResolver`,
  e.g.
```csharp
services.AddControllers()
    .AddNewtonsoftJson(opt =>
    {
        opt.SerializerSettings.ContractResolver = new ExcludeObsoletePropertiesResolver(opt.SerializerSettings.ContractResolver);
```

## [5.1.2] - 2020-06-25
### Fixed
- #154 Upgrade to Microsoft.OpenApi 1.2.2 because 1.2.0 had breaking changes
### Added
- Add `services.AddSwaggerExamples()` extension method to allow examples without automatic annotation

## [5.1.1] - 2020-04-12
### Fixed
- #115 Added workaround for request examples when SerializeAsV2 = true
- #148 AddSwaggerExamplesFromAssemblies method does not scan for IMultipleExamplesProvider implementations

## [5.1.0] - 2020-04-05
### Added
- PR #147 add support for multiple request and response examples. Thanks to @tomkludy and @pozy for the contribution.

## [5.0.2] - 2020-02-25
### Added
- PR #140 add extension methods AddSwaggerExamplesFromAssemblyOf and AddSwaggerExamplesFromAssemblies

## [5.0.1] 2020-02-25
### Fixed
- Fix #136 use either XmlSerializer or DataContractSerializer to output XML examples, depending on what is configured. 
  Thanks to @CumpsD and @ridingwolf for the PR.

## [5.0.0] - 2020-01-21
### Changed
- Use Swashbuckle.AspNetCore 5.0.0

## [5.0.0-rc9] - 2019-12-31
### Fixed
- PR #110, where using IgnoreObsoleteProperties option causes PascalCase to be emitted instead of camelCase.
### Changed
- Use Swashbuckle.AspNetCore 5.0.0-rc5

## [5.0.0-rc8] - 2019-08-02
### Fixed
- Issue #106 SecurityRequirementsOperationFilter removes existing OpenApiSecurityRequirements
## Added 
- PR #104 add optional format parameter to the SwaggerResponseHeaderAttribute

## [5.0.0-rc7] 2019-07-26
### Fixed
- Issue #98 check schemaGeneratorOptions.IgnoreObsoleteProperties when generating json examples

## [5.0.0-rc6] - 2019-07-26
### Added
- PR #103 - Response Headers filter can now take an array of status codes

## [5.0.0-rc5] - 2019-07-24
### Fixed
- Issue #101, Exception with SecurityRequirementsOperationFilter when you have already added a 401 or a 403

## [5.0.0-rc4] - 2019-07-13
### Fixed
- Issue #99, JSON examples were encoded JSON

## [5.0.0-rc3] - 2019-06-10
### Removed
- Remove IExamplesProvider interface. Only support IExamplesProvider<T>

## [5.0.0-rc2] - 2019-05-30
### Changed
- Use Swashbuckle.AspNetCore 5.0.0-rc2
- Support .NET Core 3.0 preview 5

## [5.0.0-beta] - 2019-04-23
### Changed
- Use Swashbuckle.AspNetCore 5.0.0-beta
- Drop support for .NET Standard 1.6 and .NET Framework, since Swashbuckle.AspNetCore doesn't support them any more
- Only set request example on the operation, no longer set it on the type. This means you can have different
 request examples for different operations which use the same request type, which is an often requested feature.
### Added
- XML examples
### Deprecated
- Removed AuthorizationInputOperationFilter
- Removed DescriptionOperationFilter
- Removed AddFileParamTypesOperationFilter

### [4.5.5] - 2018-03-04
### Fixed
- Repository URL in NuGet package
- Issue #89 - use Json.NET SerializationBinder property when generating examples. Thanks @dmitry-baryshev for the PR.

## [4.5.4] - 2018-02-08
### Deprecated
- Marked AddFileParamTypesOperationFilter as Obsolete, because Swashbuckle 4.0 supports IFormFile directly.
### Changed
- Issue #84 - allow security schema to have a name other than "oauth2" via configuration

## [4.5.3] - 2018-01-14
### Fixed
- Issue #80 - allow interfaces when resolving IExampleProvider<T>

## [4.5.2] - 2018-12-02
### Fixed
- Issue #69
  - Only set request examples on the schema registry object. The request parameter will only be set if 
  a schema registry object is not found. This fix prevents a warning in Redoc. Thanks @Leon99 for the
  pull request.
- Issue #72 do not override SerializerSettings.NullValueHandling because underlying issue seems to have been fixed

## [4.5.1] - 2018-11-12
### Fixed
- Issue #67 3rd time
  - Support Swashbuckle.AspNetCore 4.0.0 for .NET Framework 4.6.1 projects

## [4.5.0] - 2018-11-12
### Fixed
- Issue #67 again
  - Support Swashbuckle.AspNetCore 4.0.0 for .NET Framework 4.6.1 projects

## [4.4.0] - 2018-11-08
### Fixed
- Issue #67
  - Support Swashbuckle.AspNetCore 4.0.0 for .NET Standard 2.0 projects

## [4.3.1] - 2018-10-08
### Added
- Fix issue #63
  - Add an optional true/false value to the AddHeaderOperationFilter to determine whether the header
    is required or not.

## [4.3.0] - 2018-09-14
### Changed
- Issue #60
  - No longer support .NET Framework 4.5.1 because it doesn't work with Scrutor (because Scrutor is unsigned)
  - Support .NET Framework 4.6.1
  - Support .NET Standard 2.0 (still support 1.6 too)

### Deprecated
- Mark DescriptionOperationFilter as obsolete, because you can accomplish the same thing with summary tags

## [4.2.0] - 2018-08-15
### Changed
- It is no longer necessary to specify a ProducesResponseType or SwaggerResponse attribute in order to get
response examples, so long as it is obvious what Type your action method returns.

## [4.1.0] - 2018-08-06
### Added
- Add generic version of SecurityRequirementsOperationFilter and AppendAuthorizeToSummaryOperationFilter so that
they can be used with other attributes. Reason: a client had implemented their own `TypeFilterAttribute`
which did Authorization but wasn't an `AuthorizeAttribute`

## [4.0.3] - 2018-08-03
### Fixed
- Issue #54 where child objects weren't having their descriptions set if parent property was missing `[Description]`

## [4.0.2] - 2018-07-26
### Fixed
- Issue #54 where arrays of child objects weren't having their description set

## [4.0.1] - 2018-07-24
### Fixed
- Fix bug where generic types weren't being automatically annotated

## [4.0.0] - 2018-07-23
### Added
- Automatic annotation of request and response examples
- Dependency on [Scrutor](https://github.com/khellang/Scrutor) 2.2.2
### Changed
- How ExamplesOperationFilter is installed - see Installation section of the Readme

## [3.1.0] - 2018-07-21
### Added
- SecurityRequirementsOperationFilter to correctly set bearer token stuff when using `[Authorize]`

## [3.0.4] - 2018-07-18
### Fixed
- Fix bug with DescriptionOperationFilter where Description not set if using a DefaultContractResolver

## [3.0.3] - 2018-07-17
### Fixed
- Fix where Examples doesn't work on ASP.NET Core 2.0

## [3.0.2] - 2018-07-15
### Fixed
- Port of bug #36 from Swashbuckle.Examples

## [3.0.1] - 2018-07-15
### Changed
- Rename from Swashbuckle.AspNetCore.Examples
- Dependency on [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) 3.0 instead of 1.0