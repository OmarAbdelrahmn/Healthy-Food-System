using Application.DTOs;
using AutoMapper;
using Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapping;
public static class PromoCodeMapper
{
    public static PromoCodeDto ToDto (this PromoCode promoCode)
    {
        if (promoCode == null) return null;
        return new PromoCodeDto
        {
            Id = promoCode.Id,
            Code = promoCode.Code,
            DiscountAmount = promoCode.DiscountAmount,
            DiscountPercentage = promoCode.DiscountPercentage,
            ValidFrom = promoCode.ValidFrom,
            ValidTo = promoCode.ValidTo,
            MaxUsageCount = promoCode.MaxUsageCount,
            CurrentUsageCount = promoCode.CurrentUsageCount,
            MinimumOrderAmount = promoCode.MinimumOrderAmount,
            IsActive = promoCode.IsActive,
            OwnerUserId = promoCode.OwnerUserId
        };
    }

    public static PromoCode ToEntity(this CreatePromoCodeDto createDto)
    {
        return new PromoCode(
            createDto.Code,
            createDto.DiscountAmount,
            createDto.DiscountPercentage,
            createDto.ValidFrom,
            createDto.ValidTo,
            createDto.MaxUsageCount,
            createDto.MinimumOrderAmount
        );
    }

    public static List<PromoCodeDto> ToDtoList(this IEnumerable<PromoCode> promoCodes)
    {
        return promoCodes?.Select(pc => pc.ToDto()).ToList() ?? new List<PromoCodeDto>();
    }
}
