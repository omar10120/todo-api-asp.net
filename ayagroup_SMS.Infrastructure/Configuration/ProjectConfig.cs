using Microsoft.Extensions.Configuration;


namespace ayagroup_SMS.Infrastructure.Configuration
{
    public class ProjectConfig
    {
        private static readonly Lazy<ProjectConfig> _instance =
            new Lazy<ProjectConfig>(() => new ProjectConfig());

        public static ProjectConfig Instance => _instance.Value;

        public JwtSettings JwtSettings { get; private set; }
        
        public string ConnectionString { get; private set; }
        public string BaseUrl { get; private set; }

        private ProjectConfig()
        {
    
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

          
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) 
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true); 

            IConfiguration configuration = configBuilder.Build();

            JwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
            BaseUrl = configuration["FileSettings:BaseUrl"];
        }
    }
}
