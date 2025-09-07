using Microsoft.Extensions.Options;
using payfast.integration.poc.Models;
using System.Text;

namespace payfast.integration.poc.Services;

public class PayFastService : IPayFastService
{
    private readonly PayFastConfig _config;
    private readonly HttpClient _httpClient;
    private readonly ILogger<PayFastService> _logger;

    public PayFastService(IOptions<PayFastConfig> config, HttpClient httpClient, ILogger<PayFastService> logger)
    {
        _config = config.Value;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> CreatePaymentFormAsync(PaymentRequest request)
    {
        var formData = new List<KeyValuePair<string, string>>
        {
            new("merchant_id", _config.MerchantId),
            new("merchant_key", _config.MerchantKey),
            new("return_url", _config.ReturnUrl),
            new("cancel_url", _config.CancelUrl+"/"+request.MerchantPaymentId),
            new("notify_url", _config.NotifyUrl),
            new("name_first", request.CustomerFirstName),
            new("name_last", request.CustomerLastName),
            new("email_address", request.CustomerEmail),
            new("cell_number", request.CustomerCell),
            new("m_payment_id", request.MerchantPaymentId),
            new("amount", FormatAmountForPayFast(request.Amount)),
            new("item_name", request.ItemName),
            new("item_description", request.ItemDescription)
        };

        var signature = PayFastHelper.CreateSignature(formData, _config.Passphrase);
        formData.Add(new KeyValuePair<string, string>("signature", signature));

        var actionUrl = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
            ? _config.SandboxUrl
            : _config.BaseUrl;

        var result = $"{actionUrl}?{string.Join("&", formData.Select(kvp => $"{kvp.Key}={PayFastUrlEncode(kvp.Value)}"))}";

        return GenerateHtmlForm(formData, actionUrl);
    }

    private static string FormatAmountForPayFast(decimal amount)
    {
        // PayFast requires amounts formatted with dot as decimal separator (e.g., "1.00", not "1,00")
        return amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
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

    public async Task<bool> ValidateItnAsync(PayFastItn itn)
    {
        try
        {
            // Convert ITN to dictionary for signature validation
            var data = new Dictionary<string, string>
            {
                ["m_payment_id"] = itn.m_payment_id,
                ["pf_payment_id"] = itn.pf_payment_id,
                ["payment_status"] = itn.payment_status,
                ["item_name"] = itn.item_name,
                ["item_description"] = itn.item_description,
                ["amount_gross"] = itn.amount_gross.ToString("F2"),
                ["amount_fee"] = itn.amount_fee.ToString("F2"),
                ["amount_net"] = itn.amount_net.ToString("F2"),
                ["custom_str1"] = itn.custom_str1,
                ["custom_str2"] = itn.custom_str2,
                ["custom_str3"] = itn.custom_str3,
                ["custom_str4"] = itn.custom_str4,
                ["custom_str5"] = itn.custom_str5,
                ["custom_int1"] = itn.custom_int1,
                ["custom_int2"] = itn.custom_int2,
                ["custom_int3"] = itn.custom_int3,
                ["custom_int4"] = itn.custom_int4,
                ["custom_int5"] = itn.custom_int5,
                ["name_first"] = itn.name_first,
                ["name_last"] = itn.name_last,
                ["email_address"] = itn.email_address,
                ["merchant_id"] = itn.merchant_id
            };

            return PayFastHelper.ValidateSignature(data, itn.signature, _config.Passphrase);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating ITN");
            return false;
        }
    }

    public async Task<bool> ValidatePaymentAsync(string paymentId, decimal amount)
    {
        // Additional validation logic can be implemented here
        // This could include calling PayFast APIs to verify payment status
        return true;
    }

    private string GenerateHtmlForm(IEnumerable<KeyValuePair<string, string>> formData, string actionUrl)
    {
        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html>");
        html.AppendLine("<head><title>Redirecting to PayFast...</title></head>");
        html.AppendLine("<body>");
        html.AppendLine($"<form action='{actionUrl}' method='post' id='payfast_form'>");

        foreach (var kvp in formData)
        {
            html.AppendLine($"<input type='hidden' name='{kvp.Key}' value='{System.Net.WebUtility.HtmlEncode(kvp.Value)}' />");
        }

        html.AppendLine("<input type='submit' value='Pay Now' />");
        html.AppendLine("</form>");
        html.AppendLine("<script>document.getElementById('payfast_form').submit();</script>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return html.ToString();
    }
}
