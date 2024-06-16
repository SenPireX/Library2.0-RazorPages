using Library.Application.Model;
using Microsoft.EntityFrameworkCore;

namespace Library.Application.Infrastructure.Repositories;

// Abstract repository class for CRUD operations
public abstract class Repository<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : struct 
{
    
    // Injecting Database
    protected readonly LibraryContext _db;
    
    public IQueryable<TEntity> Set => _db.Set<TEntity>();

    // Constructor for database Injection
    protected Repository(LibraryContext db) 
    {
        _db = db;
    }

    // Find an entity by its primary key
    public TEntity? FindById(TKey id) => _db.Set<TEntity>().FirstOrDefault(e => e.Id.Equals(id));
    
    // Insert a new entity into the databaseUsing Library.Model;
    public virtual (bool success, string message) Create(TEntity entity) {
        _db.Entry(entity).State = EntityState.Added;
        try 
        {
            _db.SaveChanges();
        } 
        catch (DbUpdateException ex) 
        {
            return (false, ex.InnerException?.Message ?? ex.Message);
        }
        return (true, string.Empty);
    }
    
    // Update an existing entity in the database
    public virtual (bool success, string message) Update(TEntity entity) 
    {
        if (!HasPrimaryKey(entity)) 
        {
            return (false, "Missing primary key.");
        }

        // Attach the entity to the context
        _db.Set<TEntity>().Attach(entity);

        // Mark only the properties that have changed as modified
        var entry = _db.Entry(entity);
        foreach (var property in entry.Properties) 
        {
            if (property.Metadata.IsPrimaryKey()) 
            {
                property.IsModified = false; // Ensure the primary key is not marked as modified
            } 
            else 
            {
                var originalValue = property.OriginalValue;
                var currentValue = property.CurrentValue;

                if (!Equals(originalValue, currentValue)) 
                {
                    property.IsModified = true;
                } 
                else 
                {
                    property.IsModified = false;
                }
            }
        }
        try
        {
            _db.SaveChanges();
            return (true, string.Empty);
        }
        catch (DbUpdateException ex)
        {
            return (false, ex.InnerException?.Message ?? ex.Message);
        }
    }

    
    // Delete an existing entity from the database
    public virtual (bool success, string message) Delete(TEntity entity) 
    {
        if (!HasPrimaryKey(entity)) 
        {
            return (false, "Missing primary key.");
        }
        _db.Entry(entity).State = EntityState.Deleted;
        try 
        {
            _db.SaveChanges();
        } 
        catch (DbUpdateException ex) 
        {
            return (false, ex.Message);
        }
        return (true, string.Empty);
    }

    // Check if the entity has a primary key
    private bool HasPrimaryKey(TEntity entity) => !entity.Id.Equals(default);
}
