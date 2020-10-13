using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStoreApi.Contracts;
using BookStoreApi.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers
{
    /// <summary>
    /// Url to interact with Authors
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        // authorRepository, logger are dependency injected
        public AuthorsController(IAuthorRepository authorRepository, ILoggerService logger, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all authors
        /// </summary>
        /// <returns>List of authors</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                _logger.LogInfo("Get All Authors");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo("Successfully retrieved Authors");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"Server Error: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get Author by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                _logger.LogInfo($"Get Author: {id}");
                var author = await _authorRepository.FindById(id);
                if(author == null)
                {
                    _logger.LogWarn($"Author not found: {id}");
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"Successfully retrieved Author: {author.FirstName} {author.LastName}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"Server Error: {e.Message} - {e.InnerException}");
            }
        }


        private ObjectResult InternalError(string msg)
        {
            _logger.LogError($"Server Error: {msg}");
            return StatusCode(500, "Server Error");
        }
    }
}
