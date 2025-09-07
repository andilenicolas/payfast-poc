using Microsoft.AspNetCore.Mvc;
using payfast.integration.poc.Models;
using payfast.integration.poc.Services;

namespace payfast.integration.poc.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPayFastService _payFastService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPayFastService payFastService, ILogger<PaymentController> logger)
    {
        _payFastService = payFastService;
        _logger = logger;
    }

    [HttpPost("initiate")]
    public async Task<IActionResult> InitiatePayment([FromBody] PaymentRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Generate unique payment ID
            request.MerchantPaymentId = Guid.NewGuid().ToString();

            var htmlForm = await _payFastService.CreatePaymentFormAsync(request);

            return Content(htmlForm, "text/html");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating payment");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    [HttpPost("notify")]
    public async Task<IActionResult> PaymentNotification()
    {
        try
        {
            var formData = PayFastHelper.ParseFormData(Request.Form);

            var itn = new PayFastItn
            {
                m_payment_id = formData.GetValueOrDefault("m_payment_id", ""),
                pf_payment_id = formData.GetValueOrDefault("pf_payment_id", ""),
                payment_status = formData.GetValueOrDefault("payment_status", ""),
                item_name = formData.GetValueOrDefault("item_name", ""),
                item_description = formData.GetValueOrDefault("item_description", ""),
                amount_gross = decimal.TryParse(formData.GetValueOrDefault("amount_gross", "0"), out var gross) ? gross : 0,
                amount_fee = decimal.TryParse(formData.GetValueOrDefault("amount_fee", "0"), out var fee) ? fee : 0,
                amount_net = decimal.TryParse(formData.GetValueOrDefault("amount_net", "0"), out var net) ? net : 0,
                custom_str1 = formData.GetValueOrDefault("custom_str1", ""),
                custom_str2 = formData.GetValueOrDefault("custom_str2", ""),
                custom_str3 = formData.GetValueOrDefault("custom_str3", ""),
                custom_str4 = formData.GetValueOrDefault("custom_str4", ""),
                custom_str5 = formData.GetValueOrDefault("custom_str5", ""),
                custom_int1 = formData.GetValueOrDefault("custom_int1", ""),
                custom_int2 = formData.GetValueOrDefault("custom_int2", ""),
                custom_int3 = formData.GetValueOrDefault("custom_int3", ""),
                custom_int4 = formData.GetValueOrDefault("custom_int4", ""),
                custom_int5 = formData.GetValueOrDefault("custom_int5", ""),
                name_first = formData.GetValueOrDefault("name_first", ""),
                name_last = formData.GetValueOrDefault("name_last", ""),
                email_address = formData.GetValueOrDefault("email_address", ""),
                merchant_id = formData.GetValueOrDefault("merchant_id", ""),
                signature = formData.GetValueOrDefault("signature", "")
            };

            var isValid = await _payFastService.ValidateItnAsync(itn);

            if (isValid)
            {
                _logger.LogInformation("Valid ITN received for payment {PaymentId}", itn.m_payment_id);

                // Process the successful payment here
                // Update database, send confirmation emails, etc.

                return Ok();
            }
            else
            {
                _logger.LogWarning("Invalid ITN received for payment {PaymentId}", itn.m_payment_id);
                return BadRequest("Invalid signature");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment notification");
            return StatusCode(500);
        }
    }

    [HttpGet("return")]
    public async Task<IActionResult> PaymentReturn([FromQuery] string payment_id, [FromQuery] string payment_status)
    {
        try
        {
            _logger.LogInformation("Payment return for {PaymentId} with status {Status}", payment_id, payment_status);

            // Handle successful return from PayFast
            var successHtml = GenerateSuccessPage(payment_id, payment_status);
            return Content(successHtml, "text/html");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment return");
            return StatusCode(500, "An error occurred");
        }
    }

    [HttpGet("cancel")]
    public async Task<IActionResult> PaymentCancel()
    {
        try
        {
            var request = HttpContext.Request;
            _logger.LogInformation("Payment cancelled for {PaymentId}", request.Query["payment_id"]);

            var cancelHtml = GenerateCancelPage(request.Query["payment_id"]);
            return Content(cancelHtml, "text/html");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment cancellation");
            return StatusCode(500, "An error occurred");
        }
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        var testPayment = new PaymentRequest
        {
            Amount = 100.00m,
            ItemName = "Test Item",
            ItemDescription = "Test Description",
            CustomerEmail = "test@example.com",
            CustomerFirstName = "John",
            CustomerLastName = "Doe",
            CustomerCell = "0123456789"
        };

        return Ok(testPayment);
    }

    private string GenerateSuccessPage(string paymentId, string status)
    {
        return $@"
<!DOCTYPE html>
<html>
<head><title>Payment Successful</title></head>
<body>
    <h1>Payment Successful!</h1>
    <p>Payment ID: {paymentId}</p>
    <p>Status: {status}</p>
    <a href='/'>Return to Home</a>
</body>
</html>";
    }

    private string GenerateCancelPage(string paymentId)
    {
        return $@"
<!DOCTYPE html>
<html>
<head><title>Payment Cancelled</title></head>
<body>
    <h1>Payment Cancelled</h1>
    <p>Payment ID: {paymentId}</p>
    <a href='/'>Return to Home</a>
</body>
</html>";
    }
}
