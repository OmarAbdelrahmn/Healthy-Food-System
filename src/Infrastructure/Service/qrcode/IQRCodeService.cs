using Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service.qrcode;
public interface IQRCodeService
{

    Task<byte[]> GenerateOrderQRCodeByIdAsync(int orderId);
    Task<string> GenerateOrderQRCodeBase64ByIdAsync(int orderId);
}
