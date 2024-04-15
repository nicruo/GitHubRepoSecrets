using GitHubRepoSecrets.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Octokit;

namespace GitHubRepoSecrets.Pages.Secrets
{
    public class DetailsModel : PageModel
    {
        private readonly GitHubService gitHubService;

        [BindProperty(SupportsGet = true)]
        public string? Owner { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Repo { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SecretName { get; set; }

        public RepositorySecret Secret { get; set; } = default!;

        public DetailsModel(GitHubService gitHubService)
        {
            this.gitHubService = gitHubService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity?.IsAuthenticated is false)
            {
                return Unauthorized();
            }

            Secret = await gitHubService.GetSecretAsync(Owner, Repo, SecretName);

            return Page();
        }
    }
}