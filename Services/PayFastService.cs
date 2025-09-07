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
        var formData = new Dictionary<string, string>
        {
            ["merchant_id"] = _config.MerchantId,
            ["merchant_key"] = _config.MerchantKey,
            ["return_url"] = _config.ReturnUrl,
            ["cancel_url"] = _config.CancelUrl,
            ["notify_url"] = _config.NotifyUrl,
            ["name_first"] = request.CustomerFirstName,
            ["name_last"] = request.CustomerLastName,
            ["email_address"] = request.CustomerEmail,
            ["cell_number"] = request.CustomerCell,
            ["m_payment_id"] = request.MerchantPaymentId,
            ["amount"] = request.Amount.ToString("F2"),
            ["item_name"] = request.ItemName,
            ["item_description"] = request.ItemDescription
        };

        var signature = PayFastHelper.CreateSignature(formData, _config.Passphrase);
        formData["signature"] = signature;

        var actionUrl = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
            ? _config.SandboxUrl
            : _config.BaseUrl;

        return GenerateHtmlForm(formData, actionUrl);
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

    private string GenerateHtmlForm(Dictionary<string, string> formData, string actionUrl)
    {
        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html>");
        html.AppendLine("<head><title>Redirecting to PayFast...</title></head>");
        html.AppendLine("<body>");
        html.AppendLine($"<form action='{actionUrl}' method='post' id='payfast_form'>");

        foreach (var kvp in formData)
        {
            html.AppendLine($"<input type='hidden' name='{kvp.Key}' value='{kvp.Value}' />");
        }

        html.AppendLine("<input type='submit' value='Pay Now' />");
        html.AppendLine("</form>");
        html.AppendLine("<script>document.getElementById('payfast_form').submit();</script>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return html.ToString();
    }
}
