using Bookify.Models;
using Bookify.Data;
using Bookify.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Base de données (SQL Server LocalDB intégré à Visual Studio) ---
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDb>(options =>
    options.UseSqlServer(cs));

// --- MVC Controllers + Views ---
builder.Services.AddControllersWithViews();

// --- Services OpenLibrary et Amazon ---
builder.Services.AddHttpClient<OpenLibraryService>(c =>
{
    c.Timeout = TimeSpan.FromSeconds(5);
    c.DefaultRequestHeaders.UserAgent.ParseAdd("Bookify/1.0");
});
builder.Services.AddSingleton(new AmazonLinkBuilder(defaultMarket: "com", affiliateTag: null));
builder.Services.AddScoped<AuthorizationService>();

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bookify API",
        Version = "v1",
        Description = "API REST pour la gestion des livres et des genres"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AllowAngular");

// --- Pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// --- Swagger ---
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bookify API v1");
    c.RoutePrefix = ""; // Swagger sur http://localhost:xxxx/
});

// --- Middleware ---
app.UseHttpsRedirection();
app.UseStaticFiles();

// --- Routing & Authorization ---
app.UseRouting();
app.UseAuthorization();

app.MapControllers();             // API Controllers
app.MapDefaultControllerRoute();  // MVC Views

// --- Initialisation de la base avec des données de test ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDb>();
    context.Database.EnsureCreated();
    DbInitializer.Seed(context);
}

app.Run();
