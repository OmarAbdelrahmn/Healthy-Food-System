using Domain.Models.Entities;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service.subscri;
internal class data
{
}

public class SubscriptionStatisticsDto
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int TotalOrders { get; set; }
    public int ActivePromoCodes { get; set; }
}

public class CustomerDetailsDtos
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public Guid PlanId { get; set; }
    public string PlanName { get; set; }
    public uint DaysLeft { get; set; }
    public uint LunchMealsLeft { get; set; }
    public uint CarbGrams { get; set; }
    public DateTime StartDate { get; set; }
    public Guid? PromoCodeId { get; set; }
    public string PromoCodeName { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsPaused { get; set; }
    public decimal TotalPrice { get; set; }
}

public class PromoCodeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string DiscountType { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public int MaxUsageCount { get; set; }
    public int CurrentUsageCount { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public bool IsActive { get; set; }
    public string OwnerUserId { get; set; }
}

public class CustomerFilterDto
{
    public string SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsPaused { get; set; }
    public Guid? PlanId { get; set; }
    public Guid? PromoCodeId { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public class CreateCustomerDto
{
    public string UserId { get; set; }
    public Guid PlanId { get; set; }
    public uint DaysLeft { get; set; }
    public uint LunchMealsLeft { get; set; }
    public uint CarbGrams { get; set; }
    public DateTime StartDate { get; set; }
    public Guid? PromoCodeId { get; set; }





  
}
