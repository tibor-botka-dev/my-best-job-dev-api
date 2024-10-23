using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MyBestJob.DAL.Database.Models;

public interface IBaseModel<T>
{
    T Id { get; set; }
}

public class BaseModel : IBaseModel<Guid>
{
    [BsonId]
    public Guid Id { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    [Required]
    public DateTime LastChangedDate { get; set; }

    public Guid? UpdatedByUserId { get; set; }
}
