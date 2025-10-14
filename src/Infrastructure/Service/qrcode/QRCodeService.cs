using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Service.qrcode;
public class QRCodeService(ApplicationDbContext context) : IQRCodeService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<byte[]> GenerateOrderQRCodeByIdAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Subscription)
            .Include(o => o.Meals)
            .ThenInclude(m => m.Meal)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            throw new Exception($"Order with ID {orderId} not found");

        var orderData = new
        {
            OrderId = order.Id,
            SubscriptionId = order.SubscriptionId,
            OrderDate = order.OrderDate.ToString("yyyy-MM-dd"),
            DeliveryDate = order.DeliveryDate?.ToString("yyyy-MM-dd"),
            DayNumber = order.DayNumber,
            IsCompleted = order.IsCompleted,
            TotalMeals = order.Meals?.Count ?? 0,
            Meals = order.Meals!.Select(c => new
            {
                MealType = c.MealType.ToString(),
                Notes = c.Notes,
                Meal = new { c.Meal!.Id, c.Meal.Name, c.Meal.Description, c.Meal.ImageUrl, c.Meal.DefaultQuantityGrams }
            })


        };

        string jsonData = JsonSerializer.Serialize(orderData, new JsonSerializerOptions
        {
            WriteIndented = false
        });

        return GenerateQRCodeBytes(jsonData);
    }



    public async Task<string> GenerateOrderQRCodeBase64ByIdAsync(int orderId)
    {
        byte[] qrBytes = await GenerateOrderQRCodeByIdAsync(orderId);
        return Convert.ToBase64String(qrBytes);
    }

    private byte[] GenerateQRCodeBytes(string content)
    {
        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        {
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M);
            using (QRCode qrCode = new QRCode(qrCodeData))
            {
                using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        qrCodeImage.Save(ms, ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }
        }
    }

}
