using System.Linq;

namespace SQLModel
{
    public interface ISQLRepository<T>
    {
        IQueryable<T> GetByWhere(string where, object[] paramenter, string column = "*");
        IQueryable<T> GetAll(string column = "*",string orderby = "");
    }
}
