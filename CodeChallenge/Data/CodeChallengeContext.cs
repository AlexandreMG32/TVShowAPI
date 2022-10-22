using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Models;
using NuGet.Packaging;

namespace CodeChallenge.Data
{
    public class CodeChallengeContext : DbContext
    {
        public CodeChallengeContext (DbContextOptions<CodeChallengeContext> options)
            : base(options)
        { }

        public DbSet<CodeChallenge.Models.TVShow> TVShows { get; set; } = default!;

        public DbSet<CodeChallenge.Models.Actor> Actors { get; set; }

        public DbSet<CodeChallenge.Models.Episode> Episodes { get; set; }

        public DbSet<CodeChallenge.Models.ActorTVShow> ActorTVShows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List<Actor> actors = new List<Actor>();
            actors.Add(new Actor
            {
                ActorId = 1,
                Name = "Jennifer Anniston",
                BirthDate = DateTime.Parse("02/14/1965")
            });
            actors.Add(new Actor
            {
                ActorId = 2,
                Name = "Courtney Cox",
                BirthDate = DateTime.Parse("06/17/1968")
            });

            List<Episode> episodes = new List<Episode>();
            episodes.Add(new Episode
            {
                EpisodeId = 1,
                Duration = 21.5,
                Title = "Pilot",
                Description = "Test",
                TVShowId = 1
            });
            episodes.Add(new Episode
            {
                EpisodeId = 2,
                Duration = 23.2,
                Title = "Pilot",
                Description = "Test",
                TVShowId = 2
            });

            modelBuilder.Entity<TVShow>().HasData(
                new TVShow
                {
                    TVShowId = 1,
                    Title = "Friends",
                    ReleaseDate = DateTime.Parse("01/01/1990"),
                    Description = "A comedy show",
                    Genre = "Comedy"
                },
                new TVShow
                {
                    TVShowId = 2,
                    Title = "The Sopranos",
                    ReleaseDate = DateTime.Parse("02/01/1984"),
                    Description = "A small TV show",
                    Genre = "Romance"
                });

            modelBuilder.Entity<Actor>().HasData(
                actors
            );

            modelBuilder.Entity<Episode>().HasData(
                episodes
            );

            modelBuilder.Entity<Actor>()
                .HasMany(left => left.TVShows)
                .WithMany(right => right.Actors)
                .UsingEntity<ActorTVShow>(
                    right => right.HasOne(e => e.TVShow).WithMany(),
                    left => left.HasOne(e => e.Actor).WithMany().HasForeignKey(e => e.ActorId),
                    join => join.ToTable("ActorTVShows")
                );

            modelBuilder.Entity<ActorTVShow>().HasData(
                new ActorTVShow() { ActorId = 1, TVShowId = 1 },
                new ActorTVShow() { ActorId = 2, TVShowId = 1 }
            );






        }

    }

    
}
