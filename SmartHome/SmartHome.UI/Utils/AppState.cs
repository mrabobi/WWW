using SmartHome.UI.Data;

namespace SmartHome.UI_Auth.Utils
{
    public class AppState
    {
        public static UserModel CurrentUser { get; set; }
        public static string CurrentGroupId { get; set; }
        public static string DeviceApiUrl { get; set; }
    }
}
