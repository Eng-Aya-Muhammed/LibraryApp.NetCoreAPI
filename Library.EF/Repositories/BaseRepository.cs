using Library.core;
using Library.core.Models;
using Microsoft.EntityFrameworkCore;
using repository.core.Const;
using repository.core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace repository.EF.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected ApplicationDbContext _context;
        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public T GetById(Expression<Func<T, bool>> criteria, string? include = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if(include != null)
            {
                query = query.Include(include);
                            }
            return query.SingleOrDefault(criteria);
        }

       
        
        public IEnumerable<T> Search(Expression<Func<T, bool>> criteria , string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if(includes != null)
            {
                foreach(var include in includes)
                {
                    query=query.Include(include);
                }
            }
            return query.Where(criteria);
        }
        public IEnumerable<T> Filter(Expression<Func<T, bool>> criteria, string[] includes = null, Expression<Func<T, object>> orderBy = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            query = query.Where(criteria);
            if (orderBy != null)
            {
                query = query.OrderBy(orderBy);
            }

            return query;
        }
        public IEnumerable<T> Sort(Expression<Func<T, object>> orderBy = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (orderBy != null)
            {
                query = query.OrderBy(orderBy);
            }
            return query;
        }



        public T Add(T entity)
        {
             _context.Set<T>().Add(entity);
            return  entity;
        }
        
        public T Update(T entity)
        {
            _context.Update(entity); 
            return entity;
        }


        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }



        public IEnumerable<T> GetAll()
        {
            var books = _context.Set<T>().ToList();
            return books;
        }

        
    }
}
