using System.ComponentModel.DataAnnotations;

namespace payfast.integration.poc.Models;

public class PaymentRequest
{
    [Required]
    [Range(0.01, 999999.99)]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(100)]
    public string ItemName { get; set; } = string.Empty;

    [StringLength(255)]
    public string ItemDescription { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string CustomerFirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string CustomerLastName { get; set; } = string.Empty;

    [Phone]
    public string CustomerCell { get; set; } = string.Empty;

    public string MerchantPaymentId { get; set; } = string.Empty;
}
