using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using RestAPIBackendWebService.DataAccess;
using RestAPIBackendWebService.Domain.Services.Localization;
using RestAPIBackendWebService.Extensions;
using RestAPIBackendWebService.Services.Localization.Contract;
using RestAPIBackendWebService.Middleware;
using NLog;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<RestAPIDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDB")),
   ServiceLifetime.Scoped,
   ServiceLifetime.Scoped
);

builder.Services.AddHttpContextAccessor();

builder.Services.RegisterDependencies();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en-US");
});

//builder.Services.ConfigureCors();

builder.Services.ConfigureAuthentication(builder.Configuration);

builder.Services.AddControllers()
    .ConfigureResponseForInvalidModelsState();

builder.Services.ConfigureApiVersion();

//builder.Services.AddHttpClient();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureIdentity(builder.Configuration);

//builder.Services.ConfigureCors();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicyCors", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:4200", "https://bostonlistaprecios.dev.softipal.com");
        policyBuilder.AllowAnyHeader();
        policyBuilder.AllowAnyMethod();
    });
});

var app = builder.Build();


//Initialize translations
ApplicationTranslations.InitializeTranslations(
    identityErrors: app.Services.GetService<ILocalizationService>().GetTranslationsByFileName("identity-errors.json"),
    dataAnnotations: app.Services.GetService<ILocalizationService>().GetTranslationsByFileName("customDataAnnotations-errors.json"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("MyPolicyCors");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//app.UseCors("LocalApp");

app.UseMiddleware<ExceptionHandler>();

app.Run();

