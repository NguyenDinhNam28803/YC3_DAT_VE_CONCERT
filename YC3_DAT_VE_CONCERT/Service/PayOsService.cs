using PayOS;

namespace YC3_DAT_VE_CONCERT.Service
{
    public class PayOsService
    {
        private readonly PayOSClient _payOSClient;
        public PayOsService(IConfiguration configuration)
        {
            var options = new PayOSOptions
            {
                ClientId = configuration["PayOS:ClientId"] ?? throw new ArgumentNullException("ClientId"),
                ApiKey = configuration["PayOS:ApiKey"] ?? throw new ArgumentNullException("ApiKey"),
                ChecksumKey = configuration["PayOS:ChecksumKey"] ?? throw new ArgumentNullException("ChecksumKey")
            };

            _payOSClient = new PayOSClient(options);
        }
    }
}
