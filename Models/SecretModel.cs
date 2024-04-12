using System.ComponentModel.DataAnnotations;

namespace GitHubRepoSecrets.Models;

public class SecretModel
{
    [Required]
    public string Name { get; set; }

    [DataType(DataType.Password)]
    public string Value { get; set; }
}