using NotaFiscalAPI.Services;
using Repository.Context;

namespace NotaFiscalAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.ConfigureHttpJsonOptions(e =>
        {
            e.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

        builder.Services.AddTransient<INotaFiscalService, NotaFiscalService>();
        builder.Services.AddSqlServer<KorpContext>(
            builder.Configuration.GetConnectionString("dbConnection"),
            op => op.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
            );

        builder.Services.AddCors(e => e.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }));

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
