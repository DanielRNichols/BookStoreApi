﻿using BookStoreApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Contracts
{
    public interface IBooksControllerHelper : IDbResourceControllerHelper<Book>
    {
    }
}


