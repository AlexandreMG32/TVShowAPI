using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    /// <summary>
    /// Controller to get, add and remove episodes from the DB
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodesController : ControllerBase
    {
        private readonly CodeChallengeContext _context;

        public EpisodesController(CodeChallengeContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all episodes in the db
        /// </summary>
        /// <returns>List with all the episodes in the db</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Episode>>> GetEpisode()
        {
            return await _context.Episodes.Include(x => x.TVShow).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">id of the episode to get</param>
        /// <returns>Episode with the id passed or null if 
        /// there isn't one with that id</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Episode>> GetEpisode(int id)
        {
            var episode = await _context.Episodes.FindAsync(id);

            if (episode == null)
            {
                return NotFound();
            }

            return episode;
        }

        /// <summary>
        /// Adds new episode to the db
        /// </summary>
        /// <param name="episode"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Episode>> PostEpisode(Episode episode)
        {
            _context.Episodes.Add(episode);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEpisode", new { id = episode.EpisodeId }, episode);
        }

        /// <summary>
        /// Deletes episode with the id passed 
        /// as param, from db 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEpisode(int id)
        {
            var episode = await _context.Episodes.FindAsync(id);
            if (episode == null)
            {
                return NotFound();
            }

            _context.Episodes.Remove(episode);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EpisodeExists(int id)
        {
            return _context.Episodes.Any(e => e.EpisodeId == id);
        }
    }
}
