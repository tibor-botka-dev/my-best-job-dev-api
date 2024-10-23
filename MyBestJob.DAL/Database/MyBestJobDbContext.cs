using MongoFramework;
using MongoFramework.Infrastructure;
using MyBestJob.DAL.Database.Models;

namespace MyBestJob.DAL.Database;

public class MyBestJobDbContext(IMongoDbConnection connection) : MongoDbContext(connection)
{
    public MongoDbSet<User> Users { get; set; } = null!;
    public MongoDbSet<Role> Roles { get; set; } = null!;
    public MongoDbSet<MailSetting> MailSettings { get; set; } = null!;
    public MongoDbSet<Language> Languages { get; set; } = null!;

    public override async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetUpBaseDateEntities();

        await base.SaveChangesAsync(cancellationToken);
    }

    public override void SaveChanges()
    {
        SetUpBaseDateEntities();

        base.SaveChanges();
    }

    private void SetUpBaseDateEntities()
    {
        var entities = ChangeTracker.Entries().Where(x => x is
        { Entity: BaseModel, State: EntityEntryState.Added or EntityEntryState.Updated });

        var utcNow = DateTime.UtcNow;

        foreach (var entity in entities)
        {
            if (entity.Entity is BaseModel baseEntity)
            {
                if (entity.State == EntityEntryState.Added)
                {
                    baseEntity.CreatedDate = utcNow;
                }

                baseEntity.LastChangedDate = utcNow;
            }
        }
    }
}
