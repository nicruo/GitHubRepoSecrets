using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using GitHubRepoSecrets.Services;

var builder = WebApplication.CreateBuilder(args);

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
        options.ClientSecret = builder.Configuration["github:clientSecret"];
        options.ClientId = builder.Configuration["github:clientId"];
        options.CallbackPath = "/signin-github";
        options.Scope.Add("admin:org");
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
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<GitHubService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

//test endpoint
app.MapGet("/test", async ctx =>
{
    var gitHubService = ctx.RequestServices.GetRequiredService<GitHubService>();
    await ctx.Response.WriteAsync("Hello World!");
}); 

app.Run();