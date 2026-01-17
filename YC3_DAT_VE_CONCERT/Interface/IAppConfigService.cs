namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IAppConfigService
    {
        string AppName { get; }
        string AppVersion { get; }
        int MaxTicketsPerOrder { get; }
        decimal ServiceFeePercentage { get; }
        bool IsMaintenanceMode { get; }
    }
}
