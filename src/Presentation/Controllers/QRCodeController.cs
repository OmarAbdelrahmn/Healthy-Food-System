using Infrastructure.Service.qrcode;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;
[Route("[controller]")]
[ApiController]
public class QRCodeController(IQRCodeService _qrCodeService) : ControllerBase
{
    private readonly IQRCodeService qrCodeService = _qrCodeService;

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GenerateOrderQRCode(int orderId)
    {
        try
        {
            byte[] qrCode = await qrCodeService.GenerateOrderQRCodeByIdAsync(orderId);
            return File(qrCode, "image/png", $"Order_{orderId}_QRCode.png");
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // Get QR code as Base64 (for web/mobile display)
    [HttpGet("order/{orderId}/base64")]
    public async Task<ActionResult<QRCodeResponse>> GetOrderQRCodeBase64(int orderId)
    {
        try
        {
            string qrCodeBase64 = await qrCodeService.GenerateOrderQRCodeBase64ByIdAsync(orderId);

            return Ok(new QRCodeResponse
            {
                OrderId = orderId,
                QRCodeBase64 = qrCodeBase64,
                QRCodeDataUrl = $"data:image/png;base64,{qrCodeBase64}"
            });
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // Download QR code
    [HttpGet("order/{orderId}/download")]
    public async Task<IActionResult> DownloadOrderQRCode(int orderId)
    {
        try
        {
            byte[] qrCode = await qrCodeService.GenerateOrderQRCodeByIdAsync(orderId);
            return File(qrCode, "image/png", $"Order_{orderId}_QRCode.png");
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // Generate QR codes for multiple orders (batch)
    [HttpPost("orders/batch")]
    public async Task<ActionResult<List<QRCodeResponse>>> GenerateBatchQRCodes([FromBody] List<int> orderIds)
    {
        var results = new List<QRCodeResponse>();

        foreach (var orderId in orderIds)
        {
            try
            {
                string qrCodeBase64 = await qrCodeService.GenerateOrderQRCodeBase64ByIdAsync(orderId);
                results.Add(new QRCodeResponse
                {
                    OrderId = orderId,
                    QRCodeBase64 = qrCodeBase64,
                    QRCodeDataUrl = $"data:image/png;base64,{qrCodeBase64}"
                });
            }
            catch
            {
                results.Add(new QRCodeResponse
                {
                    OrderId = orderId,
                    Error = "Order not found"
                });
            }
        }

        return Ok(results);
    }
}

// DTO
public class QRCodeResponse
{
    public int OrderId { get; set; }
    public string QRCodeBase64 { get; set; }
    public string QRCodeDataUrl { get; set; }
    public string Error { get; set; }
}

