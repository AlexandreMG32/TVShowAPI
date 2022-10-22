using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CodeChallenge.Models
{
    public class Actor
    {

        [Key]
        public int ActorId { get; set; }     

        [Required]
        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        [JsonIgnore]
        public virtual ICollection<TVShow> TVShows { get; set; } = new List<TVShow>();
    }
}
