using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly AppDbContext _appDbContext;
		internal DbSet<T> dbSet;
		public Repository(AppDbContext appDbContext)
		{
			_appDbContext = appDbContext;
			//_appDbContext.Category == dbSet
			this.dbSet = _appDbContext.Set<T>();
			_appDbContext.Products.Include(u => u.Category);
		}


		public void Add(T entity)
		{
			dbSet.Add(entity);
		}

		public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
		{
			IQueryable<T> query = dbSet;
			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var item in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(item);
				}
			}
			query = query.Where(filter);
			return query.FirstOrDefault();
		}

		public IEnumerable<T> GetAll(string? includeProperties = null)
		{
			IQueryable<T> query = dbSet;
			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var item in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(item);
				}
			}
			return query.ToList();
		}

		public void Remove(T entity)
		{
			dbSet.Remove(entity);
		}

		public void RemoveRange(IEnumerable<T> entity)
		{
			dbSet.RemoveRange(entity);
		}
	}
}
