using System.Reflection;

using ChatGPT.REPL.SimpleMVC;

using CommunityToolkit.Maui;

namespace ChatGPT.REPL;

public static class MauiProgram
{
    public static IServiceProvider Services
    {
        get;
        private set;
    }
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();

        builder.Configuration.AddUserSecrets(typeof(MauiProgram).Assembly);
#endif

        builder.Configuration.AddConfiguration(BuildConfig());

        builder.Services.AddDbContext<ChatHistoryDbContext>();

        builder.Services.AddSingleton<ChatGptController>();

        builder.Services.AddSingleton(
            s => new OpenAI_API.OpenAIAPI(
                new OpenAI_API.APIAuthentication(
                    s.GetRequiredService<IConfiguration>().GetConnectionString("ChatGPT"))));

        MauiApp built = builder.Build();

        string cs = built.Configuration.GetConnectionString("ChatHistoryDatabase");
        ChatHistoryDbContext dbContext = built.Services.GetRequiredService<ChatHistoryDbContext>();

        string filename = cs.Split('=').Last();
        if (!File.Exists(filename))
        {
            dbContext.Database.EnsureCreated();
            dbContext.Database.Migrate();
        }
        Services = built.Services;
        return built;
    }

    private static IConfiguration BuildConfig()
    {
        Assembly callingAssembly = Assembly.GetEntryAssembly() ?? typeof(MauiProgram).Assembly;
        ConfigurationBuilder config = new();
#if WINDOWS
        Version versionRuntime = callingAssembly.GetName().Version;
        string assemblyLocation = Path.GetDirectoryName(System.AppContext.BaseDirectory); //CallingAssembly.Location
        string configFile = Path.Combine(assemblyLocation, "appsettings.windows.json");
        config.AddJsonFile(configFile, false);
#elif ANDROID
        string mainDir = FileSystem.Current.AppDataDirectory;
        config.AddInMemoryCollection(new Dictionary<string, string> {
            {
                "ConnectionStrings:ChatHistoryDatabase",
                $"Data Source={mainDir}/LocalDatabase.sqlite"
            }
        });

        string name = Array.Find(
            callingAssembly.GetManifestResourceNames(),
            n => n.IndexOf("appsettings", StringComparison.OrdinalIgnoreCase) > -1);

        if (name is { Length: > 0 })
        {
            using Stream stream = callingAssembly.GetManifestResourceStream(name);

            config.AddJsonStream(stream);
        }
#endif
        return config.Build();
    }
}
