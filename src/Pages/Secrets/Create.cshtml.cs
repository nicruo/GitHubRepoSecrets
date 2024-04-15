using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GitHubRepoSecrets.Models;
using GitHubRepoSecrets.Services;

namespace GitHubRepoSecrets.Pages.Secrets
{
    public class CreateModel : PageModel
    {
        private readonly GitHubService gitHubService;

        [BindProperty(SupportsGet = true)]
        public string? Owner { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Repo { get; set; }

        public CreateModel(GitHubService gitHubService)
        {
            this.gitHubService = gitHubService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public SecretModel SecretModel { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await gitHubService.CreateOrUpdateSecretAsync(Owner, Repo, SecretModel.Name, SecretModel.Value);

            return RedirectToPage("./Index", new { Owner, Repo });
        }
    }
}
