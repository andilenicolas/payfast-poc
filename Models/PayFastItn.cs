namespace payfast.integration.poc.Models;

public class PayFastItn
{
    public string m_payment_id { get; set; } = string.Empty;
    public string pf_payment_id { get; set; } = string.Empty;
    public string payment_status { get; set; } = string.Empty;
    public string item_name { get; set; } = string.Empty;
    public string item_description { get; set; } = string.Empty;
    public decimal amount_gross { get; set; }
    public decimal amount_fee { get; set; }
    public decimal amount_net { get; set; }
    public string custom_str1 { get; set; } = string.Empty;
    public string custom_str2 { get; set; } = string.Empty;
    public string custom_str3 { get; set; } = string.Empty;
    public string custom_str4 { get; set; } = string.Empty;
    public string custom_str5 { get; set; } = string.Empty;
    public string custom_int1 { get; set; } = string.Empty;
    public string custom_int2 { get; set; } = string.Empty;
    public string custom_int3 { get; set; } = string.Empty;
    public string custom_int4 { get; set; } = string.Empty;
    public string custom_int5 { get; set; } = string.Empty;
    public string name_first { get; set; } = string.Empty;
    public string name_last { get; set; } = string.Empty;
    public string email_address { get; set; } = string.Empty;
    public string merchant_id { get; set; } = string.Empty;
    public string signature { get; set; } = string.Empty;
}
