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

        public UsersController(CodeChallengeContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

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

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            User user = await _context.Users.FirstAsync(x => x.Username == request.Username);

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

        [HttpPost("addFavoriteShow"), Authorize]
        public async Task<ActionResult<TVShow>> AddTVShowToFavorites(int tvShowId)
        {
            string loggedUserName = User?.Identity?.Name;
            User user = await _context.Users.FirstAsync(x => x.Username == loggedUserName);
            var tVShow = await _context.TVShows
                .Include(x => x.Actors)
                .Include(x=> x.Episodes)
                .FirstAsync(x => x.TVShowId == tvShowId);

            if(tVShow == null)
            {
                return BadRequest("TVShow does not exist");
            }

            user.Favorites.Add(tVShow);
            await _context.SaveChangesAsync();
            return Ok(tVShow);
        }

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

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }


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
