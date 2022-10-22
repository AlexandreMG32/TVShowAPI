
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CodeChallenge.Models
{
    public class Episode
    {
        [Key]
        public int EpisodeId { get; set; }

        [Required]
        public String Title { get; set; }

        [Required]
        public string? Description { get; set; }

        public double Duration { get; set; }

        public int TVShowId { get; set; }

        [JsonIgnore]
        public TVShow TVShow { get; set; }

    }
}
