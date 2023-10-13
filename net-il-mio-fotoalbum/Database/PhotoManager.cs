using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Models;
using System.Linq.Expressions;

namespace net_il_mio_fotoalbum.Database
{
    public class PhotoManager : EntityRepository<Photo>
    {
        public PhotoManager(DbContext context) : base(context)
        {
        }

        public List<Photo> GetAllContaining(string? name, bool includeEverything = true)
        {
            List<Photo> foundPhotos = new List<Photo>();

            if (!string.IsNullOrEmpty(name))
            {
                if (includeEverything)
                    foundPhotos = _dbSet
                        .Where(photo => photo.Title!.ToLower().Contains(name.ToLower()))
                        .Include(photo => photo.Categories)
                        .ToList();
                else
                    foundPhotos = (List<Photo>)base.GetAllFiltered(photo => photo.Title!.ToLower().Contains(name.ToLower()));
            }

            return foundPhotos;
        }

        public Photo? GetById(int id, bool includeEverything = true)
        {
            if (includeEverything)
                return base._dbSet
                    .Include(photo => photo.Categories)
                    .Where(photo => photo.Id == id)
                    .FirstOrDefault();
            else
                return base.GetById(id);
        }

        public Photo? GetBySlug(string slug, bool includeEverything = true)
        {
            if (includeEverything)
                return base._dbSet
                    .Include(photo => photo.Categories)
                    .Where(photo => photo.Slug == slug)
                    .FirstOrDefault();
            else
                return base._dbSet.Where(photo => photo.Slug == slug).First();
        }

        public IEnumerable<Photo> GetAllFiltered(Expression<Func<Photo, bool>> filter, bool includeEverything = true)
        {
            if (includeEverything)
                return _dbSet
                    .Include(photo => photo.Categories)
                    .Where(filter)
                    .ToList();
            else
                return base.GetAllFiltered(filter);
        }

        public IEnumerable<Photo> GetAll(bool includeEverything = true)
        {
            if (includeEverything)
                return _dbSet
                    .Include(photo => photo.Categories)
                    .ToList();
            else
                return base.GetAll();
        }
    }
}

