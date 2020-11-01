using BookStoreApi.Contracts;
using BookStoreApi.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Services
{
    public class BookRepository : DbResourceRepository<Book>, IBookRepository
    {
        private readonly ApplicationDbContext _db;

        public BookRepository(ApplicationDbContext db) : base(db, db.Books)
        {
            _db = db;
        }

        public override async Task<IList<Book>> FindAll()
        {
            var books = await _db.Books.Include(b => b.Author).ToListAsync();

            return books;
        }

        public override async Task<Book> FindById(int id)
        {
            //var book = await _db.Books.FindAsync(id);
            var book = await _db.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id);

            return book;
        }
    }
}
