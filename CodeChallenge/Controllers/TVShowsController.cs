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
    /// <summary>
    /// Controller for TV Shows
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TVShowsController : ControllerBase
    {
        private readonly CodeChallengeContext _context;

        public TVShowsController(CodeChallengeContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Includes Episodes and Actors, the navigation fields of the TVShow model
        /// </summary>
        /// <returns>List with all the shows in the DB</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TVShow>>> GetTVShow()
        {
            return await _context.TVShows.Include(x => x.Episodes).Include(x => x.Actors).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>TVShow with the specified id</returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns>List with all tv shows ordered by Release Date</returns>
        [HttpGet("orderby/date")]
        public async Task<ActionResult<IEnumerable<TVShow>>> GetTVShowsOrderByReleaseDate()
        {
            return await _context.TVShows.Include(x => x.Episodes).Include(x => x.Actors).OrderBy(x => x.ReleaseDate).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>List with all shows ordered by Title</returns>
        [HttpGet("orderby/title")]
        public async Task<ActionResult<IEnumerable<TVShow>>> GetTVShowsOrderByTitle()
        {
            return await _context.TVShows.Include(x => x.Episodes).Include(x => x.Actors).OrderBy(x => x.Title).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>All shows ordered by Genre</returns>
        [HttpGet("orderby/genre")]
        public async Task<ActionResult<IEnumerable<TVShow>>> GetTVShowsOrderByGenre()
        {
            return await _context.TVShows.Include(x => x.Episodes).Include(x => x.Actors).OrderBy(x => x.Genre).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="genre"></param>
        /// <returns>List with the genre passed as param by the user</returns>
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

        /// <summary>
        /// Adds a new tvShow, passed by the user, to the DB
        /// </summary>
        /// <param name="tVShow"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<TVShow>> PostTVShow(TVShow tVShow)
        {
            _context.TVShows.Add(tVShow);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTVShow", new { id = tVShow.TVShowId }, tVShow);
        }

        /// <summary>
        /// Deletes the tv show with the id passed as param by the user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
