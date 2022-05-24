using DisCatSharp;
using Microsoft.Extensions.Logging;
using Serilog;



namespace Ben
{
    public static class Bot
    {
        public static void Main(string[] args) => MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();

        public static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Starting bot...");
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            DiscordConfiguration discordConfiguration = new()
            {
                Intents = DiscordIntents.All,
                Token = "TOKEN",
                LoggerFactory = new LoggerFactory().AddSerilog(Log.Logger)
            };

            DiscordClient discordClient = new(discordConfiguration);
            
            discordClient.MessageCreated += async (s, e) =>
            {
                if (e.Message.Content.ToLower().StartsWith("hi ben"))
                    await e.Message.RespondAsync("https://cdn.discordapp.com/attachments/818964071403487282/978702080619450408/IMG_2953.jpg");
                
                else if (e.Message.Content.ToLower().StartsWith("ben"))
                {
                    Random rnd = new Random();
                    int num = rnd.Next(1,4);
                    if (num == 1)
                        await e.Message.RespondAsync("https://cdn.discordapp.com/attachments/818964071403487282/978702080988545134/IMG_2954.jpg");
                    else if (num == 2)
                        await e.Message.RespondAsync("https://cdn.discordapp.com/attachments/818964071403487282/978702081286357042/IMG_2955.jpg");
                    else if (num == 3)
                        await e.Message.RespondAsync("https://cdn.discordapp.com/attachments/818964071403487282/978702081617719387/IMG_2956.jpg");
                    else
                        await e.Message.RespondAsync("you suck.");
                }

            };

            Log.Logger.Information("Connecting to Discord...");
            await discordClient.ConnectAsync();

            discordClient.Logger.LogInformation($"Connection success! Logged in as {discordClient.CurrentUser.Username}#{discordClient.CurrentUser.Discriminator} ({discordClient.CurrentUser.Id})");

            await Task.Delay(-1);
        }
    }
}
