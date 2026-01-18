namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IQrCodeService
    {
        byte[] GenerateQrCode(string content, int pixelsPerModule = 20);
        string GenerateQrCodeBase64(string content, int pixelsPerModule = 20);
    }
}
