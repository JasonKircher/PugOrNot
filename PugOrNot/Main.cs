namespace DefaultNamespace;

public class Program
{
    public static async Task Main(string[] args)
    {   
        var access = new WarcraftLogsAccess();
        string token = await access.GetAccessToken();
        access.GetEncounterStats(token, 12661, "Betsy", "EU", "Gilneas", "dps");
        await Task.Delay(1000);
    }
}