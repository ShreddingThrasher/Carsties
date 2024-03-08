using AuctionService.Consumers;
using AuctionService.Data;
using AuctionService.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<AuctionDbContext>(options => 
{
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configure MassTransit
builder.Services.AddMassTransit(x => 
{
	x.AddEntityFrameworkOutbox<AuctionDbContext>(options => 
	{
		options.QueryDelay = TimeSpan.FromSeconds(10);
		
		options.UsePostgres();
		options.UseBusOutbox();
	});
	
	x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();
	
	x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));
	
	x.UsingRabbitMq((context, cfg) => 
	{
		cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host => 
		{
			// guest is the default value in case if nothing is found in the configuration
			host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
			host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
		});
		
		cfg.ConfigureEndpoints(context);
	});
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options => 
	{
		// From who was issued the token
		options.Authority = builder.Configuration["IdentityServiceUrl"];
		options.RequireHttpsMetadata = false;
		options.TokenValidationParameters.ValidateAudience = false;
		options.TokenValidationParameters.NameClaimType = "username";
	});
	
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcAuctionService>();

try
{
	DbInitializer.InitDb(app);
}
catch (Exception e)
{
	Console.WriteLine(e);
}

app.Run();
