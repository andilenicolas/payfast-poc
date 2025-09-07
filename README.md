# PayFast Integration POC

This is a Proof of Concept (POC) for integrating PayFast payment gateway with an ASP.NET Core Web API.

## Features

- ✅ PayFast payment initiation
- ✅ Signature generation and validation
- ✅ ITN (Instant Transaction Notification) handling
- ✅ Return and cancel URL handling
- ✅ Development sandbox support
- ✅ Comprehensive logging
- ✅ Input validation
- ✅ Test interface

## Quick Start

### 1. Restore packages

```bash
dotnet restore
```

### 2. Run the application

```bash
dotnet run
```

### 3. Open your browser

- Main page: https://localhost:7000
- API Documentation: https://localhost:7000/swagger
- Test Payment: https://localhost:7000/test-payment.html

## API Endpoints

### Payment Endpoints

- `POST /api/payment/initiate` - Initiate a new payment
- `POST /api/payment/notify` - PayFast ITN callback (webhook)
- `GET /api/payment/return` - Payment success return URL
- `GET /api/payment/cancel` - Payment cancellation URL
- `GET /api/payment/test` - Test endpoint

## Configuration

### Development (Sandbox)

The application is pre-configured with PayFast sandbox credentials for development:

- Merchant ID: `10000100`
- Merchant Key: `46f0cd694581a`
- Passphrase: `jt7NOE43FZPn`

### Production

For production, update the `PayFast` section in `appsettings.json`:

```json
{
  "PayFast": {
    "MerchantId": "your-production-merchant-id",
    "MerchantKey": "your-production-merchant-key",
    "Passphrase": "your-production-passphrase"
  }
}
```

## Testing

### Manual Testing

1. Go to https://localhost:7000/test-payment.html
2. Fill in the payment details
3. Click "Pay Now with PayFast"
4. You'll be redirected to PayFast sandbox
5. Use test card details to complete payment

### API Testing

Use Swagger UI at https://localhost:7000/swagger to test the API endpoints directly.

### Test Card Details (Sandbox)

- Card Number: `4000000000000002`
- Expiry: Any future date
- CVV: Any 3 digits

## Architecture

```
├── Controllers/
│   └── PaymentController.cs      # Payment API endpoints
├── Models/
│   ├── PayFastConfig.cs         # Configuration model
│   ├── PaymentRequest.cs        # Payment request DTO
│   ├── PayFastPaymentForm.cs    # PayFast form model
│   └── PayFastItn.cs           # ITN callback model
├── Services/
│   ├── IPayFastService.cs      # Service interface
│   ├── PayFastService.cs       # Service implementation
│   └── PayFastHelper.cs        # Utility functions
└── wwwroot/
    ├── index.html              # Landing page
    └── test-payment.html       # Test form
```

## Key Components

### PayFastService

Handles payment form generation, signature creation, and ITN validation.

### PayFastHelper

Utility class for signature generation and validation using MD5 hashing.

### PaymentController

REST API controller exposing payment endpoints.

## Security Features

- ✅ MD5 signature validation
- ✅ Input validation with data annotations
- ✅ HTTPS enforcement
- ✅ Passphrase protection
- ✅ Error handling and logging

## ITN (Instant Transaction Notification)

For PayFast to send ITN callbacks in development, you'll need to expose your local server:

### Using ngrok

```bash
# Install ngrok
npm install -g ngrok

# Expose local server
ngrok http 7000

# Update NotifyUrl in appsettings.Development.json with ngrok URL
```

### Using Visual Studio Dev Tunnels

```bash
devtunnel create --allow-anonymous
devtunnel port create -p 7000
devtunnel host
```

## Logging

The application logs important events:

- Payment initiations
- ITN validations
- Return/cancel events
- Errors and warnings

## Next Steps

For production deployment:

1. **Security**: Use User Secrets or Azure Key Vault for credentials
2. **Database**: Implement payment tracking with Entity Framework
3. **Monitoring**: Add Application Insights or similar
4. **Testing**: Add unit and integration tests
5. **Documentation**: API documentation with examples
6. **Validation**: Additional business logic validation
7. **Error Handling**: More sophisticated error handling
8. **Notification**: Email/SMS confirmations

## Troubleshooting

### Common Issues

1. **Signature Validation Fails**

   - Check merchant credentials
   - Verify passphrase
   - Ensure data ordering is correct

2. **ITN Not Received**

   - Check NotifyUrl is publicly accessible
   - Verify PayFast can reach your server
   - Check firewall settings

3. **Payment Form Not Generated**
   - Check configuration settings
   - Verify model validation
   - Check logs for errors

## Support

- PayFast Documentation: https://www.payfast.co.za/developers/
- PayFast Support: support@payfast.co.za
- Sandbox Testing: https://sandbox.payfast.co.za
