using Azure.Core;
using CodeChallenge.Data;
using CodeChallenge.DTO;
using CodeChallenge.Models;
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CodeChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly CodeChallengeContext _context;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for the user controller, getting the injected context
        /// of the entity framework and the configuration for later getting the 
        /// token from appsettings
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configuration"></param>
        public UsersController(CodeChallengeContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Registers an user given the user data transfer object
        /// and saves it in the database
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            User user = new User{ Username = request.Username, PasswordHash = passwordHash, PasswordSalt = passwordSalt };
            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(UniqueConstraintException e)
            {
                return BadRequest("Username already exists");
            }
            return Ok(user);
        }

        /// <summary>
        /// Logins a user, checking if it exists in the database and
        /// checking if the passwords match
        /// Generates a token for further having permissions 
        /// to add favorites to his profile
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Username == request.Username);

            if(user == null)
            {
                return BadRequest("User not found");
            }
            if(!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password");
            }
            string token = CreateToken(user);

            return Ok(token);
        }

        /// <summary>
        /// Adds a show to the list of favorite shows of the user
        /// given the tvShowId
        /// </summary>
        /// <param name="tvShowId"></param>
        /// <returns></returns>
        [HttpPost("addFavoriteShow"), Authorize]
        public async Task<ActionResult<TVShow>> AddTVShowToFavorites(int tvShowId)
        {
            string loggedUserName = User?.Identity?.Name;
            User user = await _context.Users.FirstAsync(x => x.Username == loggedUserName);
            var tVShow = await _context.TVShows
                .Include(x => x.Actors)
                .Include(x=> x.Episodes)
                .FirstOrDefaultAsync(x => x.TVShowId == tvShowId);

            if(tVShow == null)
            {
                return BadRequest("TVShow does not exist");
            }

            bool exists = user.Favorites.Contains(tVShow);

            if(exists)
            {
                return BadRequest("TVShow already in favorites");
            }
            user.Favorites.Add(tVShow);
            await _context.SaveChangesAsync();
            return Ok(tVShow);
        }
        
        /// <summary>
        /// Removes a tv show from the user list of 
        /// favorite shows, given the tv show id
        /// </summary>
        /// <param name="tvShowId"></param>
        /// <returns></returns>
        [HttpPost("removeFavoriteShow"), Authorize]
        public async Task<ActionResult<TVShow>> RemoveTVShowFromFavorites(int tvShowId)
        {
            string loggedUserName = User?.Identity?.Name;
            User user = await _context.Users.FirstAsync(x => x.Username == loggedUserName);
            var tVShow = await _context.TVShows
                .Include(x => x.Actors)
                .Include(x => x.Episodes)
                .FirstOrDefaultAsync(x => x.TVShowId == tvShowId);

            if (tVShow == null)
            {
                return BadRequest("TVShow does not exist");
            }

            bool exists = user.Favorites.Contains(tVShow);

            if(!exists)
            {
                return BadRequest("TVShow isnt on your favorites list");
            }
            user.Favorites.Remove(tVShow);
            await _context.SaveChangesAsync();
            return Ok(tVShow);
        }

        /// <summary>
        /// Gets the list of favorites shows of the logged in user
        /// </summary>
        /// <returns></returns>
        [HttpGet("favoriteShows"), Authorize]
        public async Task<ActionResult<IEnumerable<TVShow>>> GetFavoriteTVShows()
        {
            string loggedUserName = User?.Identity?.Name;
            User user = await _context.Users
                .Include(x => x.Favorites).ThenInclude(x => x.Actors)
                .Include(x => x.Favorites).ThenInclude(x => x.Episodes)
                .FirstAsync(x => x.Username == loggedUserName);

            return user.Favorites.ToList();
        }

        /// <summary>
        /// Creates a password hash and salt given the password passed 
        /// previously by the user
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        /// <summary>
        /// Checks if the password typed by the user 
        /// is the same as the one on the database
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        /// <returns></returns>
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        /// <summary>
        /// Creates a token that will be given to the user
        /// after the login, to give him permissions to 
        /// add and remove tvshows from his list of favorite ones
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
