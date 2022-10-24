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
    [Route("api/[controller]")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly CodeChallengeContext _context;

        /// <summary>
        /// Context of the database, using the entity framework, injected 
        /// in the constructor
        /// </summary>
        /// <param name="context"></param>
        public ActorsController(CodeChallengeContext context)
        {
            _context = context;
        }

        // GET: api/Actors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actor>>> GetActor()
        {
            return await _context.Actors.Include(x => x.TVShows).ToListAsync();
        }

        /// <summary>
        /// Gets an actor given the id of it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Actor>> GetActor(int id)
        {
            var actor = await _context.Actors.FindAsync(id);

            if (actor == null)
            {
                return NotFound();
            }

            return actor;
        }

        /// <summary>
        /// Adds a new actor to the database, inputed from the user
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Actor>> PostActor(Actor actor)
        {
            _context.Actors.Add(actor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetActor", new { id = actor.ActorId }, actor);
        }

        /// <summary>
        /// Deletes user given the id of it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActor(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }

            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Adds an actor to a show, given the id of the actor
        /// and the show and uses the generated table for the relation
        /// to save it in the DB
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="tvShowId"></param>
        /// <returns></returns>
        [HttpPost("{actorId}/addToShow/{tvShowId}")]
        public async Task<ActionResult<Actor>> AddActorToShow(int actorId, int tvShowId)
        {

            var actor = await _context.Actors.FindAsync(actorId);
            var tvShow = await _context.TVShows.FindAsync(tvShowId);

            if(actor == null || tvShow == null)
            {
                return BadRequest();
            }

            ActorTVShow relation = new ActorTVShow
            {
                ActorId = actorId,
                TVShowId = tvShowId
            };
            _context.ActorTVShows.Add(relation);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetActor", new { id = actor.ActorId }, actor);
        }

        private bool ActorExists(int id)
        {
            return _context.Actors.Any(e => e.ActorId == id);
        }
    }
}
