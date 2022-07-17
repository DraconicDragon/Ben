using System.Diagnostics;
using DisCatSharp;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Runtime.InteropServices;
using System.Text;



namespace Ben
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Utsname 
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65)]
        public byte[] sysname;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65)]
        public byte[] machine;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65)]
        public byte[] release;
    }
    

    public static class Uts
    {
        [DllImport("libc", EntryPoint = "uname", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uname(ref Utsname buf);

        public static string PrintUtsname()
        {
            Utsname buf = new Utsname();
            uname(ref buf);

            Process currentProcess = Process.GetCurrentProcess();
            long usedMemory = currentProcess.PrivateMemorySize64 / 1024 / 1024;

            string stats = $"Running on: {GetString(buf.sysname)} {GetString(buf.machine)}\n" +
                           $"release: {GetString(buf.release)}\n" +
                           $"Ben uses {usedMemory} MB of memory";

            return stats;
        }

        private static string GetString(in byte[] data)
        {
            var pos = Array.IndexOf<byte>(data, 0);
            return Encoding.ASCII.GetString(data, 0, (pos < 0) ? data.Length : pos);
        }
    }

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

            discordClient.MessageCreated += async (s, e) => // s = DiscordClient, e = MessageCreateEventArgs
            
            {
                string msgContent = e.Message.Content.ToLower();
                
                if (msgContent.StartsWith("hi ben"))
                    await e.Message.RespondAsync(
                        "https://cdn.discordapp.com/attachments/818964071403487282/978702080619450408/IMG_2953.jpg");

                else if (msgContent.StartsWith("ben"))
                {
                    Random rnd = new Random();
                    int num = rnd.Next(1, 4);

                    if (msgContent.Contains("ping"))
                        if (num == 1)
                            await e.Message.RespondAsync($"Hohoho {discordClient.Ping}ms");
                        else if (num == 2)
                            await e.Message.RespondAsync($"YEEEES {discordClient.Ping}ms");
                        else
                            await e.Message.RespondAsync($"Nooo {discordClient.Ping}ms");

                    else if (msgContent.Contains("info"))
                        await e.Message.RespondAsync(Uts.PrintUtsname());
                    
                    
                    else
                    {
                        if (num == 1)
                            await e.Message.RespondAsync(
                                "https://cdn.discordapp.com/attachments/818964071403487282/978702080988545134/IMG_2954.jpg");
                        else if (num == 2)
                            await e.Message.RespondAsync(
                                "https://cdn.discordapp.com/attachments/818964071403487282/978702081286357042/IMG_2955.jpg");
                        else
                            await e.Message.RespondAsync(
                                "https://cdn.discordapp.com/attachments/818964071403487282/978702081617719387/IMG_2956.jpg");
                    }
                }
            };

            Log.Logger.Information("Connecting to Discord...");
            await discordClient.ConnectAsync();

            discordClient.Logger.LogInformation(
                $"Connection success! Logged in as {discordClient.CurrentUser.Username}#{discordClient.CurrentUser.Discriminator} ({discordClient.CurrentUser.Id})");

            await Task.Delay(-1);

        }
    }
}
