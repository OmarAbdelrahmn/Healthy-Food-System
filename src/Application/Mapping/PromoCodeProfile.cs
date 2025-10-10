using Application.DTOs;
using AutoMapper;
using Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapping;
public class PromoCodeProfile : Profile
{
    public PromoCodeProfile()
    {
        CreateMap<PromoCode, PromoCodeDto>()
            .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.IsValid()));
        CreateMap<PromoCodeDto,PromoCode>();
        CreateMap<CreatePromoCodeDto, PromoCode>();
    }
}
