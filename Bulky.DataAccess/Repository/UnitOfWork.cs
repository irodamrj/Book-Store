using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; }

        public IProductRepository Product { get; private set; }

        private readonly AppDbContext _appDbContext;
        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            Category = new CategoryRepository(_appDbContext);
            Product = new ProductRepository(_appDbContext);

        }

        public void Save()
        {
            _appDbContext.SaveChanges();
        }
    }
}
