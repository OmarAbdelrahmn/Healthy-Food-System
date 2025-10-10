using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;
public class PromoCodeService : IPromoCodeService
{
    private readonly IPromoCodeRepository _promoCodeRepository;
    private readonly IMapper _mapper;
    public PromoCodeService(IPromoCodeRepository promoCodeRepository, IMapper mapper)
    {
        _promoCodeRepository = promoCodeRepository;
        _mapper = mapper;
    }

    // Implement the methods defined in IPromoCodeService here
    public async Task<PromoCodeDto> CreatePromoCodeAsync(CreatePromoCodeDto createDto)
    {
        if (await _promoCodeRepository.ExistsAsync(createDto.Code))
        {
            throw new InvalidOperationException("Promo code already exists.");
        }
        var promoCode = new PromoCode(
            createDto.Code,
            createDto.DiscountAmount,
            createDto.DiscountPercentage,
            createDto.ValidFrom,
            createDto.ValidTo,
            createDto.MaxUsageCount,
            createDto.MinimumOrderAmount
        );
        await _promoCodeRepository.AddAsync(promoCode);
        var result = _mapper.Map<PromoCodeDto>(promoCode);
        result.IsValid = promoCode.IsValid();
        return result;
    }
    public async Task<PromoCodeDto?> GetPromoCodeByIdAsync(Guid id)
    {
        var promoCode = await _promoCodeRepository.GetByIdAsync(id);
        return promoCode == null ? null : _mapper.Map<PromoCodeDto>(promoCode);
    }

    public async Task<PromoCodeDto?> GetPromoCodeByCodeAsync(string code)
    {
        var promoCode = await _promoCodeRepository.GetByCodeAsync(code);
        return promoCode == null ? null : _mapper.Map<PromoCodeDto>(promoCode);
    }

    public async Task<IEnumerable<PromoCodeDto>> GetAllPromoCodesAsync()
    {
        var promoCodes = await _promoCodeRepository.GetAllAsync();
        var result = _mapper.Map<IEnumerable<PromoCodeDto>>(promoCodes);
        foreach (var item in result)
        {
            var promoCode = promoCodes.First(pc => pc.Id == item.Id);
            item.IsValid = promoCode.IsValid();
        }
        return result;
    }

    public async Task<PromoCodeDto> UpdatePromoCodeAsync(PromoCodeDto promoCodeDto)
    {
        var existingPromoCode = await _promoCodeRepository.GetByIdAsync(promoCodeDto.Id);
        if (existingPromoCode == null)
        {
            throw new InvalidOperationException("Promo code not found.");
        }
        // Update properties
        existingPromoCode.IsActive = promoCodeDto.IsActive;
        // Note: Other properties like Code, ValidFrom, ValidTo, etc. are typically not updated to maintain integrity
        await _promoCodeRepository.UpdateAsync(existingPromoCode);
        var result = _mapper.Map<PromoCodeDto>(existingPromoCode);
        result.IsValid = existingPromoCode.IsValid();
        return result;
    }

    public async Task<PromoCodeApplicationResultDto> ApplyPromoCodeAsync(ApplyPromoCodeDto applyPromoCodeDto)
    {
        var promoCode = await _promoCodeRepository.GetByCodeAsync(applyPromoCodeDto.Code);

        if (promoCode == null || !promoCode.IsValid())
        {
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
            return new PromoCodeApplicationResultDto
            {
                IsValid = true,
                DiscountAmount = discount,
                FinalAmount = applyPromoCodeDto.OrderAmount - discount,
                ErrorMessage = null
            };
        }
        catch (InvalidOperationException ex)
        {
            return new PromoCodeApplicationResultDto
            {
                IsValid = false,
                DiscountAmount = 0,
                FinalAmount = applyPromoCodeDto.OrderAmount,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<PromoCodeDto?> GetPromoCodeAsync(string code)
    {
        var promoCode = await _promoCodeRepository.GetByCodeAsync(code);
        if (promoCode == null) return null;
        var result = _mapper.Map<PromoCodeDto>(promoCode);
        result.IsValid = promoCode.IsValid();
        return result;
    }

    public async Task DeactivatePromoCodeAsync(Guid id)
    {
        var promoCode = await _promoCodeRepository.GetByIdAsync(id);
        if (promoCode == null)
        {
            throw new InvalidOperationException("Promo code not found.");
        }
        promoCode.IsActive = false;
        await _promoCodeRepository.UpdateAsync(promoCode);
    }

    public async Task DeletePromoCodeAsync(Guid id)
    {
        var promoCode = await _promoCodeRepository.GetByIdAsync(id);
        if (promoCode == null)
        {
            throw new InvalidOperationException("Promo code not found.");
        }
        await _promoCodeRepository.DeleteAsync(promoCode);
    }
}
