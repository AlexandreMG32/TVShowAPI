using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CodeChallenge.Models
{
    public class TVShow
    {
        [Key]
        public int TVShowId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public string Genre { get; set; }

        public string? Description { get; set; }

        public virtual ICollection<Actor> Actors { get; set; } = new List<Actor>();

        public virtual ICollection<Episode> Episodes { get; set; } = new List<Episode>();
    }
}
