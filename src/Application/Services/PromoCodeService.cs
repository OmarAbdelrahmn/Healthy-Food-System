using Application.DTOs;
using Application.Interfaces;
using Application.Mapping;
using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Models.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;
public class PromoCodeService : IPromoCodeService
{
    private readonly IPromoCodeRepository _promoCodeRepository;
    private readonly ILogger<PromoCodeService> _logger;
    public PromoCodeService(IPromoCodeRepository promoCodeRepository, ILogger<PromoCodeService> logger)
    {
        _promoCodeRepository = promoCodeRepository;
        _logger = logger;
    }

    // Implement the methods defined in IPromoCodeService here
    //public async Task<PromoCodeDto> CreatePromoCodeAsync(CreatePromoCodeDto createDto)
    //{
    //    if (await _promoCodeRepository.ExistsAsync(createDto.Code))
    //    {
    //        throw new InvalidOperationException("Promo code already exists.");
    //    }
    //    var promoCode = new PromoCode(
    //        createDto.Code,
    //        createDto.DiscountAmount,
    //        createDto.DiscountPercentage,
    //        createDto.ValidFrom,
    //        createDto.ValidTo,
    //        createDto.MaxUsageCount,
    //        createDto.MinimumOrderAmount
    //    );
    //    await _promoCodeRepository.AddAsync(promoCode);
    //    var result = _mapper.Map<PromoCodeDto>(promoCode);
    //    result.IsValid = promoCode.IsValid();
    //    return result;
    //}

    public async Task<PromoCodeDto> CreatePromoCodeAsync(CreatePromoCodeDto createDto)
    {
        _logger.LogInformation("Creating a new promo code with code: {Code}", createDto.Code);
        if (await _promoCodeRepository.ExistsAsync(createDto.Code))
        {
            _logger.LogWarning("Promo code with code {Code} already exists.", createDto.Code);
            throw new InvalidOperationException("Promo code already exists.");
        }
        var promoCode = createDto.ToEntity();
        await _promoCodeRepository.AddAsync(promoCode);
        await _promoCodeRepository.SaveChangesAsync();
            _logger.LogInformation("Promo code with code {Code} created successfully.", createDto.Code);
        return promoCode.ToDto();
    }

    public async Task<PromoCodeApplicationResultDto> ValidatePromoCodeAsync(ApplyPromoCodeDto applyPromoCodeDto)
    {
        _logger.LogInformation("Validating promo code: {Code}", applyPromoCodeDto.Code);
        var promoCode = await _promoCodeRepository.GetByCodeAsync(applyPromoCodeDto.Code);
        if (promoCode == null || !promoCode.IsValid())
        {
            _logger.LogWarning("Promo code {Code} is invalid or expired.", applyPromoCodeDto.Code);
            return new PromoCodeApplicationResultDto
            {
                IsValid = false,
                DiscountAmount = 0,
                FinalAmount = applyPromoCodeDto.OrderAmount,
                ErrorMessage = "Invalid or expired promo code."
            };
        }
        try
        {
            var discount = promoCode.ApplyDiscount(applyPromoCodeDto.OrderAmount);
            _logger.LogInformation("Promo code {Code} is valid. Discount amount: {Discount}", applyPromoCodeDto.Code, discount);
            var finalAmount = applyPromoCodeDto.OrderAmount - discount;
            return new PromoCodeApplicationResultDto
            {
                IsValid = true,
                DiscountAmount = discount,
                FinalAmount = applyPromoCodeDto.OrderAmount - discount,
                ErrorMessage = null,
                PromoCode = promoCode.ToDto()
            };
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Error applying promo code {Code}: {Error}", applyPromoCodeDto.Code, ex.Message);
            return new PromoCodeApplicationResultDto
            {
                IsValid = false,
                DiscountAmount = 0,
                FinalAmount = applyPromoCodeDto.OrderAmount,
                ErrorMessage = ex.Message
            };

        }
    }

    public async Task<PromoCodeApplicationResultDto> ApplyPromoCodeAsync(ApplyPromoCodeDto applyPromoCodeDto)
    {
        _logger.LogInformation("Applying promo code: {Code}", applyPromoCodeDto.Code);
        var promoCode = await _promoCodeRepository.GetByCodeAsync(applyPromoCodeDto.Code);
        if (promoCode == null || !promoCode.IsValid())
        {
            _logger.LogWarning("Promo code {Code} is invalid or expired.", applyPromoCodeDto.Code);
            return new PromoCodeApplicationResultDto
            {
                IsValid = false,
                DiscountAmount = 0,
                FinalAmount = applyPromoCodeDto.OrderAmount,
                ErrorMessage = "Invalid or expired promo code."
            };
        }
        try
        {
            var discount = promoCode.ApplyDiscount(applyPromoCodeDto.OrderAmount);
            promoCode.MarkAsUsed();
            await _promoCodeRepository.UpdateAsync(promoCode);
            await _promoCodeRepository.SaveChangesAsync();
            _logger.LogInformation("Promo code {Code} applied successfully. Discount amount: {Discount}", applyPromoCodeDto.Code, discount);
            return new PromoCodeApplicationResultDto
            {
                IsValid = true,
                DiscountAmount = discount,
                FinalAmount = applyPromoCodeDto.OrderAmount - discount,
                ErrorMessage = null,
                PromoCode = promoCode.ToDto()
            };
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Error applying promo code {Code}: {Error}", applyPromoCodeDto.Code, ex.Message);
            return new PromoCodeApplicationResultDto
            {
                IsValid = false,
                DiscountAmount = 0,
                FinalAmount = applyPromoCodeDto.OrderAmount,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<PromoCodeDto?> GetPromoCodeByIdAsync(Guid id)
    {
        var promoCode = await _promoCodeRepository.GetByIdAsync(id);
        if (promoCode == null) return null;
        var result = promoCode.ToDto();
        result.IsValid = promoCode.IsValid();
        return result;
    }

    public async Task<PromoCodeDto?> GetPromoCodeAsync(string code)
    {
        var promoCode = await _promoCodeRepository.GetByCodeAsync(code);
        return promoCode?.ToDto();
    }

    public async Task<IEnumerable<PromoCodeDto>> GetAllPromoCodesAsync()
    {
        var promoCodes = await _promoCodeRepository.GetAllAsync();
        return promoCodes.ToDtoList();
    }

    

    public async Task DeactivatePromoCodeAsync(Guid id)
    {
        var promoCode = await _promoCodeRepository.GetByIdAsync(id);
        if (promoCode != null)
        {
            promoCode.Deactivate();
            _promoCodeRepository.UpdateAsync(promoCode);
            await _promoCodeRepository.SaveChangesAsync();
            _logger.LogInformation("Promo code with ID {Id} has been deactivated.", id);
        }
    }

    public async Task DeletePromoCodeAsync(Guid id)
    {
        var promoCode = await _promoCodeRepository.GetByIdAsync(id);
        if (promoCode != null)
        {
            await _promoCodeRepository.DeleteAsync(promoCode);
            await _promoCodeRepository.SaveChangesAsync();
            _logger.LogInformation("Promo code with ID {Id} has been deleted.", id);
        }
    }

    public async Task UpdatePromoCodeAsync(UpdatePromoCodeUsageDto updateDto)
    {
        var promoCode = await _promoCodeRepository.GetByCodeAsync(updateDto.Code);
        if(promoCode != null && updateDto.MarkAsUsed)
        {
            promoCode.MarkAsUsed();
            await _promoCodeRepository.UpdateAsync(promoCode);
            await _promoCodeRepository.SaveChangesAsync();
            _logger.LogInformation("Promo code with code {Code} usage updated.", updateDto.Code);
        }

    }

public async Task<bool> PromoCodeExistsAsync(string code)
    {
        return await _promoCodeRepository.ExistsAsync(code);
    }

    public async Task<PromoCodeDto?> GetPromoCodeByCodeAsync(string code)
    {
        var promoCode = await _promoCodeRepository.GetByCodeAsync(code);
        if (promoCode == null) return null;
        var result = promoCode.ToDto();
        result.IsValid = promoCode.IsValid();
        return result;
    }

public async Task<PromoCodeApplicationResultDto> ValidationPromoCodeAsync (ApplyPromoCodeDto applyPromoCodeDto)
    {
        return await ValidatePromoCodeAsync(applyPromoCodeDto);
    }


    public Task UpdateUsageAsync(UpdatePromoCodeUsageDto updateDto)
    {
        return UpdatePromoCodeAsync(updateDto);
    }
    public async Task DeletePromoCodeAsync(string code)
    {
        var promoCode = await _promoCodeRepository.GetByCodeAsync(code);
        if (promoCode != null)
        {
            await _promoCodeRepository.DeleteAsync(promoCode);
            await _promoCodeRepository.SaveChangesAsync();
            _logger.LogInformation("Promo code with code {Code} has been deleted.", code);
        }
    }

    public async Task<PromoCodeDto> UpdatePromoCodeAsync(PromoCodeDto promoCodeDto)
    {
        var promoCode = await _promoCodeRepository.GetByIdAsync(promoCodeDto.Id);
        if (promoCode == null)
        {
            throw new InvalidOperationException("Promo code not found.");
        }
        await _promoCodeRepository.UpdateAsync(promoCode);
        await _promoCodeRepository.SaveChangesAsync();
        _logger.LogInformation("Promo code with ID {Id} has been updated.", promoCodeDto.Id);
        return promoCode.ToDto();

    }
}
