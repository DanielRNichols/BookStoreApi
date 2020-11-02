using AutoMapper;
using BookStoreApi.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Services
{
    public class DbResourceControllerHelper<T> : IDbResourceControllerHelper<T> where T : class, IDbResource
    {
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public DbResourceControllerHelper(ILoggerService logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ObjectResult> GetItems<D>(ControllerBase controller, IDbResourceRepository<T> repo)
        {
            try
            {
                string desc = GetControllerDescription(controller);
                _logger.LogInfo(desc);
                var items = await repo.FindAll();
                var response = _mapper.Map<IList<D>>(items);
                _logger.LogInfo($"{desc} - Successful");

                return controller.Ok(response);
            }
            catch (Exception e)
            {
                return InternalError(controller, e);
            }
        }

        public async Task<IActionResult> GetItem<D>(ControllerBase controller, IDbResourceRepository<T> repo, int id)
        {
            try
            {
                string desc = GetControllerDescription(controller);
                _logger.LogInfo($"{desc}: {id}");
                var item = await repo.FindById(id);
                if (item == null)
                {
                    _logger.LogWarn($"{desc}: Item not found: {id}");
                    return controller.NotFound();
                }
                var response = _mapper.Map<D>(item);
                _logger.LogInfo($"{desc} - Successful");
                return controller.Ok(response);
            }
            catch (Exception e)
            {
                return InternalError(controller, $"Server Error: {e.Message} - {e.InnerException}");
            }
        }

        public async Task<IActionResult> Create<D>(ControllerBase controller, 
            IDbResourceRepository<T> repo, D itemDTO)
        {
            string desc = GetControllerDescription(controller);
            try
            {
                _logger.LogInfo(desc);
                if (itemDTO == null)
                {
                    _logger.LogWarn($"{desc}: Empty request submitted");
                    return controller.BadRequest();
                }
                if (!controller.ModelState.IsValid)
                {
                    _logger.LogWarn($"{desc}: Invalid request submitted");
                    return controller.BadRequest(controller.ModelState);
                }
                var item = _mapper.Map<T>(itemDTO);

                var isSuccess = await repo.Create(item);
                if (!isSuccess)
                {
                    return InternalError(controller, $"{desc}: Item creation failed");
                }
                _logger.LogInfo($"{desc}: Item created");
                return controller.Created("", new { item });
            }
            catch (Exception e)
            {
                return InternalError(controller, e);
            }
        }

        public async Task<IActionResult> Update<D>(ControllerBase controller, 
            IDbResourceRepository<T> repo, int id, D itemDTO)
        {
            try
            {
                string desc = GetControllerDescription(controller);
                _logger.LogInfo(desc);
                if (id < 1 || itemDTO == null)
                {
                    _logger.LogWarn($"{desc}: Empty request submitted");
                    return controller.BadRequest();
                }
                var exists = await repo.Exists(id);
                if (!exists)
                {
                    _logger.LogWarn($"{desc}: Item with id {id} was not found");
                    return controller.NotFound();
                }
                if (!controller.ModelState.IsValid)
                {
                    _logger.LogWarn($"{desc}: Invalid request submitted");
                    return controller.BadRequest(controller.ModelState);
                }

                var item = _mapper.Map<T>(itemDTO);
                // force item.Id to be id passed in 
                item.Id = id;

                var isSuccess = await repo.Update(item);
                if (!isSuccess)
                {
                    return InternalError(controller, "{desc}: Update failed");
                }
                _logger.LogInfo("{desc}: Update Successful");
                return controller.Ok(new { item });
            }
            catch (Exception e)
            {

                return InternalError(controller, $"Server Error: {e.Message} - {e.InnerException}");
            }
        }

        public async Task<IActionResult> Delete(ControllerBase controller,
            IDbResourceRepository<T> repo, int id)
        {
            try
            {
                string desc = GetControllerDescription(controller);
                _logger.LogInfo(desc);
                if (id < 1)
                {
                    _logger.LogWarn($"{desc}: Empty request submitted");
                    return controller.BadRequest();
                }
                var item = await repo.FindById(id);
                if (item == null)
                {
                    _logger.LogWarn($"{desc}: Item with id ${id} was not found");
                    return controller.NotFound();
                }

                var isSuccess = await repo.Delete(item);
                if (!isSuccess)
                {
                    return InternalError(controller, "{desc}: Item delete failed");
                }
                _logger.LogInfo("{desc}: Item deleted");
                return controller.Ok(id);
            }
            catch (Exception e)
            {

                return InternalError(controller, e);
            }
        }

        private string GetControllerDescription(ControllerBase controller)
        {
            return $"{controller.ControllerContext.ActionDescriptor.ControllerName} - " +
                $"{controller.ControllerContext.ActionDescriptor.ActionName}";
        }

        private ObjectResult InternalError(ControllerBase controller, string msg)
        {
            _logger.LogServerError(msg);
            return controller.StatusCode(500, "Server Error");
        }
        private ObjectResult InternalError(ControllerBase controller, Exception e)
        {
            _logger.LogServerError(e);
            return controller.StatusCode(500, "Server Error");
        }

    }
}
