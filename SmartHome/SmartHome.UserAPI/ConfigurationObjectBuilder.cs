using Microsoft.Extensions.Configuration;
using SmartHome.Stardog;

namespace SmartHome.API
{
    public class ConfigurationObjectBuilder
    {
        public static StardogData BuildStardogData(IConfiguration config)
        {
            var stardogSection = config.GetSection("Stardog");
            var serverAddress = stardogSection.GetValue("Server", "http://localhost:5820");
            var databaseName = stardogSection.GetValue("Database", "SmartHome");
            var userName = stardogSection.GetValue("User", "admin");
            var password = stardogSection.GetValue("Password", "admin");
            var baseObjectUrl= stardogSection.GetValue("BaseSubjectUrl", "https://localhost:44310/api");
            return new StardogData { DatabaseName = databaseName, ServerAddress = serverAddress, Username = userName, Password = password,BaseObjectUrl= baseObjectUrl };
        }
    }
}
