using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBestJob.DAL.Database.Models;

[Table("Users")]
[CollectionName("Users")]
public class User : MongoIdentityUser<Guid>, IBaseModel<Guid>
{
    [Required]
    public string LastName { get; set; } = string.Empty;
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string FullName => $"{LastName} {FirstName}";

    [Required]
    public bool IsGoogleAccount { get; set; }

    [Required]
    public bool IsFacebookAccount { get; set; }

    public string? AvatarBase64 { get; set; }
    public string? AvatarUrl { get; set; }

    public DateTime? LastChangedDate { get; set; }

    public Guid? CreatorUserId { get; set; }
    public User? CreatorUser { get; set; }
}
