namespace ChatGPT.REPL.Data;

public partial class ChatHistoryDbContext : DbContext
{
//    private const string CS =
//#if WINDOWS
//    "Data Source=C:\\GitHub\\sharpninja\\ChatGPT.REPL\\ChatGPT.REPL\\LocalDatabase.sqlite"
//#elif ANDROID
//    "ChatHistoryDatabase"
//#endif
//        ;
    public ChatHistoryDbContext(IConfiguration configuration)
        : base(new DbContextOptions<ChatHistoryDbContext>())
        => Configuration = configuration;

    public ChatHistoryDbContext(IConfiguration configuration, DbContextOptions<ChatHistoryDbContext> options)
        : base(options)
        => Configuration = configuration;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string cs = Configuration.GetConnectionString("ChatHistoryDatabase");
        optionsBuilder.UseSqlite(cs);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => OnModelCreatingPartial(modelBuilder);

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public DbSet<PromptResponse> PromptResponses
    {
        get;set;
    }
    public IConfiguration Configuration
    {
        get;
    }
}
