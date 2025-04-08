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
        Console.WriteLine(Environment.GetEnvironmentVariables());
        clientId = Environment.GetEnvironmentVariable("warcraftlogs_client_id");
        clientSecret = Environment.GetEnvironmentVariable("warcraftlogs_client_secret");
        Console.WriteLine(clientId);
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
	async public void MakeRequest(string token, Object query)
	{
 		using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var content = new StringContent(JObject.FromObject(query).ToString(), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(graphqlUrl, content);
        var result = await response.Content.ReadAsStringAsync();

        Console.WriteLine(result);
	}
    public void GetEncounterStats(string token, int encounterid, string charname, string region, string server, string metric)
    {
		var query = new
        {
            query = $@"
characterData {{
    character(name: ""{charname}"", serverSlug: ""{server}"", serverRegion: ""{region}"") {{
        encounterRankings(encounterID: {encounterid}, metric: ""{metric}"")
    }}
}}"
        };
        MakeRequest(token, query);
    }
}