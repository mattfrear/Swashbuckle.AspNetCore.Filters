# Swashbuckle.AspNetCore.Examples
A simple library which adds the `[SwaggerRequestExample]` and `[SwaggerResponseExample]` attributes to [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).

Example request:
https://mattfrear.com/2016/01/25/generating-swagger-example-requests-with-swashbuckle/ 

This will populate swagger's `definitions.YourObject.example` with whatever object you like.

Example response: 
https://mattfrear.com/2015/04/21/generating-swagger-example-responses-with-swashbuckle/

This will populate swagger's `paths.YourEndpoint.post.responses.200.examples` with whatever object you like.
