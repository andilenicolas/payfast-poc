using payfast.integration.poc.Models;

namespace payfast.integration.poc.Services;

public interface IPayFastService
{
    Task<string> CreatePaymentFormAsync(PaymentRequest request);
    Task<bool> ValidateItnAsync(PayFastItn itn);
    Task<bool> ValidatePaymentAsync(string paymentId, decimal amount);
}
