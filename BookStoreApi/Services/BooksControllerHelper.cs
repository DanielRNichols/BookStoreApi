using AutoMapper;
using BookStoreApi.Contracts;
using BookStoreApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Services
{
    public class BooksControllerHelper : DbResourceControllerHelper<Book>, IBooksControllerHelper
    {
        public BooksControllerHelper(ILoggerService logger, IMapper mapper) : base(logger, mapper)
        {

        }
    }
}
