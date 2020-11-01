using BookStoreApi.Contracts;
using BookStoreApi.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Services
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T: class, IDbResource
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<T> _dbContext;
        public RepositoryBase(ApplicationDbContext db, DbSet<T> dbContext)
        {
            _db = db;
            _dbContext = dbContext;
        }

        public virtual async Task<bool> Create(T entity)
        {
            await _dbContext.AddAsync(entity);

            return await Save();
        }

        public virtual async Task<bool> Delete(T entity)
        {
            _dbContext.Remove(entity);

            return await Save();
        }

        public virtual async Task<bool> Exists(int id)
        {
            return await _dbContext.AnyAsync(row => row.Id == id);
        }

        public virtual async Task<IList<T>> FindAll()
        {
            // Override if you need to include related objects
            var items = await _dbContext.ToListAsync();

            return items;
        }

        public virtual async Task<T> FindById(int id)
        {
            // Override if you need to include related objects
            var item = await _dbContext.FindAsync(id);

            return item;
        }

        public virtual async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();

            return changes > 0;
        }

        public virtual async Task<bool> Update(T entity)
        {
            _dbContext.Update(entity);

            return await Save();
        }
    }
}
