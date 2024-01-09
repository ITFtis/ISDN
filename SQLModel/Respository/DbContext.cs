
using System;
using System.ComponentModel;
namespace SQLModel
{
    public class DbContext : IDisposable
    {
        protected string _dbname = "";
        public DbContext(string dbname)
        {_dbname = dbname;}
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() {return base.ToString();}
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) {return base.Equals(obj); }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() { return base.GetHashCode(); }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Type GetType() {return this.GetType();}
        public void Dispose(){}
        public virtual SQLRepository<TEntity> get<TEntity>() where TEntity : class {
            return new SQLRepository<TEntity>(_dbname);
        }
    }
}
