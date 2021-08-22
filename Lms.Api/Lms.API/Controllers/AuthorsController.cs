using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Lms.API.Core.Dto;
using Lms.API.Core.Entities;
using Lms.API.Core.Repositories;
using Lms.API.Data.Data;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace Lms.API.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly LmsAPIContext db;

        private readonly IMapper mapper;

        private readonly IUoW uow;

        public AuthorsController(LmsAPIContext db, IMapper mapper, IUoW uow)
        {
            this.db = db;
            this.mapper = mapper;
            this.uow = uow;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            var authors = await uow.AuthorRepository.GetAllAuthorsAsync();
            var authorsDto = mapper.Map<AuthorDto[]>(authors);

            var response = JsonConvert.SerializeObject(authorsDto);
            return Ok(response);
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            var authorDto = mapper.Map<AuthorDto>(await uow.AuthorRepository.GetAuthorAsync(id));

            if (authorDto == null)
            {
                return NotFound();
            }

            return Ok(authorDto);
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthorsByName(string name)
        {
            var authorsDto = mapper.Map<AuthorDto[]>(await uow.AuthorRepository.GetAuthorByNameAsync(name));

            if (authorsDto == null)
            {
                return NotFound();
            }

            return Ok(authorsDto);
        }

        // PUT: api/Authors/5 To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, Author author)
        {
            if (id != author.Id)
            {
                return BadRequest();
            }

            db.Entry(author).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Authors To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Author>> PostAuthor(Author author)
        {
            await uow.AuthorRepository.AddAsync(author);
            await uow.AuthorRepository.SaveAsync();

            return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await db.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            uow.AuthorRepository.Remove(author);
            await uow.AuthorRepository.SaveAsync();

            return NoContent();
        }

        private bool AuthorExists(int id)
        {
            return db.Authors.Any(e => e.Id == id);
        }
    }
}