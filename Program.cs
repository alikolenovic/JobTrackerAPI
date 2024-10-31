using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);


// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<JobTrackerDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbConnection")));
    builder.Services.AddDistributedMemoryCache();
}
else
{
    // Connect to SQL
    builder.Services.AddDbContext<JobTrackerDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")));

    // Set up Redis
    builder.Services.AddStackExchangeRedisCache(options =>
    {
    options.Configuration = builder.Configuration["AZURE_REDIS_CONNECTIONSTRING"];
    options.InstanceName = "SampleInstance";
    });
}

// Configure Azure AD B2C authentication
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddMicrosoftIdentityWebApi(options =>
//     {
//         builder.Configuration.Bind("AzureAdB2C", options);
//         options.TokenValidationParameters.NameClaimType = "name"; // Use "name" as the unique identifier in tokens
//     },
//     options => { builder.Configuration.Bind("AzureAdB2C", options); });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

// Add services to the container.
builder.Services.AddControllers();
// Add other necessary services and middlewares
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();  // Ensure Authorization middleware is present

// Map controller endpoints
app.MapControllers();

app.Run();
