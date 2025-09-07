using System.Security.Cryptography;
using System.Text;

namespace payfast.integration.poc.Services;

public static class PayFastHelper
{
    public static string CreateSignature(Dictionary<string, string> data, string passphrase = "")
    {
        var dataString = string.Join("&", data
            .Where(kvp => !string.IsNullOrEmpty(kvp.Value))
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

        if (!string.IsNullOrEmpty(passphrase))
        {
            dataString += $"&passphrase={Uri.EscapeDataString(passphrase)}";
        }

        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(dataString));
        return Convert.ToHexString(hashBytes).ToLower();
    }

    public static bool ValidateSignature(Dictionary<string, string> data, string receivedSignature, string passphrase = "")
    {
        var calculatedSignature = CreateSignature(data, passphrase);
        return calculatedSignature.Equals(receivedSignature, StringComparison.OrdinalIgnoreCase);
    }

    public static Dictionary<string, string> ParseFormData(IFormCollection form)
    {
        return form.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());
    }
}
