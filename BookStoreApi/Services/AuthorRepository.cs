using BookStoreApi.Contracts;
using BookStoreApi.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Services
{
    public class AuthorRepository : RepositoryBase<Author>, IAuthorRepository
    {
        private readonly ApplicationDbContext _db;

        // db created via dependency injection
        public AuthorRepository(ApplicationDbContext db) : base(db, db.Authors)
        {
            _db = db;
        }

        public override async Task<IList<Author>> FindAll()
        {
            // Using Include gets Books array added to each author
            var authors = await _db.Authors.Include(a => a.Books).ToListAsync();

            return authors;
        }

        public override async Task<Author> FindById(int id)
        {
            // Using Include gets Books array added to author
            var author = await _db.Authors.Include(a => a.Books).FirstOrDefaultAsync(q => q.Id == id);

            return author;
        }

    }
}
