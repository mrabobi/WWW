using Serilog;
using VDS.RDF.Storage;

namespace SmartHome.Stardog.Services
{
    public class StardogService
    {
        protected readonly StardogData _data;
        protected readonly ILogger _logger;

        public StardogService(StardogData data,ILogger logger)
        {
            _data = data;
            _logger = logger;

        }

        public StardogConnector GetStardogConnector()
        {
            return new StardogConnector(_data.ServerAddress, _data.DatabaseName, _data.Username, _data.Password);
        }

        protected string GetUserObjectUrl(string baseUrl, string userId)
        {
            return $"{baseUrl}/Users/{userId}";
        }

        protected string GetGroupObjectUrl(string baseUrl, string groupId)
        {
            return $"{baseUrl}/Groups/{groupId}";
        }

        protected string GetClaimUrl(string baseUrl, string groupId)
        {
            return $"{baseUrl}/Groups/{groupId}";
        }

        protected string ConvertUserUrlToId(string url)
        {
            var userId = url?.ToString().Replace($"{_data.BaseObjectUrl}/Users/", "");
            return userId;
        }
    }
}
