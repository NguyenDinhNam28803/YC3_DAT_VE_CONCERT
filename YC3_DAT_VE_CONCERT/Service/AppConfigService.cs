using YC3_DAT_VE_CONCERT.Interface;

namespace YC3_DAT_VE_CONCERT.Service
{
    public class AppConfigService : IAppConfigService
    {
        private readonly IConfiguration _configuration;
        public AppConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string AppName => _configuration["AppSettings:AppName"] ?? "YC3 Concert";
        public string AppVersion => _configuration["AppSettings:Version"] ?? "1.0.0";
        public int MaxTicketsPerOrder => int.Parse(_configuration["AppSettings:MaxTicketsPerOrder"] ?? "10");
        public decimal ServiceFeePercentage => decimal.Parse(_configuration["AppSettings:ServiceFeePercentage"] ?? "5");
        public bool IsMaintenanceMode => bool.Parse(_configuration["AppSettings:IsMaintenanceMode"] ?? "false");
    }
}
