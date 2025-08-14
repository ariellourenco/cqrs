namespace CQRSJourney.Registration.Api;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public sealed class Program

{
    public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

