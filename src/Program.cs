using GitHubRepoSecrets.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var githubClientId = builder.Configuration["github:clientId"];
if (string.IsNullOrEmpty(githubClientId))
{
    throw new ArgumentException("GITHUB__CLIENTID is required");
}

var githubClientSecret = builder.Configuration["github:clientSecret"];
if (string.IsNullOrEmpty(githubClientSecret))
{
    throw new ArgumentException("GITHUB__CLIENTSECRET is required");
}

builder.Services
    .AddAuthentication(o =>
    {
        o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(o =>
    {
        o.LoginPath = "/Index";
        o.LogoutPath = "/signout";
    })
    .AddGitHub(options =>
    {
        options.ClientSecret = githubClientSecret;
        options.ClientId = githubClientId;
        options.CallbackPath = "/signin-github";
        options.Scope.Add("repo");
        options.Events.OnCreatingTicket += context =>
        {
            if (context.AccessToken is { })
            {
                context.Identity?.AddClaim(new Claim("access_token", context.AccessToken));
            }
            return Task.CompletedTask;
        };
    });

builder.Services.AddRazorPages();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<GitHubService>();

builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{ 
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/signout", async ctx =>
{
    await ctx.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new AuthenticationProperties
        {
            RedirectUri = "/"
        });
});

app.Run();