using Microsoft.AspNetCore.Mvc;
using Shelngn.Business.FileUpload;
using Shelngn.FileUploadApi;
using Shelngn.Services.FileUpload;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddLogging();
services.AddCors();
services.AddSingleton<IFileUploadUrlSigning, FileUploadUrlSigning>(opt => new FileUploadUrlSigning(configuration.GetValue<string>("SigningPrivateKey")));
services.AddSingleton<FileUploader>(new FileUploader(configuration.GetValue<string>("ProjectsDirectory")));

var app = builder.Build();

// HTTP request pipeline.
app.UseCors(options => options
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());
app.UseHttpsRedirection();

app.MapPost("/{*filePath}", async (
    [FromRoute(Name = "filePath")] string filePath,
    [FromQuery(Name = "sign")] string signature,
    [FromHeader(Name = "Content-Range")] string contentRange,
    [FromHeader(Name = "Content-Type")] string contentType,
    HttpContext httpContext,
    IFileUploadUrlSigning fileUploadUrlSigning,
    FileUploader fileUploader) =>
{
    if (!fileUploadUrlSigning.ValidateSignature(signature, filePath, contentType))
    {
        httpContext.Response.Headers.Add("X-Reason", "Signature mismatch");
        return Results.StatusCode(403);
    }
    if (!ContentRangeHeaderValue.TryParse(contentRange, out var contentRangeValue))
    {
        httpContext.Response.Headers.Add("X-Reason", "Content-Range is malformed");
        return Results.BadRequest();
    }

    using var memoryBuffer = new MemoryStream();
    await httpContext.Request.Body.CopyToAsync(memoryBuffer);
    memoryBuffer.Position = 0;

    if (contentRangeValue.From == 0)
    {
        fileUploader.DeleteIfExists(filePath);
    }

    await fileUploader.CreateOrAppendAsync(filePath, memoryBuffer);

    return Results.NoContent();
});

app.Run();
