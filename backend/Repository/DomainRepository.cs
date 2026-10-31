using App.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace App.Repository;

public interface IDomainRepository
{
  IQueryable<T> Query<T>() where T : class;
  Task<T> Find<T>(Guid id) where T : class;
  Task<T> Lock<T>(Guid id) where T : class;
  void Add<T>(T item) where T : class;
  void Remove<T>(T item) where T : class;
  Task Save(); Task<IDbContextTransaction> Begin(); Task<bool> Healthy();
}
public class DomainRepository(AppDbContext db) : IDomainRepository
{
  public IQueryable<T> Query<T>() where T : class => db.Set<T>();
  public async Task<T> Find<T>(Guid id) where T : class => await db.Set<T>().FindAsync(id) ?? throw new App.Common.ApiException(404, "KayÄ±t bulunamadÄ±.");
  public async Task<T> Lock<T>(Guid id) where T : class
  {
    var table = db.Model.FindEntityType(typeof(T))!.GetTableName()!;
    var sql = "SELECT * FROM " + table + " WHERE id={0} FOR UPDATE";
    return await db.Set<T>().FromSqlRaw(sql, id).SingleOrDefaultAsync() ?? throw new App.Common.ApiException(404, "KayÄ±t bulunamadÄ±.");
  }
  public void Add<T>(T item) where T : class => db.Add(item);
  public void Remove<T>(T item) where T : class => db.Remove(item);
  public Task Save() => db.SaveChangesAsync();
  public Task<IDbContextTransaction> Begin() => db.Database.BeginTransactionAsync();
  public Task<bool> Healthy() => db.Database.CanConnectAsync();
}
