using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LetsEat.Models
{
    public class RecipeDAL
    {
        private readonly HttpClient _client;

        public RecipeDAL(HttpClient client)
        {
            _client = client;
        }

        public async Task<Rootobject> FindRecipesAsync(string query)
        {
            var response = await _client.GetAsync(query);
            string jsonData = await response.Content.ReadAsStringAsync();

            Rootobject ro = JsonSerializer.Deserialize<Rootobject>(jsonData);

            return ro;
        }

    }
}

