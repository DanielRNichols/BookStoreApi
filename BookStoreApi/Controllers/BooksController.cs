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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository bookRepository,
            ILoggerService logger,
            IMapper mapper)
        {
            _bookRepository = bookRepository;
            _logger = logger;
            _mapper = mapper;

        }

        /// <summary>
        /// Get Books
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            try
            {
                _logger.LogInfo("Get all books");
                var books = await _bookRepository.FindAll();
                var response = _mapper.Map<IList<BookDTO>>(books);
                _logger.LogInfo("Successfully retrieved Books");

                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError(e);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBookById(int id)
        {
            try
            {
                _logger.LogInfo($"Get book with id of {id}");
                var book = await _bookRepository.FindById(id);
                if(book == null)
                {
                    _logger.LogWarn($"Book with id of {id} was not found");
                    return NotFound();
                }
                var response = _mapper.Map<BookDTO>(book);
                _logger.LogInfo($"Successfully retrieved book with id of {id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError(e);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBook([FromBody] BookCreateDTO bookDTO)
        {
            try
            {
                _logger.LogInfo("Create Book");
                if(bookDTO == null)
                {
                    _logger.LogWarn($"Empty request submitted");
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Invalid request submitted");
                    return BadRequest(ModelState);
                }
                var book = _mapper.Map<Book>(bookDTO);
                bool isSuccess = await _bookRepository.Create(book);
                if (!isSuccess)
                {
                    return InternalError("Book create failed");
                }
                _logger.LogInfo("Book created");
                return Created("", new { book });

            }
            catch (Exception e)
            {
                return InternalError(e);
            }
        }

        /// <summary>
        /// Update Book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bookDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDTO bookDTO)
        {
            try
            {
                _logger.LogInfo("Attempted to update Book");
                if (id < 1 || bookDTO == null)
                {
                    _logger.LogWarn($"Empty request submitted");
                    return BadRequest();
                }
                var exists = await _bookRepository.Exists(id);
                if (!exists)
                {
                    _logger.LogWarn($"Book with id {id} was not found");
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Invalid request submitted");
                    return BadRequest(ModelState);
                }

                var book = _mapper.Map<Book>(bookDTO);
                // force book.Id to be id passed in 
                book.Id = id;

                var isSuccess = await _bookRepository.Update(book);
                if (!isSuccess)
                {
                    return InternalError("Book update failed");
                }
                _logger.LogInfo("Book updated");
                return Ok(new { book });
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
                _logger.LogInfo("Attempted to delete Book");
                if (id < 1)
                {
                    _logger.LogWarn($"Empty request submitted");
                    return BadRequest();
                }
                var book = await _bookRepository.FindById(id);
                if (book == null)
                {
                    _logger.LogWarn($"Book with id ${id} was not found");
                    return NotFound();
                }

                var isSuccess = await _bookRepository.Delete(book);
                if (!isSuccess)
                {
                    return InternalError("Book delete failed");
                }
                _logger.LogInfo("Book deleted");
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
