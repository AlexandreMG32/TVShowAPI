using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;
using CodeChallenge.Models;
using Microsoft.AspNetCore.Authorization;

namespace CodeChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TVShowsController : ControllerBase
    {
        private readonly CodeChallengeContext _context;

        public TVShowsController(CodeChallengeContext context)
        {
            _context = context;
        }

        // GET: api/TVShows
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TVShow>>> GetTVShow()
        {
            return await _context.TVShows.Include(x => x.Episodes).Include(x => x.Actors).ToListAsync();
        }

        // GET: api/TVShows/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TVShow>> GetTVShow(int id)
        {
            var tVShow = await _context.TVShows
                .Include(x => x.Actors)
                .Include(x => x.Episodes)
                .FirstAsync(x => x.TVShowId == id);

            if (tVShow == null)
            {
                return NotFound();
            }

            return tVShow;
        }

        [HttpGet("orderby/date")]
        public async Task<ActionResult<IEnumerable<TVShow>>> GetTVShowsOrderByReleaseDate()
        {
            return await _context.TVShows.Include(x => x.Episodes).Include(x => x.Actors).OrderBy(x => x.ReleaseDate).ToListAsync();
        }

        [HttpGet("orderby/title")]
        public async Task<ActionResult<IEnumerable<TVShow>>> GetTVShowsOrderByTitle()
        {
            return await _context.TVShows.Include(x => x.Episodes).Include(x => x.Actors).OrderBy(x => x.Title).ToListAsync();
        }

        [HttpGet("orderby/genre")]
        public async Task<ActionResult<IEnumerable<TVShow>>> GetTVShowsOrderByGenre()
        {
            return await _context.TVShows.Include(x => x.Episodes).Include(x => x.Actors).OrderBy(x => x.Genre).ToListAsync();
        }

        [HttpGet("genre/{genre}")]
        public async Task<ActionResult<IEnumerable<TVShow>>> GetTVShowsWithGenre(string genre)
        {
            try
            {
                return await _context.TVShows.Include(x => x.Episodes).Include(x => x.Actors).Where(x => x.Genre.ToLower().Equals(genre)).ToListAsync();
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }

        // POST: api/TVShows
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TVShow>> PostTVShow(TVShow tVShow)
        {
            _context.TVShows.Add(tVShow);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTVShow", new { id = tVShow.TVShowId }, tVShow);
        }

        // DELETE: api/TVShows/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTVShow(int id)
        {
            var tVShow = await _context.TVShows.FindAsync(id);
            if (tVShow == null)
            {
                return NotFound();
            }

            _context.TVShows.Remove(tVShow);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TVShowExists(int id)
        {
            return _context.TVShows.Any(e => e.TVShowId == id);
        }
    }
}
