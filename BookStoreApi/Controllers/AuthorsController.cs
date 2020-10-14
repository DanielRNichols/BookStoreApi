using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStoreApi.Contracts;
using BookStoreApi.Data;
using BookStoreApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers
{
    /// <summary>
    /// Url to interact with Authors
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
                return InternalError(e);
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

        /// <summary>
        /// Create an Author
        /// </summary>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
        {
            try
            {
                _logger.LogInfo("Attempted to create Author");
                if(authorDTO == null)
                {
                    _logger.LogWarn($"Empty request submitted");
                    return BadRequest();
                }
                if(!ModelState.IsValid)
                {
                    _logger.LogWarn($"Invalid request submitted");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorDTO);

                var isSuccess = await _authorRepository.Create(author);
                if(!isSuccess)
                {
                    return InternalError("Author create failed");
                }
                _logger.LogInfo("Author created");
                return Created("", new { author });
            }
            catch (Exception e)
            {
                return InternalError(e);
            }
        }

        /// <summary>
        /// Update Author
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Adminstrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            try
            {
                _logger.LogInfo("Attempted to update Author");
                if(id < 1 || authorDTO == null)
                {
                    _logger.LogWarn($"Empty request submitted");
                    return BadRequest();
                }
                var exists = await _authorRepository.Exists(id);
                if(!exists)
                {
                    _logger.LogWarn($"Author with id {id} was not found");
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Invalid request submitted");
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(authorDTO);
                // force author.Id to be id passed in 
                author.Id = id; 

                var isSuccess = await _authorRepository.Update(author);
                if(!isSuccess)
                {
                    return InternalError("Author update failed");
                }
                _logger.LogInfo("Author updated");
                return Ok(new { author });
            }
            catch (Exception e)
            {

                return InternalError($"Server Error: {e.Message} - {e.InnerException}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInfo("Attempted to delete Author");
                if (id < 1)
                {
                    _logger.LogWarn($"Empty request submitted");
                    return BadRequest();
                }
                var author = await _authorRepository.FindById(id);
                if(author == null)
                {
                    _logger.LogWarn($"Author with id ${id} was not found");
                    return NotFound();
                }

                var isSuccess = await _authorRepository.Delete(author);
                if (!isSuccess)
                {
                    return InternalError("Author delete failed");
                }
                _logger.LogInfo("Author deleted");
                return Ok(id);
            }
            catch (Exception e)
            {

                return InternalError(e);
            }
        }

        private ObjectResult InternalError(string msg)
        {
            _logger.LogServerError(msg);
            return StatusCode(500, "Server Error");
        }
        private ObjectResult InternalError(Exception e)
        {
            _logger.LogServerError(e);
            return StatusCode(500, "Server Error");
        }
    }
}
