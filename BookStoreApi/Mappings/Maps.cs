using AutoMapper;
using BookStoreApi.Data;
using BookStoreApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Mappings
{
    public class Maps : Profile
    {
        public Maps ()
        {
            CreateMap<Author, AuthorDTO>().ReverseMap();
            CreateMap<Book, BookDTO>().ReverseMap();
        }
    }
}
