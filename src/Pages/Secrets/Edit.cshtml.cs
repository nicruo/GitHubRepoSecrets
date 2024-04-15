using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GitHubRepoSecrets.Models;
using GitHubRepoSecrets.Services;

namespace GitHubRepoSecrets.Pages.Secrets
{
    public class EditModel : PageModel
    {
        private readonly GitHubService gitHubService;

        [BindProperty(SupportsGet = true)]
        public string? Owner { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Repo { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SecretName { get; set; }

        public EditModel(GitHubService gitHubService)
        {
            this.gitHubService = gitHubService;
        }

        [BindProperty]
        public SecretModel SecretModel { get; set; } = default!;

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated is false)
            {
                return Unauthorized();
            }

            SecretModel = new SecretModel { Name = SecretName };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (User.Identity?.IsAuthenticated is false)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await gitHubService.CreateOrUpdateSecretAsync(Owner, Repo, SecretName, SecretModel.Value);

            return RedirectToPage("./Index", new { Owner, Repo });
        }
    }
}