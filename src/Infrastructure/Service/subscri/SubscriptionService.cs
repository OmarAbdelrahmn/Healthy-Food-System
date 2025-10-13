using Domain.Models.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service.subscri;
public class SubscriptionService(ApplicationDbContext context) : ISubscriptionService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<SubscriptionStatisticsDto> GetSubscriptionStatisticsAsync()
    {
        var totalCustomers = await _context.Subscriptions
            .Select(s => s.UserId)
            .Distinct()
            .CountAsync();

        var activeCustomers = await _context.Subscriptions
            .CountAsync(s => s.IsCurrent && !s.IsPaused);

        var totalOrders = await _context.Subscriptions.CountAsync();

        var now = DateTime.UtcNow;
        var activePromoCodes = await _context.PromoCodes
            .CountAsync(p => p.IsActive &&
                           p.ValidFrom <= now &&
                           p.ValidTo >= now &&
                           p.CurrentUsageCount < p.MaxUsageCount);

        return new SubscriptionStatisticsDto
        {
            TotalCustomers = totalCustomers,
            ActiveCustomers = activeCustomers,
            TotalOrders = totalOrders,
            ActivePromoCodes = activePromoCodes
        };
    }

    public async Task<PagedResult<CustomerDetailsDtos>> GetCustomersAsync(CustomerFilterDto filter)
    {
        var query = _context.Subscriptions
            .Include(s => s.Plan)
            .Include(s => s.PromoCode)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            query = query.Where(s => s.UserId.Contains(filter.SearchTerm));
        }

        if (filter.IsActive.HasValue)
        {
            query = query.Where(s => s.IsCurrent == filter.IsActive.Value);
        }

        if (filter.IsPaused.HasValue)
        {
            query = query.Where(s => s.IsPaused == filter.IsPaused.Value);
        }

        if (filter.PlanId.HasValue)
        {
            query = query.Where(s => s.PlanId == filter.PlanId.Value);
        }

        if (filter.PromoCodeId.HasValue)
        {
            query = query.Where(s => s.PromoCodeId == filter.PromoCodeId.Value);
        }

        if (filter.StartDateFrom.HasValue)
        {
            query = query.Where(s => s.StartDate >= filter.StartDateFrom.Value);
        }

        if (filter.StartDateTo.HasValue)
        {
            query = query.Where(s => s.StartDate <= filter.StartDateTo.Value);
        }

        var totalCount = await query.CountAsync();

        var subscriptions = await query
            .OrderByDescending(s => s.StartDate)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var items = subscriptions.Select(MapToCustomerDetailsDto).ToList();

        return new PagedResult<CustomerDetailsDtos>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<CustomerDetailsDtos> GetCustomerByIdAsync(Guid id)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Plan)
            .Include(s => s.PromoCode)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (subscription == null) return null;

        return MapToCustomerDetailsDto(subscription);
    }

    public async Task<CustomerDetailsDtos> AddCustomerAsync(CreateCustomerDto dto)
    {
        var plan = await _context.Plans.FindAsync(dto.PlanId);
        if (plan == null)
            throw new Exception("Plan not found");

        if (dto.PromoCodeId.HasValue)
        {
            var promoCode = await _context.PromoCodes.FindAsync(dto.PromoCodeId.Value);
            if (promoCode == null || !promoCode.IsActive)
                throw new Exception("Invalid or inactive promo code");

            var now = DateTime.UtcNow;
            if (promoCode.ValidFrom > now || promoCode.ValidTo < now)
                throw new Exception("Promo code is not valid at this time");

            if (promoCode.CurrentUsageCount >= promoCode.MaxUsageCount)
                throw new Exception("Promo code usage limit reached");
        }

        var subscription = new Subscription
        {
            UserId = dto.UserId,
            PlanId = dto.PlanId,
            Plan = plan,
            DaysLeft = dto.DaysLeft,
            LunchMealsLeft = dto.LunchMealsLeft,
            CarbGrams = dto.CarbGrams,
            StartDate = dto.StartDate,
            PromoCodeId = dto.PromoCodeId,
            IsCurrent = true,
            IsPaused = false,
            LunchCategories = new List<SubscriptionCategory>()
        };

        _context.Subscriptions.Add(subscription);

        // Increment promo code usage count
        if (dto.PromoCodeId.HasValue)
        {
            var promoCode = await _context.PromoCodes.FindAsync(dto.PromoCodeId.Value);
            promoCode!.CurrentUsageCount++;
            _context.PromoCodes.Update(promoCode);
        }

        await _context.SaveChangesAsync();

        return await GetCustomerByIdAsync(subscription.Id);
    }

    public async Task<bool> UpdateCustomerAsync(Guid id, CreateCustomerDto dto)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription == null) return false;

        var plan = await _context.Plans.FindAsync(dto.PlanId);
        if (plan == null) return false;

        var oldPromoCodeId = subscription.PromoCodeId;

        if (dto.PromoCodeId.HasValue && dto.PromoCodeId != oldPromoCodeId)
        {
            var promoCode = await _context.PromoCodes.FindAsync(dto.PromoCodeId.Value);
            if (promoCode == null || !promoCode.IsActive)
                throw new Exception("Invalid or inactive promo code");

            var now = DateTime.UtcNow;
            if (promoCode.ValidFrom > now || promoCode.ValidTo < now)
                throw new Exception("Promo code is not valid at this time");

            if (promoCode.CurrentUsageCount >= promoCode.MaxUsageCount)
                throw new Exception("Promo code usage limit reached");

            promoCode.CurrentUsageCount++;
            _context.PromoCodes.Update(promoCode);
        }

        if (oldPromoCodeId.HasValue && oldPromoCodeId != dto.PromoCodeId)
        {
            var oldPromoCode = await _context.PromoCodes.FindAsync(oldPromoCodeId.Value);
            if (oldPromoCode != null && oldPromoCode.CurrentUsageCount > 0)
            {
                oldPromoCode.CurrentUsageCount--;
                _context.PromoCodes.Update(oldPromoCode);
            }
        }

        subscription.UserId = dto.UserId;
        subscription.PlanId = dto.PlanId;
        subscription.Plan = plan;
        subscription.DaysLeft = dto.DaysLeft;
        subscription.LunchMealsLeft = dto.LunchMealsLeft;
        subscription.CarbGrams = dto.CarbGrams;
        subscription.StartDate = dto.StartDate;
        subscription.PromoCodeId = dto.PromoCodeId;

        _context.Subscriptions.Update(subscription);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteCustomerAsync(Guid id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription == null) return false;

        // Decrement promo code usage count if exists
        if (subscription.PromoCodeId.HasValue)
        {
            var promoCode = await _context.PromoCodes.FindAsync(subscription.PromoCodeId.Value);
            if (promoCode != null && promoCode.CurrentUsageCount > 0)
            {
                promoCode.CurrentUsageCount--;
                _context.PromoCodes.Update(promoCode);
            }
        }

        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> PauseSubscriptionAsync(Guid id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription == null) return false;

        subscription.IsPaused = true;
        _context.Subscriptions.Update(subscription);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ResumeSubscriptionAsync(Guid id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription == null) return false;

        subscription.IsPaused = false;
        _context.Subscriptions.Update(subscription);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<PromoCodeDto>> GetActivePromoCodesAsync()
    {
        var now = DateTime.UtcNow;
        var promoCodes = await _context.PromoCodes
            .Where(p => p.IsActive &&
                       p.ValidFrom <= now &&
                       p.ValidTo >= now &&
                       p.CurrentUsageCount < p.MaxUsageCount)
            .ToListAsync();

        return promoCodes.Select(MapToPromoCodeDto).ToList();
    }



    public async Task<byte[]> ExportCustomersAsync(CustomerFilterDto filter)
    {
        var allCustomers = await GetCustomersAsync(new CustomerFilterDto
        {
            SearchTerm = filter.SearchTerm,
            IsActive = filter.IsActive,
            IsPaused = filter.IsPaused,
            PlanId = filter.PlanId,
            PromoCodeId = filter.PromoCodeId,
            StartDateFrom = filter.StartDateFrom,
            StartDateTo = filter.StartDateTo,
            PageNumber = 1,
            PageSize = int.MaxValue
        });

        var csv = new StringBuilder();
        csv.AppendLine("UserId,PlanName,DaysLeft,MealsLeft,CarbGrams,StartDate,IsCurrent,IsPaused,TotalPrice,PromoCode,DiscountAmount,DiscountPercentage");

        foreach (var customer in allCustomers.Items)
        {
            csv.AppendLine($"\"{customer.UserId}\",\"{customer.PlanName}\",{customer.DaysLeft},{customer.LunchMealsLeft},{customer.CarbGrams},{customer.StartDate:yyyy-MM-dd},{customer.IsCurrent},{customer.IsPaused},{customer.TotalPrice:F2},\"{customer.PromoCodeName ?? "N/A"}\",{customer.DiscountAmount?.ToString("F2") ?? "0"},{customer.DiscountPercentage?.ToString("F2") ?? "0"}");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    // Manual Mapping Methods
    private CustomerDetailsDtos MapToCustomerDetailsDto(Subscription subscription)
    {
        return new CustomerDetailsDtos
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            PlanId = subscription.PlanId,
            PlanName = subscription.Plan?.Name ?? "Unknown",
            DaysLeft = subscription.DaysLeft,
            LunchMealsLeft = subscription.LunchMealsLeft,
            CarbGrams = subscription.CarbGrams,
            StartDate = subscription.StartDate,
            PromoCodeId = subscription.PromoCodeId,
            PromoCodeName = subscription.PromoCode?.Code,
            DiscountAmount = subscription.PromoCode?.DiscountAmount,
            DiscountPercentage = subscription.PromoCode?.DiscountPercentage,
            IsCurrent = subscription.IsCurrent,
            IsPaused = subscription.IsPaused,
            TotalPrice = subscription.GetTotalPrice()
        };
    }

    private PromoCodeDto MapToPromoCodeDto(PromoCode promoCode)
    {
        return new PromoCodeDto
        {
            Id = promoCode.Id,
            Code = promoCode.Code,
            DiscountType = promoCode.DiscountType.ToString(),
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
}
