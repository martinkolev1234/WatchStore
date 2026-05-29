using Microsoft.OpenApi;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Serilog;
using WatchStore.Api.HealthChecks;
using WatchStore.Api.Middleware;
using WatchStore.BL;
using WatchStore.DL;

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting WatchStore API...");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    var mongoSettings = builder.Configuration.GetSection("MongoDbSettings");
    var connectionString = mongoSettings.GetValue<string>("ConnectionString");
    var databaseName = mongoSettings.GetValue<string>("DatabaseName");

    builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
    builder.Services.AddScoped<IMongoDatabase>(sp =>
        sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));

    builder.Services.AddDataLayer(builder.Configuration);
    builder.Services.AddBusinessLayer();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "WatchStore API", Version = "v1" });

        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    });

    builder.Services.AddHealthChecks()
        .AddCheck<DatabaseHealthCheck>("MongoDB");

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}