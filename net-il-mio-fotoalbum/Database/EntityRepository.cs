using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using NuGet.Versioning;

namespace net_il_mio_fotoalbum.Database
{
    public class EntityRepository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public EntityRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public T? GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<T> GetAllFiltered(Expression<Func<T, bool>> filter)
        {
            return _dbSet.Where(filter).ToList();
        }
        public IEnumerable<T> GetAllFiltered(Expression<Func<T, bool>> filter, Expression<Func<T, object>> includeQuery)
        {
            return _dbSet.Where(filter).Include(includeQuery).ToList();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }
        public IEnumerable<T> GetAll(Expression<Func<T, object>> includeQuery)
        {
            return _dbSet.Include(includeQuery).ToList();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

    }
}
