using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;
public interface IPromoCodeService
{
    Task<PromoCodeDto> CreatePromoCodeAsync(CreatePromoCodeDto createDto);
    Task<PromoCodeDto?> GetPromoCodeByIdAsync(Guid id);
    Task<PromoCodeDto?> GetPromoCodeByCodeAsync(string code);
    Task<IEnumerable<PromoCodeDto>> GetAllPromoCodesAsync();
    Task<PromoCodeDto> UpdatePromoCodeAsync(PromoCodeDto promoCodeDto);
    Task DeletePromoCodeAsync(Guid id);
    Task<PromoCodeApplicationResultDto> ApplyPromoCodeAsync(ApplyPromoCodeDto applyPromoCodeDto);
    Task<PromoCodeDto?> GetPromoCodeAsync(string code);
    Task DeactivatePromoCodeAsync(Guid id);
    Task<PromoCodeApplicationResultDto> ValidationPromoCodeAsync(ApplyPromoCodeDto applyPromoCodeDto);
    //Task UpdateUsageAsync(UpdatePromoCodeUsageDto updateDto);
    Task<bool> PromoCodeExistsAsync(string code);
    Task UpdateUsageAsync(UpdatePromoCodeUsageDto updateDto);
}
