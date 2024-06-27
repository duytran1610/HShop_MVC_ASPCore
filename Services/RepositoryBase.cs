using HShop.Models;
using System.Linq.Expressions;

namespace HShop.Services
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected HshopContext ctx { get; set; }
        public RepositoryBase(HshopContext context)
        {
            ctx = context;
        }
        public IQueryable<T> FindAll() => ctx.Set<T>();
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) =>
            ctx.Set<T>().Where(expression);
        public void Create(T entity) => ctx.Set<T>().Add(entity);
        public void Update(T entity) => ctx.Set<T>().Update(entity);
        public void Delete(T entity) => ctx.Set<T>().Remove(entity);
    }
}
