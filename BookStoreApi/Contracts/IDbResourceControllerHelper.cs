using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Contracts
{
    public interface IDbResourceControllerHelper<T> where T : class, IDbResource
    {
        Task<ObjectResult> GetItems<D>(ControllerBase controller, IDbResourceRepository<T> repo);
        Task<IActionResult> GetItem<D>(ControllerBase controller, IDbResourceRepository<T> repo, int id);
        Task<IActionResult> Create<D>(ControllerBase controller, IDbResourceRepository<T> repo, D itemDTO);
        Task<IActionResult> Update<D>(ControllerBase controller, IDbResourceRepository<T> repo, int id, D itemDTO);
        Task<IActionResult> Delete(ControllerBase controller, IDbResourceRepository<T> repo, int id);
    }
}
