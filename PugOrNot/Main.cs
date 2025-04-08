namespace DefaultNamespace;

public class Program
{
    public static async Task Main(string[] args)
    {
        var access = new WarcraftLogsAccess();
        string token = await access.GetAccessToken();
        Console.WriteLine(token);
        access.GetEncounterStats(token, 43, "Betsy", "EU", "Gilneas", "dps");
        await Task.Delay(1000);
    }
}