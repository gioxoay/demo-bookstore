using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using BookStore.Data;
using BookStore.Services;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System.Text;
using System.Text.Json.Serialization;
using TabBlazor;
using BookStore.Data.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
});

builder.Services
    .AddServerSideBlazor()
    .AddHubOptions(x => x.MaximumReceiveMessageSize = 102400000);

builder.Services.AddMvcCore()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .AddApiExplorer();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddSwaggerGen(cfg =>
{
    cfg.CustomSchemaIds(type => {
        string returnedValue = type.Name;
        if (returnedValue.EndsWith("Dto"))
            returnedValue = returnedValue.Replace("Dto", string.Empty);
        return returnedValue;
    });
});
builder.Services.AddHttpClient();
builder.Services.AddTabler();

var mongoConnection = builder.Configuration.GetConnectionString("MongoConnection");

// MongoDB
var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);
builder.Services.AddSingleton(sp => new MongoClient(mongoConnection));
builder.Services.AddSingleton(sp =>
{
    var url = new MongoUrl(mongoConnection);
    var client = sp.GetRequiredService<MongoClient>();
    return client.GetDatabase(url.DatabaseName ?? "default");
});

// Repository
builder.Services.AddTransient(typeof(IRepository<>), typeof(MongoDBRepository<>));

// Auto Mapper
builder.Services.AddAutoMapper(cfg =>
{
       
}, typeof(Program).Assembly);

#region Identity

builder.Services.AddIdentityMongoDbProvider<User, Role>(identity =>
{

}, options =>
{
    options.ConnectionString = mongoConnection;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = (context) =>
        {
            if (context.Request.Path.Value?.StartsWith("/api") == true)
            {
                context.Response.Clear();
                context.Response.StatusCode = 401;
                return Task.FromResult(0);
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.FromResult(0);
        }
    };
})
.AddJwtBearer(options =>
{
    var signingKey = Encoding.UTF8.GetBytes(builder.Configuration["Authentication:Jwt:SigningSecret"]);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Authentication:Jwt:Issuer"],
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(signingKey)
    };
});

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    //options.FallbackPolicy = options.DefaultPolicy;
});

#endregion

#region Book Store

// Mongo Collections
builder.Services.AddTransient(sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<Store>("Stores"));
builder.Services.AddTransient(sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<Book>("Books"));
builder.Services.AddTransient(sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<BookInStore>("BookInStores"));
builder.Services.AddTransient(sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<Order>("Orders"));

// Services
builder.Services.AddTransient<StoreService>();
builder.Services.AddTransient<BookService>();
builder.Services.AddTransient<OrderService>();

#endregion

builder.Services.AddSingleton<AppService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseSwagger();

app.UseSwaggerUI();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapBlazorHub();

app.MapFallbackToPage("/_Host");

app.Run();
