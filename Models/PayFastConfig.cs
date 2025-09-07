namespace payfast.integration.poc.Models;

public class PayFastConfig
{
    public string BaseUrl { get; set; } = string.Empty;
    public string SandboxUrl { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string MerchantKey { get; set; } = string.Empty;
    public string Passphrase { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
    public string NotifyUrl { get; set; } = string.Empty;
}
