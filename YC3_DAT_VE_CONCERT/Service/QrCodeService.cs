using QRCoder;
using YC3_DAT_VE_CONCERT.Interface;


namespace YC3_DAT_VE_CONCERT.Service
{
    public class QrCodeService : IQrCodeService
    {
        public byte[] GenerateQrCode(string content, int pixelsPerModule = 20)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);

            return qrCode.GetGraphic(pixelsPerModule);
        }

        public string GenerateQrCodeBase64(string content, int pixelsPerModule = 20)
        {
            var qrBytes = GenerateQrCode(content, pixelsPerModule);
            return Convert.ToBase64String(qrBytes);
        }
    }
}
