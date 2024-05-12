using Library.core.Models;
using repository.core.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace repository.core.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        T GetById(Expression<Func<T, bool>> criteria, string? include = null);
        IEnumerable<T> GetAll();

        IEnumerable<T> Search(Expression<Func<T, bool>> criteria, string[] includes=null);
        IEnumerable<T> Filter(Expression<Func<T, bool>> criteria, string[] includes= null
            , Expression<Func<T, object>> orderBy = null);
        IEnumerable<T> Sort(Expression<Func<T, object>> orderBy = null);
        T Add(T entity);
        T Update(T entity);
        void Delete(T entity);
        

    }
}
