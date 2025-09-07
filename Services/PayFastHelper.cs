using System.Security.Cryptography;
using System.Text;

namespace payfast.integration.poc.Services;

public static class PayFastHelper
{
    public static string CreateSignature(IEnumerable<KeyValuePair<string, string>> data, string passphrase = "")
    {
        // Only include non-blank variables, preserving order
        var filtered = data.Where(kvp => !string.IsNullOrEmpty(kvp.Value));
        var dataString = string.Join("&", filtered
            .Select(kvp => $"{kvp.Key}={PayFastUrlEncode(kvp.Value)}"));

        if (!string.IsNullOrEmpty(passphrase))
        {
            dataString += $"&passphrase={PayFastUrlEncode(passphrase)}";
        }

        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(dataString));
        return Convert.ToHexString(hashBytes).ToLower();
    }

    public static string PayFastUrlEncode(string value)
    {
        if (value == null) return string.Empty;
        var encoded = Uri.EscapeDataString(value)
            .Replace("!", "%21")
            .Replace("'", "%27")
            .Replace("(", "%28")
            .Replace(")", "%29")
            .Replace("*", "%2A")
            .Replace("~", "%7E")
            .Replace("%20", "+");
        return encoded;
    }


    public static bool ValidateSignature(IEnumerable<KeyValuePair<string, string>> data, string receivedSignature, string passphrase = "")
    {
        var calculatedSignature = CreateSignature(data, passphrase);
        return calculatedSignature.Equals(receivedSignature, StringComparison.OrdinalIgnoreCase);
    }

    public static Dictionary<string, string> ParseFormData(IFormCollection form)
    {
        return form.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());
    }
}
