using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Octokit;

namespace GitHubRepoSecrets.Pages
{
    public class SecretsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? Owner { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Repo { get; set; }

        public RepositorySecretsCollection Secrets { get; set; } = new RepositorySecretsCollection();

        public async Task OnGetAsync()
        {
            if (User.Identity?.IsAuthenticated is false)
            {
                return;
            }
            var accessToken = User.FindFirst("access_token")?.Value;

            var client = new GitHubClient(new ProductHeaderValue("test"))
            {
                Credentials = new Credentials(accessToken)
            };

            Secrets = await client.Repository.Actions.Secrets.GetAll(Owner, Repo);
        }
    }
}
