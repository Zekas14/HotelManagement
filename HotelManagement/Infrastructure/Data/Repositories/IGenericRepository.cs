using HotelManagement.Domain.Models;
using System.Linq.Expressions;

namespace HotelManagement.Infrastructure.Data.Repositories
{
    public interface IGenericRepository<Entity> where Entity : BaseModel
    {
        IQueryable<Entity> GetAll();
        IQueryable<Entity> GetAllWithDeleted();
        IQueryable<Entity> GetDeleted();
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
}