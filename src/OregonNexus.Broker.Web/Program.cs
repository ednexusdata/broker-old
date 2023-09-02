// Copyright: 2023 Education Nexus Oregon
// Author: Makoa Jacobsen, makoa@makoajacobsen.com

using Microsoft.EntityFrameworkCore;
using OregonNexus.Broker.Data;
using MediatR;
using Autofac;
using OregonNexus.Broker.SharedKernel;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using InertiaAdapter.Extensions;
using OregonNexus.Broker.Web;
using OregonNexus.Broker.Web.Services;
using System.Reflection;
using OregonNexus.Broker.Domain;
using Microsoft.AspNetCore.Authentication;
using OregonNexus.Broker.Connector;

var builder = WebApplication.CreateBuilder(args);

// Add Autofac
//builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Add services to the container.
builder.Services.AddHttpContextAccessor();
//builder.Services.AddScoped<ScopedHttpContext>();
builder.Services.AddMediatR(typeof(Program).Assembly);

var msSqlConnectionString = builder.Configuration.GetConnectionString("MsSqlBrokerDatabase") ?? throw new InvalidOperationException("Connection string 'MsSqlBrokerDatabase' not found.");
var pgSqlConnectionString = builder.Configuration.GetConnectionString("PgSqlBrokerDatabase") ?? throw new InvalidOperationException("Connection string 'PgSqlBrokerDatabase' not found.");

builder.Services.AddDbContext<BrokerDbContext>(options => {
    if (msSqlConnectionString is not null && msSqlConnectionString != "")
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("MsSqlBrokerDatabase")!,
            x => x.MigrationsAssembly("OregonNexus.Broker.Data.Migrations.SqlServer")
        );
    }
    if (pgSqlConnectionString is not null && pgSqlConnectionString != "")
    {
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("PgSqlBrokerDatabase")!,
            x => x.MigrationsAssembly("OregonNexus.Broker.Data.Migrations.PostgreSQL")
        );
    }
}
);

builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped(typeof(IMediator), typeof(Mediator));

foreach(var assembly in Assembly.GetExecutingAssembly().GetTypes().Where(t => String.Equals(t.Namespace, "OregonNexus.Broker.Web.Helpers", StringComparison.Ordinal)).ToArray())
{
    builder.Services.AddScoped(assembly, assembly);
}

builder.Services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(options =>
{
    options.User.RequireUniqueEmail = false;
})
.AddEntityFrameworkStores<BrokerDbContext>()
.AddTokenProvider<DataProtectorTokenProvider<IdentityUser<Guid>>>(TokenOptions.DefaultProvider);

builder.Services.ConfigureApplicationCookie(options => 
{
    options.AccessDeniedPath = "/AccessDenied";
    options.Cookie.Name = "OregonNexus.Broker";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Login";
    // ReturnUrlParameter requires 
    //using Microsoft.AspNetCore.Authentication.Cookies;
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;
});

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".OregonNexus.Broker.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(60);
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    })
    .AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"]!;
        microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"]!;
    }
);

builder.Services.AddAuthorization(options => {
    options.AddPolicy("SuperAdmin",
      policy => policy.RequireClaim("SuperAdmin", "true")
    );

    options.AddPolicy("AllEducationOrganizations",
      policy => policy.RequireClaim("AllEducationOrganizations", PermissionType.Read.ToString(), PermissionType.Write.ToString())
    );

    options.AddPolicy("TransferRecords",
      policy => policy.RequireClaim("TransferRecords", "true")
    );
});
builder.Services.AddTransient<IClaimsTransformation, BrokerClaimsTransformation>();

builder.Services.AddControllersWithViews();
builder.Services.AddInertia();

builder.Services.AddScoped<ICurrentUser, CurrentUserService>();

builder.Services.AddConnectorLoader();
builder.Services.AddConnectorDependencies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseInertia();

app.UseHttpMethodOverride(new HttpMethodOverrideOptions()
{
    FormFieldName = "_METHOD"
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

//app.UseMiddleware<ScopedHttpContextMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
