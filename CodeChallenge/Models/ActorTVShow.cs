namespace CodeChallenge.Models
{
    public class ActorTVShow
    {
        public int ActorId { get; set; }

        public Actor Actor { get; set; }

        public int TVShowId { get; set; }

        public TVShow TVShow { get; set; }
    }
}
