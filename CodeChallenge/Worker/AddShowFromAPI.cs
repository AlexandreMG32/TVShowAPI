using Azure.Core;
using CodeChallenge.Controllers;
using CodeChallenge.Data;
using CodeChallenge.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using RestSharp;
using System;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Web.Mvc;

namespace CodeChallenge.Worker
{
    public class AddShowFromAPI
    { 
        static HttpClient client = new HttpClient();
        static string path = "https://www.episodate.com/api/show-details?q=";
        static int tVShowId = 79290;

        public static async Task GetTVShowAsync()
        {
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
            catch(ArgumentException e) { Console.WriteLine(e.Message); }
        }

        private static async void GetFieldsForTVShowModel(JObject tvShow)
        {
            try
            {
                string tvShowName = tvShow["tvShow"]["name"].Value<string>().ToString();
                string tvShowDescription = tvShow["tvShow"]["description"].Value<string>().ToString();
                string tvShowGenre = "";
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
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        private static async Task AddShowToDatabase(TVShow tVShow)
        {
            if (client.BaseAddress == null)
            {
                client.BaseAddress = new Uri("https://localhost:7140/api/TVShows");
            }
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.PostAsJsonAsync(client.BaseAddress, tVShow);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(response.Content.ToString());
        }
    }
}
