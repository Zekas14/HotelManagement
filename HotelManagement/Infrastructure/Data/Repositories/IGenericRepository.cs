using HotelManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace HotelManagement.Infrastructure.Data.Repositories
{
    public interface IGenericRepository<Entity> where Entity : BaseModel
    {
        IQueryable<Entity> GetAll();
        IQueryable<Entity> Get(Expression<Func<Entity,bool>> predicate);
        IQueryable<Entity> GetAllByPage(int PageNumber, int PageSize);
        IQueryable<Entity> GetByPage(Expression<Func<Entity,bool>> predicate, int PageNumber, int PageSize);
        void Add(Entity entity);
        void SaveInclude(Entity entity, params string[] properties);

        void Update(Entity entity);
        public void Delete(int id);

        Entity GetById(int id);
        void SaveChanges();
        Task SaveChangesAsync();
        void AddRange(IEnumerable<Entity> entities);
    }

    public class GenericRepository<Entity>(ApplicationDbContext context) : IGenericRepository<Entity> where Entity : BaseModel
    {
        private readonly ApplicationDbContext _context = context;
        private readonly DbSet<Entity> _dbSet = context.Set<Entity>();

        public void Add(Entity entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            _dbSet.Add(entity);
        }

        public void AddRange(IEnumerable<Entity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            entity.IsDeleted = true;
            SaveInclude(entity, nameof(BaseModel.IsDeleted));
        }

        public IQueryable<Entity> Get(Expression<Func<Entity, bool>> predicate)
        {
            return GetAll().Where(predicate);
        }

        public IQueryable<Entity> GetAll()
        {
            return _dbSet.Where(e => e.IsDeleted == false);
        }

        public IQueryable<Entity> GetAllByPage(int PageNumber, int PageSize)
        {
            return GetAll()
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize);
        }

        public Entity GetById(int id)
        {
            return Get(e => e.Id == id).FirstOrDefault()!;
        }

        public IQueryable<Entity> GetByPage(Expression<Func<Entity, bool>> predicate, int PageNumber, int PageSize)
        {
            return Get(predicate)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void SaveInclude(Entity entity, params string[] properties)
        {
            var local = _dbSet.Local.FirstOrDefault(x => x.Id == entity.Id);

            EntityEntry entry = null!;

            if (local is null)
            {
                entry = _context.Entry(entity);
            }
            else
            {
                entry = _context.ChangeTracker.Entries<Entity>()
                    .FirstOrDefault(x => x.Entity.Id == entity.Id)!;
            }

            foreach (var property in entry.Properties)
            {
                if (properties.Contains(property.Metadata.Name))
                {
                    property.CurrentValue = entity.GetType().GetProperty(property.Metadata.Name)!.GetValue(entity);
                    property.IsModified = true;
                }
            }
        }

        public void Update(Entity entity)
        {
            _dbSet.Update(entity);
        }
    }
}