using GitHubRepoSecrets.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Octokit;

namespace GitHubRepoSecrets.Pages.Secrets
{
    public class IndexModel : PageModel
    {
        private readonly GitHubService gitHubService;

        [BindProperty(SupportsGet = true)]
        public string? Owner { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Repo { get; set; }

        public RepositorySecretsCollection Secrets { get; set; } = new RepositorySecretsCollection();

        public IndexModel(GitHubService gitHubService)
        {
            this.gitHubService = gitHubService;
        }

        public async Task OnGetAsync()
        {
            if (User.Identity?.IsAuthenticated is false)
            {
                return;
            }

            Secrets = await gitHubService.GetSecretsAsync(Owner, Repo);
        }
    }
}
