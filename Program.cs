using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using JobTrackerAPI.GraphQL.Mutations;

var builder = WebApplication.CreateBuilder(args);

// Define allowed origins for CORS
string devFrontendOrigin = "http://localhost:3000"; // Local development URL
string prodFrontendOrigin = "https://your-production-frontend.com"; // Production frontend URL

// Add CORS policies
builder.Services.AddCors(options =>
{
    // Development CORS policy
    options.AddPolicy("DevelopmentCorsPolicy", builder =>
    {
        builder.WithOrigins(devFrontendOrigin)
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });

    // Production CORS policy
    options.AddPolicy("ProductionCorsPolicy", builder =>
    {
        builder.WithOrigins(prodFrontendOrigin)
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddPooledDbContextFactory<JobTrackerDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

     // Use local Redis for development - Make sure to run the docker image
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = "localhost:6379";
    });
}
else
{
    // Connect to SQL
    builder.Services.AddPooledDbContextFactory<JobTrackerDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")));

    // Set up Redis
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration["AZURE_REDIS_CONNECTIONSTRING"];
        options.InstanceName = "SampleInstance";
    });
}


builder.Services.AddHttpContextAccessor();

// Register scoped services
builder.Services.AddScoped<JobService>();
builder.Services.AddScoped<JobMutations>();
builder.Services.AddScoped<UserMutations>();
builder.Services.AddScoped<Mutation>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

// Configure GraphQL
builder.Services
    .AddGraphQLServer()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true)
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
        .AddTypeExtension<JobMutations>()
        .AddTypeExtension<UserMutations>() 
    .AddType<JobType>()
    .AddType<UserType>()
    .AddFiltering()
    .AddSorting();

var app = builder.Build();

// Apply the appropriate CORS policy based on the environment
app.UseCors("DevelopmentCorsPolicy");

// Use when Production Frontend Exists
// if (app.Environment.IsDevelopment())
// {
//     app.UseCors("DevelopmentCorsPolicy");
// }
// else
// {
//     app.UseCors("ProductionCorsPolicy");
// }

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();

app.Run();
