namespace net_il_mio_fotoalbum.Database
{
    public interface IRepository<T>
    {
        T? GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllFiltered(Func<T, bool> filter);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

    }
}
