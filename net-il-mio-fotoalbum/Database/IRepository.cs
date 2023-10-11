namespace net_il_mio_fotoalbum.Database
{
    public interface IRepository<T>
    {
        T? GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Update(T originalEntity, T modifiedEntity);
        void Delete(T entity);

    }
}
