using Telegram.Bot;
using Telegram.Bot.Polling;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var host = CreateHostBuilder(args).Build();

    }

    #region CreateHostBuilder
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                //var configuration = hostContext.Configuration;
                //var appConfig = new AppConfig();
                //configuration.Bind(appConfig);
                //services.AddSingleton(appConfig);


                //if (appConfig?.ConnectionStrings == null)
                //{
                //    throw new InvalidOperationException("ConnectionStrings is not configured.");
                //}

                //var connectionDefault = appConfig.ConnectionStrings["ConnectionDefault"];

                //if (string.IsNullOrEmpty(connectionDefault))
                //{
                //    throw new InvalidOperationException("ConnectionDefault is not set.");
                //}

                //services.AddDbContext<ApplicationDbContext>(options =>
                //    options.UseMySql(connectionDefault));

                //services.AddSingleton((provider) =>
                //{
                //    var appConfig = provider.GetRequiredService<AppConfig>();
                //    var dbContext = provider.GetRequiredService<ApplicationDbContext>();

                //    return new DatabaseService(appConfig, dbContext);

                //});

                //services.AddSingleton((provider) =>
                //{
                //    var appConfig = provider.GetRequiredService<AppConfig>();
                //    return new TelegramBotClient(appConfig.BotKeyRelease);
                //});
            });
    }
    #endregion
}