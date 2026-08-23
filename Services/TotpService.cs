using OtpNet;
using QRCoder;

namespace TOTPDemo.WebAPI.Services;

public sealed class TotpService
{
    public string GenerateSecret()
    {
        byte[] keys = KeyGeneration.GenerateRandomKey(20);
        var secret = Base32Encoding.ToString(keys);
        return secret;
    }

    public string GenerateCode(string secretKey)
    {
        byte[] keys = Base32Encoding.ToBytes(secretKey);
        var totp = new Totp(keys);
        var code = totp.ComputeTotp();
        return code;
    }

    public bool Verify(string secretKey, string code)
    {
        byte[] keys = Base32Encoding.ToBytes(secretKey);
        var totp = new Totp(keys);
        var res = totp.VerifyTotp(
            code,
            out long timeStepMached,
            new VerificationWindow(previous: 1, future: 1));
        return res;
    }

    public string GenerateOtpUri(
        string secret,
        string email)
    {
        string issuer = "TOTP Demo";

        return $"otpauth://totp/{issuer}:{email}" +
               $"?secret={secret}" +
               $"&issuer={issuer}";
    }

    public byte[] GenerateQrCode(string secret, string email)
    {
        var otpUri = GenerateOtpUri(secret, email);

        using var qrCodeGenerator = new QRCodeGenerator();

        using var qrCodeData = qrCodeGenerator.CreateQrCode(
            otpUri,
            QRCodeGenerator.ECCLevel.Q);

        var qrCode = new PngByteQRCode(qrCodeData);

        return qrCode.GetGraphic(20);
    }

}
