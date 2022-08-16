using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OnlineShopWebApp.DataModels;
using OnlineShopWebApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMvc();

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ShopContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHttpContextAccessor();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        //Title = "Swagger for Volunteers project API",
        Title = $"{builder.Configuration["SwaggerDocs_ProductName"]} API",
        //Description = "A simple example ASP.NET Core Web API",
        //TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            //Name = "Shayne Boyer",
            Name = builder.Configuration["SwaggerDocs_ContactName"],
            //Email = string.Empty,
            Email = builder.Configuration["SwaggerDocs_ContactEmail"]
            //Url = new Uri("https://twitter.com/spboyer"),
        },
        License = new OpenApiLicense
        {
            //Name = "Use under LICX",
            //Url = new Uri("https://example.com/license"),
        }
    });

    c.TagActionsBy(api =>
    {
        if (api.GroupName != null)
        {
            return new[] { api.GroupName };
        }

        if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            return new[] { controllerActionDescriptor.ControllerName };
        }

        throw new InvalidOperationException("Unable to determine tag for endpoint.");
    });

    c.DocInclusionPredicate((name, api) => true);
});

builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IClientRepository, ClientRepository>();
builder.Services.AddTransient<IGenderRepository, GenderRepository>();
builder.Services.AddTransient<IStorageRepository, StorageRepository>();
builder.Services.AddTransient<IOrderedProductRepository, OrderedProductRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee API V1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=account}/{action=Login}/{id?}");
});

app.Run();
