using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CodeChallenge.Models
{
    [Index(nameof(User.Username), IsUnique = true)]
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        [JsonIgnore]
        public byte[] PasswordHash { get; set; }
        [JsonIgnore]
        public byte[] PasswordSalt { get; set; }
        public virtual ICollection<TVShow> Favorites { get; set; } = new List<TVShow>();
    }
}
