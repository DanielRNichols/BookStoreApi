using BookStoreApi.Contracts;
using BookStoreApi.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Services
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext _db;

        // db created via dependency injection
        public AuthorRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(Author entity)
        {
            await _db.Authors.AddAsync(entity);

            return await Save();
        }

        public async Task<bool> Delete(Author entity)
        {
            _db.Authors.Remove(entity);

            return await Save();
        }

        public async Task<IList<Author>> FindAll()
        {
            // Using Include gets Books array added to each author
            var authors = await _db.Authors.Include(a => a.Books).ToListAsync();

            return authors;
        }

        public async Task<Author> FindById(int id)
        {
            //var author = await _db.Authors.FindAsync(id);
            // Using Include gets Books array added to author
            var author = await _db.Authors.Include(a => a.Books).FirstOrDefaultAsync(q => q.Id == id);

            return author;
        }

        public async Task<bool> Exists(int id)
        {
            return await _db.Authors.AnyAsync(row => row.Id == id);
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();

            return changes > 0;
        }

        public async Task<bool> Update(Author entity)
        {
            _db.Authors.Update(entity);

            return await Save();
        }
    }
}
