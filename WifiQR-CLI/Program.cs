using Microsoft.Extensions.Configuration;
using QRCoder;

namespace WifiQR_CLI;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            var settings = config.GetSection(nameof(AppSettings)).Get<AppSettings>();
            if (settings is null)
            {
                throw new Exception("No appsettings found.");
            }

            string wifiPayload = $"WIFI:T:{settings.Auth};S:{settings.SSID};P:{settings.Password};H:{(settings.Hidden ? "true" : "")};;";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(wifiPayload, QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20);

            var outputName = "wifiqr.png";
            File.WriteAllBytes(outputName, qrCodeBytes);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"QR code saved to {outputName}");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("An error occurred");
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Console.ResetColor();
        }
    }
}