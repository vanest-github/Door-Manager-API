using System.Text.Json.Serialization;
using DoorManager.Api.Extensions;
using DoorManager.Service.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DoorManager.Api", Version = "v1" });
    c.SwaggerDoc("DoorManager.Authentication.Api", new OpenApiInfo { Title = "DoorManager.Authentication.Api", Version = "v1" });
});
builder.Services.AddApiVersionExtensions();
builder.Services.AddConfigurations(builder.Configuration);
builder.Services.AddServices();
builder.Services.AddMediatRServices();
builder.Services.AddRepositorties(builder.Configuration);
builder.Services.AddCors();
builder.Services.AddMvc().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DoorManager.Api v1");
    c.SwaggerEndpoint("/swagger/DoorManager.Authentication.Api/swagger.json", "DoorManager.Authentication.Api");
});
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();