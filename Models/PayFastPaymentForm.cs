namespace payfast.integration.poc.Models;

public class PayFastPaymentForm
{
    public string merchant_id { get; set; } = string.Empty;
    public string merchant_key { get; set; } = string.Empty;
    public string return_url { get; set; } = string.Empty;
    public string cancel_url { get; set; } = string.Empty;
    public string notify_url { get; set; } = string.Empty;
    public string name_first { get; set; } = string.Empty;
    public string name_last { get; set; } = string.Empty;
    public string email_address { get; set; } = string.Empty;
    public string cell_number { get; set; } = string.Empty;
    public string m_payment_id { get; set; } = string.Empty;
    public string amount { get; set; } = string.Empty;
    public string item_name { get; set; } = string.Empty;
    public string item_description { get; set; } = string.Empty;
    public string signature { get; set; } = string.Empty;
}
