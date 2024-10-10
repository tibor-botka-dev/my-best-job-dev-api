using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBestJob.DAL.Database.Models;

[Table("Roles")]
[CollectionName("Roles")]
public class Role : MongoIdentityRole<Guid>, IBaseModel<Guid>
{ }
