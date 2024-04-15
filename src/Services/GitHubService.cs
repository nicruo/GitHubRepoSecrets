using Octokit;

namespace GitHubRepoSecrets.Services;

public class GitHubService
{
    private readonly GitHubClient _client;

    public GitHubService(IHttpContextAccessor httpContextAccessor)
    {
        var accessToken = httpContextAccessor.HttpContext.User.FindFirst("access_token")?.Value;

        if (accessToken is not null)
        {
            _client = new GitHubClient(new ProductHeaderValue("test"))
            {
                Credentials = new Credentials(accessToken)
            };
        }
    }

    public async Task<IReadOnlyList<Repository>> GetRepositoriesAsync()
    {
        return await _client.Repository.GetAllForCurrent(new RepositoryRequest { Sort = RepositorySort.Updated, Direction = SortDirection.Descending});
    }

    public async Task<RepositorySecretsCollection> GetSecretsAsync(string owner, string repo)
    {
        return await _client.Repository.Actions.Secrets.GetAll(owner, repo);
    }

    public async Task<RepositorySecret> GetSecretAsync(string owner, string repo, string secretName)
    {
        return await _client.Repository.Actions.Secrets.Get(owner, repo, secretName);
    }

    public async Task DeleteSecretAsync(string owner, string repo, string secretName)
    {
        await _client.Repository.Actions.Secrets.Delete(owner, repo, secretName);
    }

    public async Task CreateOrUpdateSecretAsync(string owner, string repo, string secretName, string secretValue)
    {
        SecretsPublicKey secretsPublicKey = await _client.Repository.Actions.Secrets.GetPublicKey(owner, repo);
        var secretsPublicKeyBytes = Convert.FromBase64String(secretsPublicKey.Key);
        var encryptedValue = Convert.ToBase64String(Sodium.SealedPublicKeyBox.Create(secretValue, secretsPublicKeyBytes));
        await _client.Repository.Actions.Secrets.CreateOrUpdate(owner, repo, secretName, new UpsertRepositorySecret { KeyId = secretsPublicKey.KeyId, EncryptedValue = encryptedValue });
    }
}