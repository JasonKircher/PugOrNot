using Newtonsoft.Json;
using RestSharp;

namespace DefaultNamespace;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using DotNetEnv;

public class WarcraftLogsAccess
{
    private static string clientId;
    private static string clientSecret;
	private string token;
    private static readonly string tokenUrl = "https://www.warcraftlogs.com/oauth/token";
    private static readonly string graphqlUrl = "https://www.warcraftlogs.com/api/v2/client";
    
    public WarcraftLogsAccess()
    {
        Env.Load();
        clientId = Environment.GetEnvironmentVariable("warcraftlogs_client_id");
        clientSecret = Environment.GetEnvironmentVariable("warcraftlogs_client_secret");
        
    }

    async public Task<string> GetAccessToken()
    {
        using var client = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
        var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
        request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        var tokenObj = JObject.Parse(responseBody);
        return tokenObj["access_token"]?.ToString();
    }
	async public void MakeRequest(string token, string query)
	{
        var client = new RestClient(graphqlUrl);
        var request = new RestRequest("", Method.Post);
        request.AddHeader("Authorization", $"Bearer {token}");
        request.AddHeader("Content-Type", "application/json");
        request.AddParameter("application/json", query, ParameterType.RequestBody);
        Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(client.Execute(request).Content), Formatting.Indented));
	}
    public void GetEncounterStats(string token, int encounterId, string charName, string region, string server, string metric)
    {
        var query = string.Format(
            "{{\"query\":\"query {{\\n  characterData {{\\n    character(name: \\\"{0}\\\", serverSlug: \\\"{1}\\\", serverRegion: \\\"{2}\\\"){{encounterRankings(encounterID: {3}, metric: {4})}}}}}}\"}}",
            charName, server, region, encounterId, metric
        );
        MakeRequest(token, query);
    }
}