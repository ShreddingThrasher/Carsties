
using System.Net;
using Polly;
using Polly.Extensions.Http;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionSvcHttpClient>()
	.AddPolicyHandler(GetPolicy());

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

// Initializes DB when the application is fully started
app.Lifetime.ApplicationStarted.Register(async () =>
{
	try
	{
		await DbInitializer.InitDb(app);
	}
	catch (Exception e)
	{
		Console.WriteLine(e);
	}

});


app.Run();

// Policy to handle the AuctionService HttpClient in case DB initialize request fails
static IAsyncPolicy<HttpResponseMessage> GetPolicy()
	=> HttpPolicyExtensions
		.HandleTransientHttpError()
		.OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
		.WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));