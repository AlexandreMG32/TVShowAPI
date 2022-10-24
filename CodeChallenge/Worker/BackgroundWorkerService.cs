using CodeChallenge.Models;
using Newtonsoft.Json.Linq;
using NuGet.ProjectModel;
using System.Net.Http.Headers;

namespace CodeChallenge.Worker
{
    public class BackgroundWorkerService : BackgroundService
    {

        readonly ILogger<BackgroundWorkerService> _logger;
        readonly IHttpClientFactory _httpClientFactory;
        string path = "https://www.episodate.com/api/show-details?q=";
        static int tVShowId = 79292;

        public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running");
                await GetTVShowAsync();
                await Task.Delay(10000, stoppingToken);
            }
        }

        private async Task GetTVShowAsync()
        {
            var client = _httpClientFactory.CreateClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync(path + "" + tVShowId);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    tVShowId++;
                    GetFieldsForTVShowModel(JObject.Parse(jsonResponse));
                }
            }
            catch (ArgumentException e) { Console.WriteLine(e.Message); }
        }

        private async void GetFieldsForTVShowModel(JObject tvShow)
        {
            try
            {
                string tvShowName = tvShow["tvShow"]["name"].Value<string>().ToString();
                string tvShowDescription = tvShow["tvShow"]["description"].Value<string>().ToString();
                string tvShowGenre = "Unknown";
                string tvShowReleaseDate = tvShow["tvShow"]["start_date"].Value<string>().ToString();
                try
                {
                    tvShowGenre = tvShow["tvShow"]["genres"][0].Value<string>().ToString();
                }
                catch (ArgumentOutOfRangeException e) { }

                await AddShowToDatabase(new TVShow
                {
                    Title = tvShowName,
                    Description = tvShowDescription,
                    ReleaseDate = DateTime.Parse(tvShowReleaseDate),
                    Genre = tvShowGenre
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private async Task AddShowToDatabase(TVShow tVShow)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                if (client.BaseAddress == null)
                {
                    client.BaseAddress = new Uri("https://localhost:7140");
                }
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                CancellationTokenSource token = new CancellationTokenSource(2000);
                var response = await client.PostAsJsonAsync("api/TVShows", tVShow, token.Token);
                response.EnsureSuccessStatusCode();
                Console.WriteLine(response.Content.ToString());
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
            }
            
        }
    }
}
