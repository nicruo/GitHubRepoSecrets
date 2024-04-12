using GitHubRepoSecrets.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Octokit;

namespace GitHubRepoSecrets.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly GitHubService gitHubService;

    public IEnumerable<AuthenticationScheme> Schemes { get; set; }
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }
    public IReadOnlyList<Repository> Repositories { get; set; } = new List<Repository>();

    public IndexModel(ILogger<IndexModel> logger, GitHubService gitHubService)
    {
        _logger = logger;
        this.gitHubService = gitHubService;
    }

    public async Task<IActionResult> OnPost([FromForm] string provider)
    {
        if (string.IsNullOrWhiteSpace(provider))
        {
            return BadRequest();
        }

        return await IsProviderSupportedAsync(HttpContext, provider) is false
            ? BadRequest()
            : Challenge(new AuthenticationProperties
            {
                RedirectUri = Url.IsLocalUrl(ReturnUrl) ? ReturnUrl : "/"
            }, provider);
    }

    private static async Task<AuthenticationScheme[]> GetExternalProvidersAsync(HttpContext context)
    {
        var schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
        return (await schemes.GetAllSchemesAsync())
            .Where(scheme => !string.IsNullOrEmpty(scheme.DisplayName))
            .ToArray();
    }

    private static async Task<bool> IsProviderSupportedAsync(HttpContext context, string provider) =>
        (await GetExternalProvidersAsync(context))
        .Any(scheme => string.Equals(scheme.Name, provider, StringComparison.OrdinalIgnoreCase));

    public async Task OnGetAsync()
    {
        if(User.Identity?.IsAuthenticated is false)
        {
            Schemes = await GetExternalProvidersAsync(HttpContext);
            return;
        }

        Repositories = await gitHubService.GetRepositoriesAsync();
    }
}